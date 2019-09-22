using System;
using System.Threading.Tasks;

namespace apiservice.Services
{
    /// <summary>
    /// SMS service stub just writing the secret to the console
    /// </summary>
    public class SMSService : ISMSService
    {
        public async Task Send(string phonenumber, string message)
        {
            await Task.Run(() => Console.WriteLine($"Access Code SMS: {message}"));
        }
    }
}