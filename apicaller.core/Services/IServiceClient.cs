using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace apicaller.Services
{
    public interface IServiceClient
    {
        public string[] Cookies { get; set; }   // persist cookies outside to retain DI

        public Task<ActionResult<string>> Authenticate(string phonenumber);

        public Task<ActionResult<string>> Verify(string accesscode);
    }
}