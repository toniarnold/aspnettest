using System.Text.Json;

namespace asplib.Model
{
    /// <summary>
    /// Json Serializer/Deserialiizer
    /// </summary>
    public static class Json
    {
        /// <summary>
        /// Lightweight built-in alternative to AutoMapper's Map<T>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="src"></param>
        /// <returns></returns>
        public static T? Map<T>(object src)
            where T : new()
        {
            var json = Serialize(src);
            return Deserialize<T>(json);
        }

        /// <summary>
        /// Serialize the object to a JSON string
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Serialize(object obj)
        {
            return JsonSerializer.Serialize(obj);
        }

        /// <summary>
        /// Deserialize the object T from a JSON string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T? Deserialize<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json);
        }
    }
}