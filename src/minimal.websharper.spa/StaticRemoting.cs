﻿using System.Collections.Generic;
using System.Threading.Tasks;
using WebSharper;

namespace minimal.websharper.spa
{
    public static class StaticRemoting
    {
        // Static reference to the Data
        public static List<string> Content;

        /// <summary>
        /// Makes the deserialized JSON content visible to NUnit
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        [Remote]
        public static Task Put(List<string> content)
        {
            Content = content;
            return Task.FromResult(true);
        }
    }
}