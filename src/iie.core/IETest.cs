using asplib.Controllers;
using NUnit.Framework;

namespace iie
{
    /// <summary>
    /// Minimal base class for IE tests with a [OneTimeSetUp]/[OneTimeTearDown]
    /// pair for starting/stopping Internet Explorer
    /// </summary>
    public abstract class IETest : IIE
    {
        /// <summary>
        /// Start Internet Explorer
        /// </summary>
        [OneTimeSetUp]
        public virtual void OneTimeSetUpIE()
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
    }

    /// <summary>
    /// Minimal base class for IE tests with a [OneTimeSetUp]/[OneTimeTearDown]
    /// pair for starting/stopping Internet Explorer and a static Controller
    /// accessor.
    /// </summary>
    public abstract class IETest<C> : IETest
    {
        /// <summary>
        /// Typed accessor for the controller under test
        /// </summary>
        protected C Controller
        {
            get { return (C)StaticControllerExtension.GetController(); }
        }
    }
}