using asplib.Model;
using System.IO.Pipelines;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

namespace asplib.Services
{
    /// <summary>
    /// Web API specific JSON serialization for HttpContent
    /// </summary>
    public static class JsonContent
    {
        /// <summary>
        /// Return a JSON StringContent object for HttpClient.PostAsync()
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static StringContent Serialize(object obj)
        {
            var json = Json.Serialize(obj);
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