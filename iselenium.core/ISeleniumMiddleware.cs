using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace iselenium
{
    public class ISeleniumMiddleware
    {
        protected readonly RequestDelegate _next;

        public ISeleniumMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            // Application_BeginRequest

            await _next(context);

            // Application_EndRequest
            SeleniumExtension.StatusCode = context.Response.StatusCode;
        }
    }
}