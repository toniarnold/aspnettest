namespace apiservice.View
{
    public class AuthenticateResponse
    {
        private readonly AccesscodeContext.AccesscodeControllerState _state;

        public string State { get { return _state.Name; } }
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
        }
    }
}