using asplib.Model;
using NUnit.Framework;
using System.Linq;

namespace test.asplib.Model
{
    [TestFixture]
    public class CryptTest
    {
        [Test]
        public void NewSecretTest()
        {
            var secret = Crypt.NewSecret();
            Assert.That(secret, Is.Not.Null);
            Assert.That(secret.Key, Has.Exactly(32).Items); // default AES256
            Assert.That(secret.IV, Has.Exactly(Crypt.IV_LENGTH).Items);
        }

        [Test]
        public void EncryptDecryptTest()
        {
            var secret = Crypt.NewSecret();
            var plain = new byte[] { 1, 2, 3 };
            var cipher = Crypt.Encrypt(secret, plain);
            Assert.That(cipher.Count(), Is.GreaterThan(3));
            Assert.That(cipher, Is.Not.EquivalentTo(plain));
            var decrypted = Crypt.Decrypt(secret, cipher);
            Assert.That(decrypted, Is.EquivalentTo(plain));

            var secret2 = Crypt.NewSecret(secret.Key);
            var cipher2 = Crypt.Encrypt(secret2, plain);
            Assert.That(cipher2.Count(), Is.GreaterThan(3));
            Assert.That(cipher, Is.Not.EquivalentTo(plain));
            Assert.That(cipher2, Is.Not.EquivalentTo(cipher));
            var decrypted2 = Crypt.Decrypt(secret2, cipher2);
            Assert.That(decrypted2, Is.EquivalentTo(plain));
        }
    }
}