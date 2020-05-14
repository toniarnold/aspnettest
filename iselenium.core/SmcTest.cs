using asplib.Controllers;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.IE;
using System;

namespace iselenium
{
    /// <summary>
    /// Base class for Selenium tests with accessors for ISmcControl
    /// </summary>
    public abstract class SmcTest<TWebDriver, TController, TFSMContext, TState> : StorageTest<TWebDriver, TController>
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
    [Obsolete("Replaced by SmcTest<InternetExplorerDriver, TController, TFSMContext, TState>")]
    [TestFixture]
    public abstract class SmcTest<TController, TFSMContext, TState> : SmcTest<InternetExplorerDriver, TController, TFSMContext, TState>, IIE

        where TController : SmcController<TFSMContext, TState>
        where TFSMContext : statemap.FSMContext
        where TState : statemap.State
    {
    }
}