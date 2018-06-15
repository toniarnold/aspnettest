# Writing GUI tests

1. [```minimaltest.DefaultTest```: a view from the outside](#minimaltest.DefaultTest-a-view-from-the-outside)
2. [```minimaltest.WithRootTest```: don't hunt for web controls](#minimaltest.WithRootTest-dont-hunt-for-web-controls)


As stated in the motivational [README.md](../README.md), this project is all about "teaching 
the app to test itself". For demonstration purposes, the ```minimal```/```minimaltest```
pair contains ascending stages of entanglement between the test suite, the ASP.NET
infrastructure and the ```SHDocVw.InternetExplorer``` COM component.

The full ```asp```/```testie``` pair tries to showcase the full potential of
the presented test pattern on a not-so-trivial ASP.NET Web Application project
using SMC, [The State Machine Compiler](http://smc.sourceforge.net), which
in itself serves as an example for putting together a rich user control with predictable
and auto-documented behaviour.


## 1. ```minimaltest.DefaultTest```: a view from the outside

Except for the deliberately failing ```ClickControlThrowsTest()```, this
simple setup provides no real advantage over any test scripting engine 
besides the fact that anything is written in C#.

At least clickable elements are not hunt for by their coordinates within the 
window, but by their HTML ```id``` attribute, which therefore must be known in
advance and remain constant on layout changes. In ASP.NET web controls, the 
```ClientID``` is dynamically generated before rendering the page and thus 
will change when rearranging elements.

The sole introspection is implicitly performed by checking for HTTP status
200 OK after each click, but otherwise any result assertions must be performed
by analyzing the result HTML after a click on an element:

```csharp
[Test]
public void ClickWithrootTest()
{
    this.Navigate("/minimal/default.aspx");
    this.ClickID("withroot-link");
    Assert.That(this.Html(), Does.Contain("<h1>minimalist test setup with root</h1>"));
}
```

This test case first opens the default page of the minimalist test setup,
then clicks the first HTML link leading to the ```withroot.aspx``` page
and verifies its title.


## 2. ```minimaltest.WithRootTest```: don't hunt for web controls

The second stage already implements most of the promises of the motivation letter
by requiring the associated Web Forms resp. Web Controls to inherit from the
```IRootControl``` extension interface and explicitly setting the root control
of a potential control object composition tree as the root from which
navigation through the tree via reflection (with dot notation) will occur. This is
*the* minimal root page:

```csharp
public partial class withroot : System.Web.UI.Page, IRootControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        this.SetRoot();
    }
}
```

The ```SetRoot()``` method puts a static global reference to that very control,
therefore it cannot yet be garbage collected by the .NET runtime after the page
rendering has finished (until the next request), although specific page 
```Request```/```Response``` objects will not be available no more.

But the auto-generated ```ClientID``` properties are still present, and this is the
crucial point: E.g. the ```this.Click("submitButton")``` action takes the literal
member name (here directly within the root object) of the corresponding ASP.NET control:

```csharp
protected global::System.Web.UI.WebControls.Button submitButton;
```

The test runner gets a reference to it via reflection starting from the root control 
and queries that object instance for its ```ClientID``` to be used with ```SHDocVw.InternetExplorer```.


The other way round, assertions can query ASP.NET control instances *directly*,
without the need to tamper with brittle regular expressions to be used in assertions
as in the first case with ```Does.Contain``` string extraction.

The condensed example test case:

```csharp
[Test]
public void WriteContentTest()
{
    this.Navigate("/minimal/withroot.aspx");
    this.Write("contentTextBox", "a first content line");
    this.Click("submitButton");
    Assert.That(((BulletedList)this.GetControl("contentList")).Items.Count, Is.EqualTo(1));
    var firstItem = (ListItem)((BulletedList)this.GetControl("contentList")).Items[0];
    Assert.That(firstItem.Text, Is.EqualTo("a first content line"));
}
```

All assertions directly operate on the typed ```System.Web.UI.WebControl.BulletedList``` 
instance with its ```ListItem``` collection created by ASP.NET infrastructure
and directly query their properties.

A more sophisticated example from ```testie.asp.calculator.SessionGridViewTest``` involving a
 ```GridView``` control:

```csharp
private GridView GridView
{
    get { return (GridView)this.GetControl("sessionDumpGridView"); }
}

private GridViewRow SelectRowContainig(string substr)
{
    return (from GridViewRow r in this.GridView.Rows
            where (
                from TableCell c in r.Cells
                where c.FindControl("stackLabel") != null &&
                    ((Label)c.FindControl("stackLabel")).Text.Contains(substr)
                select c).FirstOrDefault() != null
```

This LINQ query navigates though the ```GridViewRow``` instances of the
rendered ```GridView``` and returns those rows where a ```Label```
instance in a cell contains a specific substring. Good lock with the attempt of achieving
that *reliably* with hand-crafted regular expressions on the result HTML of
a complex dashboard-like web application project with many GridViews...

>Some people, when confronted with a problem, think "I know,
>I'll use regular expressions." Now they have two problems.

...or alternatively parsing the generated HTML and navigating its DOM without
*concrete* knowledge of the HTML ids given by ASP.NET.
