# REST API with SMC

The claim that REST APIs are stateless is counteracted with almost any
implementation, be it authentication state in a JWT header or implicit
application state written as entities to a database. The [SMC Telephone
Demo](http://smc.sourceforge.net/TelephoneFSM.htm) in fact closely resembles the
callback API provided by commercial telephone service companies for implementing
automated call services. 

The `apiservice.core` Web API implements a much simpler finite state automaton.
The user sends an SMS verification request, receives the access code via a third
channel (`SMSServiceMock` writes only to the console) and has three attempts to
enter that code:, which is then saved to the database together with the phone
number.

![state diagram of the service](./img/Accesscode_sm.png)

This stateful REST API requires persistence, in case of `apiservice.core` to the
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