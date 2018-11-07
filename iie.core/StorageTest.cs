using NUnit.Framework;

namespace iie
{
    /// <summary>
    /// Base class for IE tests with a [OneTimeSetUp] / [OneTimeTearDown] pair
    /// for Storage.Database which cleans up newly added Main rows.
    /// Call SetUpStorage() to configure the storage for that specific test suite.
    /// Provides accessors for IStorageControl
    [TestFixture]
    public abstract class StorageTest<C> : IETest<C>
    {
        /// <summary>
        /// Remember the last row in [Main] before the tests started
        /// </summary>
        [OneTimeSetUp]
        public void OneTimeSetUpDatabase()
        {
            this.SetUpDatabase();
        }

        /// <summary>
        /// Delete any rows in [Main] that have been added since the test start
        /// </summary>
        [OneTimeTearDown]
        public void OneTimeTearDownDatabase()
        {
            this.TearDownDatabase();
        }
    }
}
