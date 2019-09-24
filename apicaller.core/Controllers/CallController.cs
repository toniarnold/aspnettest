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
        internal ServiceClient _serviceClient;

        public CallController(IConfiguration configuration) : base(configuration)
        {
        }

        [HttpGet("helo")]
        public async Task<ActionResult<string>> Helo()
        {
            return new ActionResult<string>("ehlo");
        }

        [HttpPost("authenticate")]
        public async Task<ActionResult<string>> Authenticate([FromBody] string phonenumber)
        {
            return await _serviceClient.Authenticate(phonenumber);
        }

        [HttpPost("verify")]
        public async Task<ActionResult<string>> Verify([FromBody] string accesscode)
        {
            return await _serviceClient.Verify(accesscode);
        }
    }
}