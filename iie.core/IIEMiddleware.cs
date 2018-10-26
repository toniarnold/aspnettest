using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace iie
{
    /// <summary>
    /// Global.asax equivalent
    /// </summary>
    public class IIEMiddleware
    {
        private readonly RequestDelegate _next;

        public IIEMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            // Application_BeginRequest

            await _next(context);

            // Application_EndRequest
            IEExtensionBase.SetStatusCode(context.Response.StatusCode);
        }
    }
}