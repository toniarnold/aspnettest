using asplib.Model;
using NUnit.Framework;

namespace minimaltest.websharper.spa
{
    [TestFixture]
    public class WithStorageRemoteTest
    {
        [TearDown]
        public void ResetStorage()
        {
            StorageImplementation.SetStorage(null);
        }
    }
}