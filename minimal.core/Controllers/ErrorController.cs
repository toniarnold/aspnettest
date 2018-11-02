using asplib.Controllers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace minimal.Controllers
{
    public class ErrorController : ErrorControllerBase
    {
        public ErrorController(IConfigurationRoot config, ILogger<ErrorController> logger) : base(config, logger)
        {
        }
    }
}