using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System.Threading.Tasks;

namespace iselenium
{
    // https://support.microsoft.com/en-us/help/234067/how-to-prevent-caching-in-internet-explorer
    // But even this does not stop IE from getting 304 responses and the test runner still requires two loads.

    public static class NoCacheMiddlewareExtension
    {
        /// <summary>
        /// Send no-cache and expires headers to avoid caching
        /// </summary>
        public static IApplicationBuilder UseNoCache(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<NoCacheMiddleware>();
        }
    }

    public class NoCacheMiddleware
    {
        protected readonly RequestDelegate _next;

        public NoCacheMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            context.Response.Headers[HeaderNames.CacheControl] = "no-cache";
            context.Response.Headers[HeaderNames.Pragma] = "no-cache";
            context.Response.Headers[HeaderNames.Expires] = "-1";
            await _next(context);
        }
    }
}