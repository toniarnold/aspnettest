using System.Threading.Tasks;

namespace apiservice.Services
{
    public interface ISMSService
    {
        public Task Send(string phonenumber, string message);
    }
}