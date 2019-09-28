using System.IO.Pipelines;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

namespace asplib.Services
{
    /// <summary>
    /// Web API specific JSON serialization based on System.IO.Pipelines
    /// </summary>
    public static class Json
    {
        /// <summary>
        /// Return a JSON StringContent object for HttpClient.PostAsync()
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static StringContent Serialize(object obj)
        {
            var json = Serialize2JsonString(obj);
            return new StringContent(json, Encoding.UTF8, Application.Json);
        }

        /// <summary>
        /// Return an object from the HttpContent.ReadAsStreamAsync stream
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static T Deserialize<T>(HttpContent content)
        {
            var buffer = new Pipe().Writer.GetSpan();
            var stream = content.ReadAsStreamAsync().Result;
            stream.Read(buffer);
            return JsonSerializer.Deserialize<T>(buffer.Slice(0, (int)stream.Length),
                new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                });
        }

        /// <summary>
        /// Lightweight built-in alternative to AutoMapper's Map<T>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="src"></param>
        /// <returns></returns>
        public static T Map<T>(object src)
            where T : new()
        {
            var json = Serialize2JsonString(src);
            return DeserializeJsonString<T>(json);
        }

        internal static string Serialize2JsonString(object obj)
        {
            return JsonSerializer.Serialize(obj,
                    new JsonSerializerOptions()
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    }
                );
        }

        internal static T DeserializeJsonString<T>(string json)
        {
            var buffer = new Pipe().Writer.GetSpan();
            return JsonSerializer.Deserialize<T>(json,
                new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                });
        }
    }
}