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
        /// <returns>the deserialized object as object</returns>
        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        public static object Deserialize(byte[] bytes, Func<byte[], byte[]> filter = null)
        {
            if (bytes == null)
            {
                return null;
            }
            using (var stream = new MemoryStream((filter == null) ? bytes : filter(bytes)))
            using (var writer = new BinaryWriter(stream))
            {
                var formattter = new BinaryFormatter();
                try
                {
                    return formattter.Deserialize(stream);
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Deserializes a byte array into an object and apply the crypto
        /// filter if given Silently returns null if the deserialization fails
        /// for whatever reason (wrong key, old version...)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bytes"></param>
        /// <param name="filter"></param>
        /// <returns>the deserialized object as T</returns>
        public static T Deserialize<T>(byte[] bytes, Func<byte[], byte[]> filter = null)
        {
            return (T)Deserialize(bytes, filter);
        }

        /// <summary>
        /// Nullable tolerant function composition: Compose two byte[] filters
        /// together and return the resulting function. If one of them is null
        /// (e.g. disabled by configuration), return the other one. If both are
        /// null, return null.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Func<byte[], byte[]> ComposeFilters(Func<byte[], byte[]> first, Func<byte[], byte[]> second)
        {
            if (first == null && second == null)
            {
                return x => x;
            }
            if (first == null)
            {
                return second;
            }
            else if (second == null)
            {
                return first;
            }
            else
            {
                return x => second(first(x));
            }
        }
    }
}