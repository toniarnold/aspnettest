using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace asplib.Model
{
    public static class Serialization
    {
        /// <summary>
        /// Serializes any object into a byte array and apply the crypto filter if given
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] Serialize(object obj, Func<byte[], byte[]> filter = null)
        {
            using (var stream = new MemoryStream())
            {
                var formattter = new BinaryFormatter();
                formattter.Serialize(stream, obj);
                return (filter == null) ? stream.ToArray() : filter(stream.ToArray());
            }
        }

        /// <summary>
        /// Deserializes a byte array into an object and apply the crypto
        /// filter if given Silently returns null if the deserialization fails
        /// for whatever reason (wrong key, old version...)
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        public static object Deserialize(byte[] bytes, Func<byte[], byte[]> filter = null)
        {
            using (var stream = new MemoryStream((filter == null) ? bytes : filter(bytes)))
            using (var writer = new BinaryWriter(stream))
            {
                var formattter = new BinaryFormatter();
                try
                {
                    return formattter.Deserialize(stream);
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }
    }
}