using apicaller.Services;
using asplib.Controllers;
using asplib.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace apicaller.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Clsid("1541FD7B-0F77-4E2C-B61E-4A479D662E23")]
    public class CallController : SerializableController
    {
        internal IServiceClient _serviceClient;
        internal string[] _serviceClientCookies;

        public CallController(
            IConfiguration configuration,
            IServiceClient serviceClient) : base(configuration)
        {
            _serviceClient = serviceClient;
        }

        public CallController()
        {
        }

        [HttpGet("helo")]
        public async Task<ActionResult<string>> Helo()
        {
            return await Task.Run(() => new ActionResult<string>("ehlo"));
        }

        [HttpPost("authenticate")]
        public async Task<ActionResult<string>> Authenticate([FromBody] string phonenumber)
        {
            var authenticateResult = await _serviceClient.Authenticate(phonenumber);
            _serviceClientCookies = _serviceClient.Cookies; // save in this session
            return authenticateResult;
        }

        [HttpPost("verify")]
        public async Task<ActionResult<string>> Verify([FromBody] string accesscode)
        {
            _serviceClient.Cookies = _serviceClientCookies; // resrore from the session for service state persistence
            return await _serviceClient.Verify(accesscode);
        }
    }
}