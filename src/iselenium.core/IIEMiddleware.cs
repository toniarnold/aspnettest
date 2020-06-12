using Microsoft.AspNetCore.Http;
using System;

namespace iselenium
{
    [Obsolete("Replaced by ISeleniumMiddleware")]
    public class IIEMiddleware : ISeleniumMiddleware
    {
        public IIEMiddleware(RequestDelegate next) : base(next)
        {
        }
    }
}