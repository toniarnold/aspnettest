using asplib.Model;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;
using System;

namespace asplib.Remoting
{
    /// <summary>
    /// Enables WebSharper remote methods to handle HTTP GET query strings.
    /// Stores the Request.Query in the current session to make it accessible to
    /// Ajax POST requests - therefore insert after .UseSession().
    /// On GET requests also deletes the SessionOnceKey(OnceAction) session
    /// variables used by StorageServer to enable F5 reloads of a stored page.
    /// Consecutive Ajax GET requests containing query strings will thus overwrite
    /// the original request from the browser URL and should therefore be omitted.
    /// </summary>
    public class RequestQuerySessionMiddleware
    {
        public const string SESSION_QUERY_KEY = "_SESSION_QUERY_KEY";

        protected readonly RequestDelegate _next;

        public RequestQuerySessionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Method == WebRequestMethods.Http.Get)
            {
                if (context.Request.Query.Count > 0)
                {
                    var query = new Dictionary<string, string>();
                    foreach (var key in context.Request.Query.Keys)
                    {
                        query.Add(key, context.Request.Query[key]);
                    }
                    context.Session.Set(SESSION_QUERY_KEY, Serialization.Serialize(query));
                    context.Session.Remove(SessionOnceKey(OnceAction.Clear));
                    context.Session.Remove(SessionOnceKey(OnceAction.Load));
                }
            }

            await _next(context);
        }

        /// <summary>
        /// Retrieve the stored Query object from the session.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>Original context.Request.Query as Dictionary if present, null otherwise</returns>
        public static Dictionary<string, string> Query(HttpContext context)
        {
            context.Session.TryGetValue(SESSION_QUERY_KEY, out byte[] bytes);
            return (bytes != null) ?
                    (Dictionary<string, string>)Serialization.Deserialize(bytes) :
                    null;
        }

        /// <summary>
        /// Gets the session keys for the respective action used in StorageServer.Once()
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static string SessionOnceKey(OnceAction action)
        {
            return String.Format("{0}_{1}_{2}", SESSION_QUERY_KEY, "ONCE", action);
        }

        public enum OnceAction
        {
            Load,
            Clear
        }
    }
}