using asplib.View;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.IE;
using System;

namespace iselenium
{
    /// <summary>
    /// Base class for Selenium tests with accessors for ISmcControl
    /// </summary>
    public abstract class SmcTest<TWebDriver, TMain, FSMContext, TState> : StorageTest<TWebDriver, TMain>, IDatabase, ISelenium
        where TWebDriver : IWebDriver, new()
        where TMain : new()
        where FSMContext : statemap.FSMContext
        where TState : statemap.State
    {
        protected FSMContext Fsm
        {
            get { return this.MainControl.Fsm; }
        }

        protected TState State
        {
            get { return this.MainControl.State; }
        }

        protected new ISmcControl<TMain, FSMContext, TState> MainControl
        {
            get { return (ISmcControl<TMain, FSMContext, TState>)ControlRootExtension.GetRoot(); }
        }
    }

    /// <summary>
    /// Base class for IE tests with accessors for ISmcControl
    /// </summary>
    [Obsolete("Replaced by SmcTest<InternetExplorerDriver, TMain, FSMContext, TState>")]
    [TestFixture]
    public abstract class SmcTest<TMain, FSMContext, TState> : SmcTest<InternetExplorerDriver, TMain, FSMContext, TState>, IIE
        where TMain : new()
        where FSMContext : statemap.FSMContext
        where TState : statemap.State
    {
    }
}