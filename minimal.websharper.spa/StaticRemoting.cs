using System.Collections.Generic;
using System.Threading.Tasks;
using WebSharper;

namespace minimal.websharper.spa
{
    public static class StaticRemoting
    {
        // Static Model initially empty
        public static List<string> Content = new List<string>();

        /// <summary>
        /// Adds the specified content to the static model
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        [Remote]
        public static Task<List<string>> Add(string content)
        {
            Content.Add(content);
            return Task.FromResult(Content);
        }
    }
}