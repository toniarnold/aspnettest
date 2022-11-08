namespace apiservice.View
{
    public class AuthenticateResponse : IMessageResponse
    {
        private readonly AccesscodeContext.AccesscodeControllerState _state = default!;

        public string State { get; set; } = default!;
        public string Phonenumber { get; set; } = default!;

        public string Message
        {
            get
            {
                return $"Sent an SMS with the access code to {Phonenumber}";
            }
        }

        public AuthenticateResponse(AccesscodeContext.AccesscodeControllerState state)
        {
            _state = state;
            State = _state.Name;
        }

        public AuthenticateResponse()
        { }
    }
}