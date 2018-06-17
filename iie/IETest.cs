using NUnit.Framework;

namespace iie
{
    /// <summary>
    /// Minimal abse class for IE tests with a [OneTimeSetUp] / [OneTimeTearDown] pair
    /// for starting/stopping Internet Explorer.
    /// </summary>
    public abstract class IETest : IIE
    {
        /// <summary>
        /// Start Internet Explorer
        /// </summary>
        [OneTimeSetUp]
        public void OneTimeSetUpIEp()
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
}