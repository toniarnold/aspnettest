﻿using NUnit.Framework;
using OpenQA.Selenium;
using System.Collections.Generic;
using System.Threading;

namespace iselenium
{
    /// <summary>
    /// Minimal base class for Selenium tests with a [OneTimeSetUp]/[OneTimeTearDown]
    /// pair for starting/stopping the browser
    /// </summary>
    /// <typeparam name="TWebDriver">Selenium WebDriver</typeparam>
    public abstract class SeleniumTest<TWebDriver> : ISeleniumBase
        where TWebDriver : IWebDriver, new()
    {
#pragma warning disable IDE1006 // Members in Selenium-generated C# code
        public IDictionary<string, object> vars { get; set; }
        public IJavaScriptExecutor js { get; set; }
        public IWebDriver driver { get; set; }
#pragma warning restore IDE1006

        /// <summary>
        /// Suppress [SetUp] DeleteAllCookies()
        /// </summary>
        public bool KeepCookies { get; set; } = true;   // no more needed with modern browsers (not IE)

        /// <summary>
        /// Start the browser
        /// </summary>
        [OneTimeSetUp]
        public virtual void OneTimeSetUpBrowser()
        {
            this.SetUpBrowser<TWebDriver>();
        }

        /// <summary>
        /// Stop the browser
        /// </summary>
        [OneTimeTearDown]
        public void OneTimeTearDownBrowser()
        {
            this.TearDownBrowser();
        }

        /// <summary>
        /// Ensures reproducibility
        /// </summary>
        [SetUp]
        public void DeleteAllCookies()
        {
            if (!KeepCookies)
            {
                Thread.Sleep(1000); // avoid eventual OpenQA.Selenium.NoSuchWindowException
                this.driver.Manage().Cookies.DeleteAllCookies();
            }
        }
    }
}