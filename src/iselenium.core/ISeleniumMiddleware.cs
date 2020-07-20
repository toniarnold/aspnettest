using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace iselenium
{
    public static class SeleniumMiddlewareExtension
    {
        /// <summary>
        /// Store the response StatusCode in leniumExtensionBase.StatusCode if it is
        /// not from /favicon.ico
        /// </summary>
        public static IApplicationBuilder UseISelenium(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ISeleniumMiddleware>();
        }
    }

    public class ISeleniumMiddleware
    {
        protected readonly RequestDelegate _next;

        public ISeleniumMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            // Application_BeginRequest in WebForms

            await _next(context);

            // Application_EndRequest in WebForms
            if (context.Request.Path != "/favicon.ico")
            {
                SeleniumExtensionBase.StatusCode = context.Response.StatusCode;
            }
        }
    }
}