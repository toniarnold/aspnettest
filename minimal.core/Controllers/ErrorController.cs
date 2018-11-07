using asplib.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace minimal.Controllers
{
    public class ErrorController : ErrorControllerBase
    {
        public ErrorController(IConfigurationRoot config, ILogger<ErrorController> logger) : base(config, logger)
        {
        }
    }
}