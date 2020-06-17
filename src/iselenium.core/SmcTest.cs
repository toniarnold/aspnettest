using asplib.Controllers;
using OpenQA.Selenium;

namespace iselenium
{
    /// <summary>
    /// Base class for Selenium SMC tests with a specific typed accessors and a
    /// [OneTimeSetUp] / [OneTimeTearDown] pair for starting/stopping the
    /// browser
    /// </summary>
    /// <typeparam name="TWebDriver">Selenium WebDriver</typeparam>
    /// <typeparam name="TController">the Controller under test</typeparam>
    /// <typeparam name="TFSMContext">SMC context class</typeparam>
    /// <typeparam name="TState">SMC state</typeparam>
    public abstract class SmcTest<TWebDriver, TController, TFSMContext, TState> : SeleniumDbTest<TWebDriver, TController>
        where TWebDriver : IWebDriver, new()
        where TController : SmcController<TFSMContext, TState>
        where TFSMContext : statemap.FSMContext
        where TState : statemap.State
    {
        protected TFSMContext Fsm
        {
            get { return this.Controller.Fsm; }
        }

        protected TState State
        {
            get { return this.Controller.State; }
        }
    }
}