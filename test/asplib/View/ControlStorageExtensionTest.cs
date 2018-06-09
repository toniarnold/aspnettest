﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using asplib.View;

namespace test.asplib.View
{
    [TestFixture]
    public class ControlStorageExtensionTest
    {
        [Test]
        public void StorageUninitializedTest()
        {
            Assert.That(ControlStorageExtension.SessionStorage, Is.Null);
        }

        [Test]
        public void EncryptDatabaseStorageUninitializedTest()
        {
            Assert.That(ControlStorageExtension.EncryptDatabaseStorage, Is.Null);
        }
    }
}