# REST API with SMC

* [SMC API service with database persistence](#smc-api-service-with-database-persistence)
* [RESTful API client with HTTP header persistence](#restful-api-client-with-http-header-persistence)
* [Levels of integration in testing with NUnit](#levels-of-integration-in-testing-with-nunit)
   * [Why integration testing is important](#why-integration-testing-is-important)
   * [DI in tests with a `ServiceProvider` class](#di-in-tests-with-a-serviceprovider-class)
   * [Automatic transaction rollback with `IGlobalTransaction`](#automatic-transaction-rollback-with-iglobaltransaction)
   * [Automatic cleanup with `IDeleteNewRows`](#automatic-cleanup-with-ideletenewrows)
   * [Microservice integration tests with `TestServer`](#microservice-integration-tests-with-testserver)


The claim that REST APIs are stateless is counteracted with almost any
implementation, be it authentication state in a JWT header or implicit
application state written as entities to a database. The [SMC Telephone
Demo](http://smc.sourceforge.net/TelephoneFSM.htm) in fact closely resembles the
stateful callback API provided by commercial telephone service companies for
implementing automated call services. 

## SMC API service with database persistence

The `apiservice.core` Web API implements a much simpler finite state automaton.
The user sends an SMS verification request, receives the access code via another
channel (`SMSServiceMock` writes only to the console) and has three attempts to
enter that code:, which is then saved to the database together with the phone
number.

![state diagram of the service](./img/Accesscode_sm.png)

This stateful REST API requires persistence, in case of `apiservice.core` in the
database, in case of `apicaller.core` only in a server side session variable:

![apicaller apiservice sequence](apicaller-apiservice.png)

To achieve persistence is astonishingly simple, it suffices to just inherit from
`PersistentController`. The not-so-intuitive assignment to the private
`_serviceClientCookies` field for reading it in *another* follow up request is
commented in the code:

```csharp
[Route("api/[controller]")]
[ApiController]
public class CallController : PersistentController
{
    internal string[] _serviceClientCookies;

    [HttpPost("authenticate")]
    public async Task<ActionResult<string>> Authenticate([FromBody] string phonenumber)
    {
        var authenticateResult = await _serviceClient.Authenticate(phonenumber);
        _serviceClientCookies = _serviceClient.Cookies; // save in this session
        return authenticateResult;
    }

    [HttpPost("verify")]
    public async Task<ActionResult<string>> Verify([FromBody] string accesscode)
    {
        _serviceClient.Cookies = _serviceClientCookies; // resrore from the session for service state persistence
        return await _serviceClient.Verify(accesscode);
    }
}
```

From the client's perspective, there is no difference whether the session is
stored in the database or not. The API above would be used exactly like the
`apiservice.core` API in this shortened `apicaller.Services.ServiceClient`:

```csharp
public class ServiceClient : IServiceClient
{
    internal IConfiguration _configuration;
    internal IHttpClientFactory _clientFactory;

    public string[] Cookies { get; set; }

    internal HttpClient GetHttpClient()
    {
        var client = _clientFactory.CreateClient("ServiceClient");
        AddDefaultHeaders(client);
        return client;
    }

    public async Task<ActionResult<string>> Authenticate(string phonenumber)
    {
        using (var client = GetHttpClient())
        {
            var request = new AuthenticateRequest() { Phonenumber = phonenumber };
            var response = await client.PostAsync(ResouceUri("authenticate"), Json.Serialize(request));
            if (response.StatusCode != HttpStatusCode.OK) return new StatusCodeResult((int)HttpStatusCode.ServiceUnavailable);
            this.Cookies = response.Headers.GetValues(SetCookie).ToArray();
            var result = Json.Deserialize<MessageResponseDto>(response.Content);
            return result.Message;
        }
    }

    public async Task<ActionResult<string>> Verify(string accesscode)
    {
        using (var client = GetHttpClient())
        {
            var request = new VerifyRequest() { Accesscode = accesscode };
            var response = await client.PostAsync(ResouceUri("verify"), Json.Serialize(request));
            if (response.StatusCode != HttpStatusCode.OK) return new StatusCodeResult((int)HttpStatusCode.ServiceUnavailable);
            var result = Json.Deserialize<MessageResponseDto>(response.Content);
            return result.Message;
        }
    }

    internal void AddDefaultHeaders(HttpClient client)
    {
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue(Application.Json));
        client.DefaultRequestHeaders.Add(UserAgent, ".NET HttpClient");
        foreach (var cookie in Cookies ?? Enumerable.Empty<string>())
        {
            client.DefaultRequestHeaders.Add(HeaderNames.Cookie, cookie);
        }
    }
}
```

The `Cookies` property is persisted in the `CallController` using the
`ServiceClient`. That way, arbitrary long chains of stateful API calls become
possible.


## RESTful API client with HTTP header persistence

While `Database` or `Session` persistence require server side state storage in
all frameworks, `ViewState` persistence does not and is therefore kinda "truly
statelss". This "stateless state persistence" pattern was an achievement at the
time of classic VBScript.asp which needed explicit implementation as shown in
the PHP example from [The State Machine Compiler](http://smc.sourceforge.net)
(in the `.\examples\Php\web` folder).

In the nowadays mostly abandoned ASP.NET WebForms framework, the encrypted
ViewState for "stateless state persistence" came virtually for free, but was
mostly neglected by most architects and developers in favor of the then epidemic
PostBack-`Response.Redirect` pattern.

ViewState is implemented by using a hidden form input element for POST requests.
This conflicts with the pluralism of HTTP verbs for RESTful API requests and the
non-standardized body payload, thus nowadays the payload of a signed JWT Web
Token in the HTTP request header is sometimes used to securely make a nominally
stateless service stateful. But that shamefacedly hidden orthogonal other *state
transfer* within the REST "state transfer" paradigm doesn't need to be readable
by the client, simply encrypting it symmetrically instead of signing the
(client-readable) JWT state suffices.

The API-caller in the test examples must persist the cookie received from the
SMC service, but in the custom `X-ViewState` header instead of the ViewState input element
like in WebForms or  MVC Core:

`.\apitest.core\apicaller\appsettings.json`:

```json
{
  "SessionStorage": "Header",
  "EncryptViewStateKey": "<-- secret w/o server affinity -->"
}
```

That encrypted and thus non-manipulable header must be carried over to the next
request sent by the same client using the static
`StorageImplementation.SetViewStateHeader` method:

`apitest.apicaller.ServerTest`:

```csharp
using (var client = GetHttpClient())
{
    var responseAuth = client.PostAsync("/api/call/authenticate", Json.Serialize(DbTestData.PHONENUMBER)).Result;
    StorageImplementation.SetViewStateHeader(responseAuth, client);
    var resultAuth = Json.Deserialize<string>(responseAuth.Content);
    Assert.That(resultAuth, Does.StartWith("Sent an SMS"));
    Assert.That(resultAuth, Does.Contain(DbTestData.PHONENUMBER));

    for (int i = 0; i < 3; i++)
    {
        var responseWrong = client.PostAsync("/api/call/verify", Json.Serialize("wrong code")).Result;
        StorageImplementation.SetViewStateHeader(responseWrong, client);
        var resultWrong = Json.Deserialize<string>(responseWrong.Content);
        Assert.That(resultWrong, Does.StartWith("Wrong access code"));
    }

    var responseDenied = client.PostAsync("/api/call/verify", Json.Serialize("wrong code")).Result;
    StorageImplementation.SetViewStateHeader(responseDenied, client);
    var resultDeniedh = Json.Deserialize<string>(responseDenied.Content);
    Assert.That(resultDeniedh, Does.StartWith("Access denied"));
}
```

Note the orthogonality of the `response.Content` payload and the header payload in
the `SetViewStateHeader` implementation:

```csharp
/// <summary>
/// Add/Replace the X-ViewState default header on the client with the
/// new state received from the last response for the next request.
/// </summary>
/// <param name="response"></param>
/// <param name="client"></param>
public static void SetViewStateHeader(HttpResponseMessage response, HttpClient client)
{
    var viewState = response.Headers.GetValues(StorageImplementation.HeaderName).ToList();
    client.DefaultRequestHeaders.Remove(StorageImplementation.HeaderName);
    client.DefaultRequestHeaders.Add(StorageImplementation.HeaderName, viewState[0]);
}
```

The *size* of that header state representation is - unlike the virtually
unlimited size of form input elements in the request/response payload stream -
limited to a few kilobytes by the web server and competes with the other headers
(particularly the JWT token for authentication) for that limited space.


## Levels of integration in testing with NUnit

### Why integration testing is important

"A unit test that talks to a database is not a unit test." (Michael Feathers).
By following that rule, people mock up everything except the narrow
functionality under test (which ends up to be trivial outside special
mathematically/logically complex applications), realize that the expense/yield
ratio goes down the tubes and end up abandoning automated testing as unnecessary
overhead at all - the primary goal of modern DevOps environments after all is
enabling developers to fix errors where they occur: directly in production. I've
got to hear this more or less exactly so.

In my personal experience, with narrow unit testing, I'm testing what is
expected, which is perfect during the initial development stage - but later on,
things usually break at *unexpected* places due to things like implicit
assumptions outside consciously made design contracts. Especially within the
IoC/DI paradigm encouraged by the .NET Core framework, mocking up all
dependencies quickly becomes very expensive, as this is relatively cheap only if
the functionally provided by them is either trivial or not needed by the class
under test (usually just persistence). And, most importantly: A test execution
greenscreen then tendentiously means: "The mocked up engine works perfectly,
no idea about the real engine."

Another observation while building a complex freelance contract accounting
application has been the fact that I've built a GUI tailored specifically to
quickly and efficiently generate complex data records, and manually entering the
same data line by line, number by number in the C# GUI not just felt, but
actually *was* incredibly tedious and inefficient.

So in this .NET Core API project, I've decided to embrace an inversion: All
tests are functional integration tests by default by using the same .NET Core DI
also in the NUnit test classes - unless the real objects are specifically
instantiated with mocked up ones.

This has the well known disadvantage that one single defect tends to immediately
repaint the whole test explorer tree thoroughly in spotted red (the "burning
Christmas tree pattern"), and the root cause of the problem can impossibly be
deduced from the pattern of the red dots. At the end, the time saved upfront
might be spent downstream by applying the infamous "checkout/compile/test"
binary search pattern to find the particular single commit that broke things.
This encourages the "commit each and every change separately" mentality, which
can be dizzying in an environment where commits are expected to mirror the
micromanaged "strictly-incremental-and-not-iterative" development tasks assigned
to the developers.


### DI in tests with a `ServiceProvider` class

The `apitest.core` project covers both the `apiservoce.core` and
`apicaller.core` Web API projects, each with its own DI dependency tree.
Correspondingly, the `apitest.core/apicaller` and `apitest.core/apiservice`
sub-folders each contain a static `ServiceProvider` class which reads both the
main test-project's `appsettings.json` configuration file and combines it with
the specific `appsettings.json` in that sub-folder. The idempotent
`CreateMembers()` method instantiates the *productive* `Startup` class of the
project under test and calls its *productive* `ConfigureServices` method to make
accessible the .NET Core DI engine to the test classes. Here an excerpt:

```csharp
_configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile(
        Path.Combine("apicaller", "appsettings.json"),
            optional: false, reloadOnChange: true)
    .Build();
var startup = new Startup(_configuration);
var sc = new ServiceCollection();
startup.ConfigureServices(sc);
_provider = sc.BuildServiceProvider();
```

Any test class inherits from the class under test and retrieves its dependencies
once from the DI instance of the `ServiceProvider`, here from
`apitest.apiservice.Controllers.AccesscodeControllerTest` (more on
`RetrieveDbContexts()` in the next chapter):

```csharp
[OneTimeSetUp]
public void ConfigureServices()
{
    _configuration = ServiceProvider.Configuration;
    _DbContext = ServiceProvider.Get<AspserviceDbContext>();
    _SMSService = ServiceProvider.Get<ISMSService>();
    this.RetrieveDbContexts();
}
```

This requires the dependency members to be declared at least as `internal`
togehter with an `[assembly: InternalsVisibleTo("apitest.core")]` in the
AssemblyInfo.cs of the project under test (instead of the usual `readonly` as
in all textbooks on C# DI). 

The infamous dependencies of the dependencies are automatically injected that
way - and its precisely those tree of dependencies of dependencies which is
prohibitively expensive to instantiate manually without DI in place in each and
every test class - which in turn leads to the only allegedly compulsory "need to
mock up everything" paradigm which in turn leads to the widespread "tests are
too expensive" practice while still claiming to be "agile".


### Automatic transaction rollback with `IGlobalTransaction`

One - if not the main - reason to mock up anything that writes to the database
is excessive growth of tables by running the tests. Luckily, with Entity
Framework 6, Microsoft finally decided that there *might* still be use cases for
developers to manage transactions consciously. The `this.RetrieveDbContexts()`
extension method call in the `OneTimeSetup` above of a test class brought in by
`IGlobalTransaction` initializes the automatic transaction rollback which
eliminates that problem.

It recursively walks the dependencies three and collects all `DbContext` objects
found to make them accessible to these `SetUp`/`TearDown` test scaffolding to
definitely solve that problem at a relatively low level:

```csharp
[SetUp]
public void BeginTrans()
{
    this.BeginTransaction(IsolationLevel.ReadUncommitted);
}

[TearDown]
public void RollbackTrans()
{
    this.RollbackTransaction();
}
```

Thanks to the `ReadUncommitted` isolation level, assertions can be made on the
data actually written to the database. But being able to rollback of course
relies on the fact that the methods under test don't commit transactions,
therefore "relatively low level". Methods "higher" in the call graph inevitably
need to commit transactions initiated by themselves, ultimately if the caller
calls *another* service (read "the microservice architecture").


### Automatic cleanup with `IDeleteNewRows`

As long as you refrain from the nowadays common "strictly use GUIDs as identity
column" pattern<sup>1</sup>, you can use the `IDeleteNewRows` extension to clean up newly inserted
rows by test runs. Of course, this cannot revert UPDATE changes applied to
existing rows.

As the possibly affected tables and identity columns can't be statically
deduced, they need to be declared manually, as in this excerpt from the setup of
the `CallControllerTest` class:

```csharp
[OneTimeSetUp]
public void ConfigureServices()
{
    (...)
    this.SelectMaxId(ConnectionString, "Accesscode", "accesscodeid");
    this.SelectMaxId(ConnectionString, "Main", "mainid");
}
```

The corresponding `TearDown`  method then deletes any rows not present in that
tables when the test class was instantiated:

```csharp
        [TearDown]
        public void DeleteNewRows()
        {
            this.DeleteNewRows(ConnectionString);
        }
```

1. The usual reason given for strictly using GUIDs as identity *everywhere* (not
   just for keys intended for GET parameters) is quite good "security by
   obscurity", as in the modern SPA web world, users can and ultimately will
   manipulate (integer) identities received from your backend, be it "just for
   fun". But using non-sequential identity values inevitably leads to clustered
   index fragmentation and subsequent performance degradation without periodic
   *expensive* clustered index rebuilds. The `[Main]` table used here throughout
   for controller persistence avoids that problem by using separate identity/PK
   columns as shown below:

    ```sql
    CREATE TABLE [dbo].[Main](
	    [mainid] [bigint] IDENTITY(1,1) NOT NULL,
	    [session] [uniqueidentifier] DEFAULT NEWID() NOT NULL,
        (...)
    CONSTRAINT [PK_Main] PRIMARY KEY NONCLUSTERED
    (
	    [session] ASC
    )
    ```
    The `NEWSEQUENTIALID()` T-SQL function was introduced to avoid the clustered
    index fragmentation problem, but this comes at the price of drastically
    lowering the "security by obscurity" virtue of using GUIDs, according to the
    [docs](https://docs.microsoft.com/en-us/sql/t-sql/functions/newsequentialid-transact-sql?view=sql-server-2017):

    >If privacy is a concern, do not use this function. It is possible to guess
    >the value of the next generated GUID and, therefore, access data associated
    >with that GUID.

    *But*: By using controller persistence to emulate ASP.NET WebForms'
    ViewState in a "modern" SPA with cleartext (within the browser) JSON data
    transfer, integer IDs received from a client are just *indexes* to
    transparently persisted (and thanks to the encryption *unmanipulable*) data
    objects that were presented to the user in the interaction before, after all
    access authorization checks. These IDs just *happen* to be identically equal
    to the actual database identity. Such an ID should never be passed
    *directly* to an SQL statement and thereby allowing above "just for fun"
    de-facto SQL injection (despite using `SqlParameter`) to gain access to data
    not belonging to the user. Just manipulate the *object* appointed by the ID
    in memory and write its new state to the database. 

    End of the GUID-as-"security by obscurity" discussion.


### Microservice integration tests with `TestServer`

The same `CallControllerTest` class uses a `TestServer` received from the
`TestServerClientFactory`in the `asplib.Services` namespace by injecting that
`_serviceClient` (which incidentally avoids touching the local network loopback
interface, formally yielding a "unit test" in the wording, but of course not the
spirit of Michael Feathers). 

The above omitted lines in the `OneTimeSetup` scaffold this.
`ApiserviceServiceProvider` is an alias for the custom `ServiceProvider` class
of the *other* Web API service currently not under test, but used as a
dependency.

```csharp
[OneTimeSetUp]
public void ConfigureServices()
{
    this.configuration = ServiceProvider.Configuration;
    _serviceServer = CreateApiserviceServer(ApiserviceServiceProvider.Configuration);
    var clientFactory = new TestServerClientFactory(_serviceServer);
    _serviceClient = new ServiceClient(this.configuration, clientFactory);
    (...)
}

private TestServer CreateApiserviceServer(IConfiguration configuration)
{
    var builder = new WebHostBuilder()
        .UseEnvironment("Development")
        .UseConfiguration(configuration)
        .UseStartup<ApiserviceStartup>();
    return new TestServer(builder);
}
```

The injected `_serviceClient` instance is never called by any test method
directly, but indirectly as a side effect of calling top-level methods in the
`CallController` under test.