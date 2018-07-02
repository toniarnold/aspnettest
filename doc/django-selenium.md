# Comparison to Selenium/Python

* [Test-Driven Development with Python](#test-driven-development-with-python)
* [The Browser with Selenium](#the-browser-with-selenium)
* [Unit testing with the Django infrastructure](#unit-testing-with-the-django-infrastructure)
* [Functional testing with the Django infrastructure](#functional-testing-with-the-django-infrastructure)
* [Testing the Database](#testing-the-database)
* [Implicit and explicit waits](#implicit-and-explicit-waits)

*All Python listings reproduced under the CC license of O'Reilly*


## Test-Driven Development with Python

I've come across the the O'Reilly book "Test-Driven Development with Python"
by Harry J.W. Percival, available online at 
[Obey the Testing Goat!](https://www.obeythetestinggoat.com), via this
[Hacker News submission](https://news.ycombinator.com/item?id=17413127).

Its aim is nearly congruent to this aspnettest project, but everything is built
on the Diango/Python/Selenium stack. And contrary to aspnettest, it explains
everything step by step from the ground up and doesn't assume deep familiarity
with the benefits and shortcomings of TDD for web applications as a base for
explaining a very specific technical solution in terse detail.

This comparison will reveal that the integration level between test framework
and application under test is much deeper in aspnettest.


## The Browser with Selenium

Unlike aspnettest which requires a very specific web browser (Internet
Explorer) and a very specific web application server (Visual Studio IIS
Express), Selenium is much more agnostic with respect to browser/server
infrastructure. The book recommends Firefox and is built on the Django Web
framework.

Selenium drives the browser in a similar way as ```SHDocVw.InternetExplorer```
in aspnettest, in the first example in [chapter
1.1](https://www.obeythetestinggoat.com/book/chapter_01.html#_obey_the_testing_goat_do_nothing_until_you_have_a_test)
with an explicit instantiation within the test method:

```python
from selenium import webdriver

browser = webdriver.Firefox()
browser.get('http://localhost:8000')

assert 'Django' in browser.title
```

This corresponds roughly to the first aspnettest example in [Writing GUI
Tests](writing.md#minimaltestdefaulttest-a-view-from-the-outside), but here
"downgraded" and made up with an explicit URL and port using the
```NavigateURL``` method and a hard-coded port number:

```csharp
[Test]
public void ClickWithrootTest()
{
    this.NavigateURL("http://127.0.0.1:55269/minimal/");
    Assert.That(this.Html(), Does.Contain("<h1>minimalist test setup</h1>"));
}
``` 

## Unit testing with the Django infrastructure

In [chapter
3](https://www.obeythetestinggoat.com/book/chapter_unit_test_first_view.html),
the ```django.test.TestCase``` to inherit from in the Pyton test classes
resembles the aspnettest ```Navigate``` method, requiring and absolute path
instead of an URL as parameter, but it does only URL resolving. This is
required for unit testing the web page - something that is impossible in a
comparable way with aspnettest, as there is to my knowledge no way to
instantiate ```System.Web.UI.Page``` objects outside of IIS *and* making them
interpret and merge the ASP.NET markup in ```.aspx``` files into manually
created code-behind instances outside IIS. But the assertions in these Django
powered test cases simply use isomorphic string comparison methods as in 
above ```ClickWithrootTest()```:

```python
class HomePageTest(TestCase):

    def test_root_url_resolves_to_home_page_view(self):
        found = resolve('/')
        self.assertEqual(found.func, home_page)


    def test_home_page_returns_correct_html(self):
        request = HttpRequest()  
        response = home_page(request)  
        html = response.content.decode('utf8')  
        self.assertTrue(html.startswith('<html>'))  
        self.assertIn('<title>To-Do lists</title>', html)  
        self.assertTrue(html.endswith('</html>'))  
```


## Functional testing with the Django infrastructure

The functional tests in [chapter
4](https://www.obeythetestinggoat.com/book/chapter_philosophy_and_refactoring.html)
again use the explicit browser instance with an URL, but declares this as "very
much the pragmatic choice". This is the slightly shortened example:

```python
class NewVisitorTest(unittest.TestCase):

    def setUp(self):
        self.browser = webdriver.Firefox()

    def tearDown(self):
        self.browser.quit()

    def test_can_start_a_list_and_retrieve_it_later(self):
        # Edith has heard about a cool new online to-do app. She goes
        # to check out its homepage
        self.browser.get('http://localhost:8000')

        # She notices the page title and header mention to-do lists
        self.assertIn('To-Do', self.browser.title)
        header_text = self.browser.find_element_by_tag_name('h1').text  
        self.assertIn('To-Do', header_text)

        # She is invited to enter a to-do item straight away
        inputbox = self.browser.find_element_by_id('id_new_item')  
        self.assertEqual(
            inputbox.get_attribute('placeholder'),
            'Enter a to-do item'
        )

        # She types "Buy peacock feathers" into a text box (Edith's hobby
        # is tying fly-fishing lures)
        inputbox.send_keys('Buy peacock feathers')  

        # When she hits enter, the page updates, and now the page lists
        # "1: Buy peacock feathers" as an item in a to-do list table
        inputbox.send_keys(Keys.ENTER)  
        time.sleep(1)  

        table = self.browser.find_element_by_id('id_list_table')
        rows = table.find_elements_by_tag_name('tr')  
        self.assertTrue(
            any(row.text == '1: Buy peacock feathers' for row in rows)
        )
```

The corresponding functionality in aspnettest. Note that now the
```setUp```/```tearDown``` methods closely resemble the corresponding static
methods implemented in ```IEExtension``` - except the ```DocumentComplete```
handler which will be discussed later:

```csharp
public static class IEExtension
{
    /// <summary>
    /// [OneTimeSetUp]
    /// Start Internet Explorer and set up events
    /// </summary>
    /// <param name="inst"></param>
    public static void SetUpIE(this IIE inst)
    {
        Trace.Assert(ie == null, "Only one SHDocVw.InternetExplorer instance allowed");
        ie = new SHDocVw.InternetExplorer();
        ie.AddressBar = true;
        ie.Visible = true;
        ie.DocumentComplete += new SHDocVw.DWebBrowserEvents2_DocumentCompleteEventHandler(OnDocumentComplete);
    }

    /// <summary>
    /// [OneTimeTearDown]
    /// Quit Internet Explorer
    /// </summary>
    /// <param name="inst"></param>
    public static void TearDownIE(this IIE inst)
    {
        if (ie != null)
        {
            ie.Quit();
            ie = null;
        }
    }
}
```

The crucial "Well, let's avoid having to "find" the controls, and instead make
them always available." from the motivational [README.md](../README.md) via
```find_element_by_id``` corresponds directly to the ```GetHTMLElement```
method operating on the ```mshtml.Document``` class representing the DOM in
aspnettest:

```csharp
[Test]
public void ThrowRetrieveDumpTest()
{
    ...
    var linkToDump = this.GetHTMLElement(IEExtension.EXCEPTION_LINK_ID);
    var coredumpUrl = (string)linkToDump.getAttribute("href");
    Assert.That(coredumpUrl, Does.Contain("/withstorage.aspx?session="));
}
```

The corresponding method to
```inputbox.send_keys('Buy peacock feathers')``` is not even made public, but
are wrapped in the ```Write``` method:

```csharp
public static void Write(this IIE inst, string path, string text)
{
    var input = GetElement(inst, ControlRootExtension.GetRoot(), path);
    input.setAttribute("value", text);
}
```


## Testing the Database

The database test example in [chapter
5](https://www.obeythetestinggoat.com/book/chapter_post_and_database.html) is
nearly identical to the example used in
[```WithStorageTest```](writing.md#minimaltestwithstoragetest-flexible-persistency-for-a-model-object) - 
but there are some crucial differences.

One difference is a matter of architecture/design: The whole point of using the
SMC State Machine Compiler and is the idea of using the Single-page application
(SPA) pattern, but not in the modern sense of using JavaScript to dynamically
modify the single web page (with the entire state logic resting on the client),
but instead on the server side by using (potentially asynchronous) PostBack
requests. Therefore there are no Response.Redirects available for assertions.
This avoids the Post/Redirect/Get (PRG) pattern which would destroy the ViewState
on state changes caused by button clicks.

But the main difference is the fact that testing the database by performing
assertions on the ORM objects themselves is done in the unit tests, here the
example in [chapter
5.5](https://www.obeythetestinggoat.com/book/chapter_post_and_database.html#_the_django_orm_and_our_first_model):

```python
rom lists.models import Item
[...]

class ItemModelTest(TestCase):

    def test_saving_and_retrieving_items(self):
        first_item = Item()
        first_item.text = 'The first (ever) list item'
        first_item.save()

        second_item = Item()
        second_item.text = 'Item the second'
        second_item.save()

        saved_items = Item.objects.all()
        self.assertEqual(saved_items.count(), 2)

        first_saved_item = saved_items[0]
        second_saved_item = saved_items[1]
        self.assertEqual(first_saved_item.text, 'The first (ever) list item')
        self.assertEqual(second_saved_item.text, 'Item the second')
```

The [lists/tests.py](https://github.com/hjwp/book-example/blob/chapter_post_and_database/lists/tests.py) 
example in fact uses a mixture between functional and unit tests: The ```Item```
class in this example is the ORM object which operates out of the context of
the web application processing the POST request, thus reading from the database
in the context of the test framework:

```python
def test_can_save_a_POST_request(self):
    self.client.post('/', data={'item_text': 'A new list item'})

    self.assertEqual(Item.objects.count(), 1)
    new_item = Item.objects.first()
    self.assertEqual(new_item.text, 'A new list item')
```

In aspnettest, there is no difference between the test context and the web
application context, thus the written data can be read back by the web application
itself, but the assertions are nevertheless performed at the model level:


```csharp
[Test]
public void ClearStorageTest()
{
    // Local setup: Store a value into the database
    this.Navigate("/minimal/withstorage.aspx");
    this.Select("storageList", "Database", expectPostBack: true);
    this.Write("contentTextBox", "a stored content line");
    this.Click("submitButton");
    Assert.That(this.Main.Content, Has.Exactly(1).Items);
    Assert.That(this.Main.Content[0], Is.EqualTo("a stored content line"));

    // Confirm that the line persistent
    this.RestartIE();
    Assert.That(this.Main.Content, Has.Exactly(1).Items);
    Assert.That(this.Main.Content[0], Is.EqualTo("a stored content line"));

    // Method under test: explicitly clear the database storage
    this.ClearDatabaseStorage();

    // Confirm that the content is now empty
    this.RestartIE();
    Assert.That(this.Main.Content, Has.Exactly(0).Items);
}
``` 

The complete [source
code](https://github.com/hjwp/book-example/blob/chapter_database_layer_validation/functional_tests/test_simple_list_creation.py)
from the [Validation at the Database
Layer](https://www.obeythetestinggoat.com/book/chapter_database_layer_validation.html)
chapter 13 uses a very similar pattern, but contrary to its title, the validation
in the sense of the assertions themselves are performed on the view layer by 

```page_text = self.browser.find_element_by_tag_name('body').text``` :

```python
def test_multiple_users_can_start_lists_at_different_urls(self):
    
    (...)

    ## We use a new browser session to make sure that no information
    ## of Edith's is coming through from cookies etc
    self.browser.quit()
    self.browser = webdriver.Firefox()

    # Francis visits the home page.  There is no sign of Edith's
    # list
    self.browser.get(self.live_server_url)
    page_text = self.browser.find_element_by_tag_name('body').text
    self.assertNotIn('Buy peacock feathers', page_text)
    self.assertNotIn('make a fly', page_text)

    # Francis starts a new list by entering a new item. He
    # is less interesting than Edith...
    inputbox = self.browser.find_element_by_id('id_new_item')
    inputbox.send_keys('Buy milk')
    inputbox.send_keys(Keys.ENTER)
    self.wait_for_row_in_list_table('1: Buy milk')

    (...)
```


## Implicit and explicit waits

[Chapter
6.3](https://www.obeythetestinggoat.com/book/chapter_explicit_waits_1.html#_on_implicit_and_explicit_waits_and_voodoo_time_sleeps)
discusses the problems addressed by the ```DocumentComplete``` handler in
aspnettest. Of course, aspnettest using ```SHDocVw.InternetExplorer``` suffers
from similar problems as Selenium, cited from the article: "Selenium does
theoretically do some "implicit" waits, but the implementation varies between
browsers, and at the time of writing was highly unreliable in the Selenium 3
Firefox driver. "Explicit is better than implicit", as the Zen of Python says,
so prefer explicit waits." And "explicit waits" in that context just 
means ```time.sleep()```.

While ```SHDocVw.InternetExplorer``` with the ```DocumentComplete```
handler may be less "flakey" than ```implicitly_wait``` in Selenium, it nevertheless
is unreliable in context of client side JavaScript issued the ASP.NET framework
itself. The complete signature of the Click() method demonstrates how expicit
and implicit waits in Selenium terminology are collapsed into one method:

```csharp
/// <summary>
/// Click the ASP.NET control element (usually a Button instance) at the given path and wait for the response
/// when expectPostBack is true.
/// </summary>
/// <param name="inst"></param>
/// <param name="path">Member name path to the control starting at the main control</param>
/// <param name="expectPostBack">Whether to expect a server request from the click</param>
/// <param name="expectedStatusCode">Expected StatusCofe of the response</param>
/// <param name="delay">Optional delay time in milliseconds before clicking the element</param>
/// <param name="pause">Optional pause time in milliseconds after IE claims DocumentComplete</param>
public static void Click(this IIE inst, string path, bool expectPostBack = true, int expectedStatusCode = 200, int delay = 0, int pause = 0)
{
    var button = GetElement(inst, ControlRootExtension.GetRoot(), path);
    Click(button, expectPostBack, expectedStatusCode, delay, pause);
}
```

The bool ```expectPostBack``` controls whether the ```AutoResetEvent```
semaphore is used for asynchronously waiting for a DocumentComplete signal from
Internet Explorer, thus perform a truly "implicit wait". The ```pause```
argument mirrors precisely what is called "explicit wait" in the book. Here's
the real implementation of ```Click()``` called by above public API method:

```csharp
private static void Click(IHTMLElement element, bool expectPostBack = true, int expectedStatusCode = 200, int delay = 0, int pause = 0)
{
    Thread.Sleep(delay);
    element.click();
    if (expectPostBack)
    {
        are.WaitOne(millisecondsTimeout);
    }
    Thread.Sleep(pause);
    Assert.That(IEExtension.StatusCode, Is.EqualTo(expectedStatusCode));
}
```

And here's a (shortened) example of flakiness on the ASP.NET stack where it is
required to fall back to the "explicit wait" pattern:

```csharp
[Test]
public void InsertOrphaneDeleteTest()
{
    // Initialize (eventually with a new cookie/row)
    this.Navigate("/asp/default.aspx");
    this.rowCountBefore = this.GridView.Rows.Count;

    (...)

    // At last click on the delete button to delete the dump row
    this.Click("hamburgerDiv", expectPostBack: false);
    row = this.SelectRowContainig(unique);
    var delete = row.FindControl("deleteLinkButton");
    // Partial PostBack does not trigger DocumentComplete,
    // as a fall back just wait long enough for the row to disappear
    this.Click(delete, expectPostBack: false, pause: 500);
    Assert.That(this.GridView.Rows.Count, Is.EqualTo(this.rowCountBefore));  // as in the beginning
}
```

While "explicit wait" is the exceptional case to the default "implicit wait" in
aspnettest, in Selenium the situation is inverted. Therefore, in 
[Capter 20](https://www.obeythetestinggoat.com/book/chapter_fixtures_and_wait_decorator.html),
a "generic explicit wait helper" is presented which encapsulates polling the
returned HTML for an assertion to become true:

```python
# The home page refreshes, and there is an error message saying
# that list items cannot be blank
self.wait_for(lambda: self.assertEqual(  
    self.browser.find_element_by_css_selector('.has-error').text,
    "You can't have an empty list item"
))
```

implemented as:

```python
def wait_for(self, fn):  
    start_time = time.time()
    while True:
        try:
            table = self.browser.find_element_by_id('id_list_table')  
            rows = table.find_elements_by_tag_name('tr')
            self.assertIn(row_text, [row.text for row in rows])
            return
        except (AssertionError, WebDriverException) as e:
            if time.time() - start_time > MAX_WAIT:
                raise e
            time.sleep(0.5)
```

While in aspnettest that functionality is not as necessary as with Selenium, it
could be added for handling complex applications which heavily rely on possibly
nested partial PostBacks *and* on assertions on that sub-controls instead of
the ```Main``` control/model class.
