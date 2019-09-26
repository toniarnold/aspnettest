namespace apiservice.View
{
    public class AuthenticateResponse : IMessageResponse
    {
        private readonly AccesscodeContext.AccesscodeControllerState _state;

        public string State { get; set; }
        public string Phonenumber { get; set; }

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