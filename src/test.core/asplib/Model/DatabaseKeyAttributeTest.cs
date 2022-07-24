using asplib.Model;
using NUnit.Framework;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace test.asplib.Model
{
    public class DatabaseKeyAttributeTest
    {
        [Test]
        public void ReadWriteProtectedMemory()
        {
            var secret = new byte[] { 1, 2, 3 };
            AssertGetKey(secret);
        }

        private void AssertGetKey(byte[] secret)
        {
            var attr = new DatabaseKeyAttribute(secret);    // secret stored encrypted
            Assert.That(attr.Key, Is.EqualTo(secret));      // retrieved from encrypted memory
        }

        [Test]
        public void ConcurrentlyReadWriteProtectedMemory()
        {
            const int THREADS = 100;

            // 1. create THREADS byte arrays
            var sectrets = new List<byte[]>();
            using (var aes = Aes.Create())
            {
                for (int i = 0; i < THREADS; i++)
                {
                    aes.GenerateKey();
                    sectrets.Add(aes.Key);
                }
            }

            // 2. write/read them in parallel
            //    (might eventually detect thread safety problems with the static IDataProtector instance)
            var tasks = new List<Task>();
            foreach (var secret in sectrets)
            {
                tasks.Add(Task.Run(() => AssertGetKey(secret)));
            }
            Task.WaitAll(tasks.ToArray());
        }
    }
}