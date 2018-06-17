using asplib.View;
using NUnit.Framework;

namespace iie
{
    /// <summary>
    /// Base class for IE tests with a [OneTimeSetUp] / [OneTimeTearDown] pair for Storage.Database
    /// Override SetUpStorage() to configure the storage for that specific test suite.
    /// Provides accessors for IStorageControl
    /// </summary>
    public abstract class StorageTest<M> : IETest
    where M : new()
    {
        /// <summary>
        /// The central access point made persistent across requests
        /// </summary>
        protected M Main
        {
            get { return this.MainControl.Main; }
        }

        protected IStorageControl<M> MainControl
        {
            get { return (IStorageControl<M>)ControlRootExtension.RootControl; }
        }

        /// <summary>
        /// Remember the last row in [Main] before the tests started
        /// </summary>
        [OneTimeSetUp]
        public void OneTimeSetUpDatabase()
        {
            this.StartUpDatabase();
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