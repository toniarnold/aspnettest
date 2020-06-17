using asplib.Controllers;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.IE;
using System;

namespace iselenium
{
    /// <summary>
    /// Base class for Selenium SMC tests with a specific typed accessors, a
    /// [OneTimeSetUp] / [OneTimeTearDown] pair for starting/stopping the
    /// browser and a [OneTimeSetUp] / [OneTimeTearDown] pair for Storage.Database
    /// which cleans up newly added Main rows
    /// </summary>
    /// <typeparam name="TWebDriver">Selenium WebDriver</typeparam>
    /// <typeparam name="TController">the Controller under test</typeparam>
    /// <typeparam name="TFSMContext">SMC context class</typeparam>
    /// <typeparam name="TState">SMC state</typeparam>
    public abstract class SmcDbTest<TWebDriver, TController, TFSMContext, TState> : SeleniumDbTest<TWebDriver, TController>
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

    /// <summary>
    /// Base class for IE tests with accessors for ISmcControl
    /// </summary>
    [Obsolete("Replaced by SmcDbTest<InternetExplorerDriver, TController, TFSMContext, TState>")]
    [TestFixture]
    public abstract class SmcTest<TController, TFSMContext, TState> : SmcDbTest<InternetExplorerDriver, TController, TFSMContext, TState>, IIE

        where TController : SmcController<TFSMContext, TState>
        where TFSMContext : statemap.FSMContext
        where TState : statemap.State
    {
    }
}