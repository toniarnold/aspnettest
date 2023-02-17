using asplib.View;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.IE;
using System;

namespace iselenium
{
    /// <summary>
    /// Base class for Selenium SMC tests with specific typed accessors, a
    /// [OneTimeSetUp] / [OneTimeTearDown] pair for starting/stopping the
    /// browser, a [OneTimeSetUp] / [OneTimeTearDown] pair for Storage.Database
    /// which cleans up newly added Main rows
    /// </summary>
    /// <typeparam name="TWebDriver">Selenium WebDriver</typeparam>
    /// <typeparam name="TMain">the class Main under test</typeparam>
    /// <typeparam name="TFSMContext">SMC context class</typeparam>
    /// <typeparam name="TState">SMC state</typeparam>
    public abstract class SmcDbTest<TWebDriver, TMain, FSMContext, TState> : SeleniumDbTest<TWebDriver, TMain>, IDatabase, ISelenium
        where TWebDriver : IWebDriver, new()
        where TMain : new()
        where FSMContext : statemap.FSMContext
        where TState : statemap.State
    {
        public FSMContext Fsm
        {
            get { return this.MainControl.Fsm; }
        }

        public TState State
        {
            get { return this.MainControl.State; }
        }

        public new ISmcControl<TMain, FSMContext, TState> MainControl
        {
            get { return (ISmcControl<TMain, FSMContext, TState>)ControlRootExtension.GetRoot(); }
        }
    }

    /// <summary>
    /// Base class for IE tests with accessors for ISmcControl
    /// </summary>
    [Obsolete("Replaced by SmcDbTest<InternetExplorerDriver, TMain, FSMContext, TState>")]
    [TestFixture]
    public abstract class SmcTest<TMain, FSMContext, TState> : SmcDbTest<InternetExplorerDriver, TMain, FSMContext, TState>, IIE
        where TMain : new()
        where FSMContext : statemap.FSMContext
        where TState : statemap.State
    {
    }
}