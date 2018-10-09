using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using asplib.core.Controllers;

namespace iie
{
    /// <summary>
    /// Minimal base class for IE tests with a [OneTimeSetUp] / [OneTimeTearDown] pair
    /// for starting/stopping Internet Explorer.
    /// </summary>
    public abstract class IETest : IIE
    {
        /// <summary>
        /// Start Internet Explorer
        /// </summary>
        [OneTimeSetUp]
        public void OneTimeSetUpIE()
        {
            this.SetUpIE();
        }

        /// <summary>
        /// Stop Internet Explorer
        /// </summary>
        [OneTimeTearDown]
        public void OneTimeTearDownIE()
        {
            this.TearDownIE();
        }

        /// <summary>
        /// Get the global reference to the root control of an application under test within tests
        /// </summary>
        /// <returns></returns>
        protected T GetController<T>()
        {
            return (T)StaticControllerExtension.GetController();
        }
    }
}
