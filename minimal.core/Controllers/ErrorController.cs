using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using asplib.Controllers;
using Microsoft.Extensions.Logging;

namespace minimal.Controllers
{
    public class ErrorController : ErrorControllerBase
    {

        public ErrorController(ILogger<ErrorController> logger) : base(logger) { }
    }
}
