using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace asplib.Model
{
    /// <summary>
    /// Encapsulates the encryption of the [Main].[main] storage column
    /// </summary>
    public static class Crypt
    {
        /// <summary>
        /// Key/IV pair value type to minimize accidental proliferation of the key
        /// </summary>
        public struct Secret
        {
            public readonly byte[] Key, IV;

            internal Secret(byte[] key, byte[] iv)  // internal to avoid accidentally reusing the same IV
            {
                Key = key;
                IV = iv;
            }
        }

        public const int IV_LENGTH = 16;  // constant to avoid instantiating AesCryptoServiceProvider twice

        /// <summary>
        /// Generate a new Key/IV pair
        /// </summary>
        /// <returns></returns>
        public static Secret NewSecret()
        {
            using (var aes = new AesCryptoServiceProvider())
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
            using (var aes = new AesCryptoServiceProvider())
            {
                aes.GenerateIV();
                return new Secret(key, aes.IV);
            }
        }

        /// <summary>
        /// Convert a password string to a 32 Bytes/256 Bit AES-Key
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static byte[] Key(string password)
        {
            var bytes = Encoding.UTF8.GetBytes(password);
            using (var sha = SHA256.Create())
            {
                return sha.ComputeHash(bytes);
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
            using (var aes = new AesCryptoServiceProvider())
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
            using (var aes = new AesCryptoServiceProvider())
            using (var decrypt = aes.CreateDecryptor(secret.Key, iv))
            {
                return decrypt.TransformFinalBlock(cipher, IV_LENGTH, cipher.Length - IV_LENGTH);
            }
        }
    }
}