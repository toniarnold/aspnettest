﻿using asplib.View;
using OpenQA.Selenium;

namespace iselenium
{
    /// <summary>
    /// Base class for Selenium SMC tests with specific typed accessors, a
    /// [OneTimeSetUp] / [OneTimeTearDown] pair for starting/stopping the
    /// browser
    /// </summary>
    /// <typeparam name="TWebDriver">Selenium WebDriver</typeparam>
    /// <typeparam name="TMain">the class Main under test</typeparam>
    /// <typeparam name="TFSMContext">SMC context class</typeparam>
    /// <typeparam name="TState">SMC state</typeparam>
    public abstract class SmcTest<TWebDriver, TMain, FSMContext, TState> : SeleniumTest<TWebDriver, TMain>, IDatabase, ISelenium
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
}