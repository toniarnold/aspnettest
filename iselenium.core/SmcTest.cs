using asplib.Controllers;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.IE;

namespace iselenium
{
    /// <summary>
    /// Base class for Selenium tests with accessors for ISmcControl
    /// </summary>
    [TestFixture]
    public abstract class SmcTest<TWebDriver, TController, FSMContext, TState> : StorageTest<TController>
        where TWebDriver : IWebDriver, new()
        where TController : SmcController<FSMContext, TState>
        where FSMContext : statemap.FSMContext
        where TState : statemap.State
    {
        protected FSMContext Fsm
        {
            get { return this.Controller.Fsm; }
        }

        protected TState State
        {
            get { return this.Controller.State; }
        }
    }

    /// <summary>
    /// Base class for IE tests with accessors for ISmcControl
    /// </summary>
    [TestFixture]
    public abstract class SmcTest<C, F, S> : SmcTest<InternetExplorerDriver, C, F, S>
        where C : SmcController<F, S>
        where F : statemap.FSMContext
        where S : statemap.State
    {
    }
}