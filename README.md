# aspnettest

## [Don't Develop GUI Tests, Teach Your App To Test Itself!](http://www.drdobbs.com/testing/dont-develop-gui-tests-teach-your-app-to/240168468)

*While* reading above article on Dr. Dobb's, I immediately knew: 
"This is it!" - even more so on the ASP.NET stack. Quote from  the article:

>	### GUI Test Cheating: Do It Yourself

> Basically, the premise for this development was:

> * What are the issues with GUI tests? Control detection and ease of development and maintenance.

> * How can we fix them? Well, let's avoid having to "find" the controls, and instead make them always available. Then, let's use our favorite language and environment to write the test code.

### In Code

The demo code is a direct port of my PHP example from [The State Machine Compiler](http://smc.sourceforge.net) 
(in the `.\examples\Php\web` folder).

The unit test on the left-hand side inherits from the `Calculator` app class,
directly calls the transition methods on the contained state machine and 
asserts its result states and the calculation result on the stack.

The GUI test on the right-hand side talks via COM to an internet explorer instance, 
writes into a `TextBox` and clicks an ASP.NET `Button`. Indirectly, these clicks call
the very same transition methods on the state machine in the embedded `Calculator` instance 
and - this is the salient point - assert the result states strongly typed directly 
on that instance, exactly as in the unit tests. This is possible because the 
test engine runs in the same  address space as the Visual Studio Development Server. 
Moreover, the same test engine also has access to the rendered HTML and asserts the 
text of the calculation result in there, too.

<table>
<tr><th>Unit Test</th><th>GUI Test</th></tr>
<tr><td><pre>
[Test]
public void SqrtTest()
{
	this._fsm.Enter("");
	Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
	this._fsm.Enter("49");
	Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
	var before = this.Stack.Count;
	this._fsm.Sqrt(this.Stack);
	Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
	Assert.That(this.Stack.Peek(), Is.EqualTo("7"));
	Assert.That(this.Stack.Count, Is.EqualTo(before));
}
</pre>/td><td><pre>
[Test]
public void SqrtTest()
{
	this.Navigate("/asp/default.aspx");
	this.Click("footer.enterButton");
	Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
	this.Write("enter.operandTextBox", "49");
	this.Click("footer.enterButton");
	Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
	var before = this.Stack.Count;
	this.Click("calculate.sqrtButton");
	Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
	Assert.That(this.Stack.Peek(), Is.EqualTo("7"));
	Assert.That(this.Stack.Count, Is.EqualTo(before));
	Assert.That(this.Html(), Does.Contain(" 7\n"));
}
</pre></td></tr>
</table>
		
To quote again:

>### How Does It Look and Feel?

>Since one video is worth a thousand pictures, check out this screencast recorded on a ~~Mac~~ win10 box running an initial GUI test suite:

![Tests running...][running]
[running]: file://C:\_\wwwroot\doc\running.gif "Tests running..."
