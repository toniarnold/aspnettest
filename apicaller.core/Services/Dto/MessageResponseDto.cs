using apiservice.View;

namespace apicaller.Services.Dto
{
    public class MessageResponseDto : IMessageResponse
    {
        public string State { get; set; }
        public string Message { get; set; }
    }
}