using apiservice.View;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace apiservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public partial class AccesscodeController
    {
        [HttpGet("helo")]
        public async Task<ActionResult<string>> Helo()
        {
            return await Task.Run(() => new ActionResult<string>("ehlo"));
        }

        [HttpPost("authenticate")]
        public async Task<ActionResult<AuthenticateResponse>> Authenticate([FromBody] AuthenticateRequest query)
        {
            await Task.Run(() => this.Fsm.Authenticate(query.Phonenumber));
            var response = new AuthenticateResponse(this.State)
            {
                Phonenumber = _pnonenumber,
            };
            return new ActionResult<AuthenticateResponse>(response);
        }

        [HttpPost("verify")]
        public async Task<ActionResult<VerifyResponse>> Verify([FromBody] VerifyRequest query)
        {
            await Task.Run(() => this.Fsm.Verify(query.Accesscode));
            var response = new VerifyResponse(this.State)
            {
                Phonenumber = _pnonenumber,
            };
            return new ActionResult<VerifyResponse>(response);
        }
    }
}