using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace asplib.Model
{
    public static class Crypt
    {
        /// <summary>
        /// Key/IV pair vor storing in a cookie
        /// </summary>
        public struct Secret
        {
            public readonly byte[] Key, IV;
            public Secret(byte[] key, byte[] iv)
            {
                Key = key;
                IV = iv;
            }
        }

        public const int IV_LENGTH = 16;    // as CreateDecryptor() for getting the length already requires the IV


        private static AesCryptoServiceProvider GetAesProvider()
        {
            var aes = new AesCryptoServiceProvider();
            return aes;
        }

        /// <summary>
        /// Generate a new Key/IV pair
        /// </summary>
        /// <returns></returns>
        public static Secret NewSecret()
        {
            using (var aes = GetAesProvider())
            {
                aes.GenerateKey();
                aes.GenerateIV();
                return new Secret(aes.Key, aes.IV);
            }
        }

        /// <summary>
        /// Generate a new IV for an existing Key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Secret NewSecret(byte[] key)
        {
            using (var aes = GetAesProvider())
            {
                aes.GenerateIV();
                return new Secret(key, aes.IV);
            }
        }

        /// <summary>
        /// Encrypt the plain byte[] and prepends the IV
        /// </summary>
        /// <param name="secret"></param>
        /// <param name="plain"></param>
        /// <returns></returns>
        public static byte[] Encrypt(Secret secret, byte[] plain)
        {
            using (var aes = GetAesProvider())
            using (var encrypt = aes.CreateEncryptor(secret.Key, secret.IV))
            {
                return secret.IV.Concat(encrypt.TransformFinalBlock(plain, 0, plain.Length)).ToArray();
            }
        }

        /// <summary>
        /// Decrypt the cipher byte[] with the IV prefix
        /// </summary>
        /// <param name="secret"></param>
        /// <param name="cipher"></param>
        /// <returns></returns>
        public static byte[] Decrypt(Secret secret, byte[] cipher)
        {
            var iv = cipher.Take(IV_LENGTH).ToArray();
            using (var aes = GetAesProvider())
            using (var decrypt = aes.CreateDecryptor(secret.Key, iv))
            {
                return decrypt.TransformFinalBlock(cipher, IV_LENGTH, cipher.Length - IV_LENGTH);
            }
        }
    }
}
