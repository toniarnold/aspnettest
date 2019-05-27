using asplib.Model;
using iie;
using minimal.websharper.spa;
using NUnit.Framework;
using System.Collections.Generic;

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
