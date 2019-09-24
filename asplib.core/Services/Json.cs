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
        /// <param name="queryObject"></param>
        /// <returns></returns>
        public static StringContent Serialize<T>(T queryObject)
        {
            var json = JsonSerializer.Serialize(queryObject,
                    new JsonSerializerOptions()
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    }
                );
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
    }
}