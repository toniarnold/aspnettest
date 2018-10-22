using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Linq;

namespace asplib.Common
{
    /// <summary>
    /// Bring back ASP.NET Cookie sub-keys
    /// </summary>
    public static class CookieSubkeyExtension
    {
        /// <summary>
        /// Returns an URL-encoded single string for the dictionary
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        public static string ToCookieString(this NameValueCollection dict)
        {
            var items = new List<string>();
            foreach (string key in dict.Keys)
            {
                items.Add(String.Join("=",
                   WebUtility.UrlEncode(key),
                   WebUtility.UrlEncode(dict[key])));
            }
            return String.Join("&", items);
        }

        public static NameValueCollection FromCookieString(this string cookieString)
        {
            NameValueCollection retval = null;
            if (!String.IsNullOrEmpty(cookieString))
            { 
                var items = from item in cookieString.Split("&")
                            select item.Split("=");
                retval = new NameValueCollection();
                foreach (var item in items)
                {
                    retval.Add(WebUtility.UrlDecode(item[0]),
                               WebUtility.UrlDecode(item[1]));
            }
            }
            return retval;
        }
    }
}
