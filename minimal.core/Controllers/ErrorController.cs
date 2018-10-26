using asplib.Controllers;
using Microsoft.Extensions.Logging;

namespace minimal.Controllers
{
    public class ErrorController : ErrorControllerBase
    {
        public ErrorController(ILogger<ErrorController> logger) : base(logger)
        {
        }
    }
}