using asplib;
using iie;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
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
