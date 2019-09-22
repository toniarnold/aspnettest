using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace apicaller.Services
{
    public interface IServiceClient
    {
        public Task<ActionResult<string>> Authenticate(string phonenumber);

        public Task<ActionResult<string>> Verify(string accesscode);
    }
}