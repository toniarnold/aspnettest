using System;
using System.Collections.Generic;
using WebSharper;

namespace asplib.View
{
    [JavaScript]
    public class TagHelper
    {
        /// <summary>
        /// Generate a dot separated unique client ID for an element nested
        /// within a sub-application.
        /// </summary>
        /// <param name="idHierarchy">The identifier hierarchy as a string array.</param>
        /// <returns></returns>
        public static string Id(params string[] idHierarchy)
        {
            var nonEmpty = new List<string>();  // no WebSharper.JavaScript.Array for String.Join
            foreach (var id in idHierarchy)
            {
                if (!String.IsNullOrEmpty(id))
                {
                    nonEmpty.Add(id);
                }
            }

            return String.Join(".", nonEmpty);
        }
    }
}