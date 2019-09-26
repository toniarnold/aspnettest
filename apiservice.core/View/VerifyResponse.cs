using System;
using static AccesscodeContext.AuthMap;

namespace apiservice.View
{
    public class VerifyResponse : IMessageResponse
    {
        private readonly AccesscodeContext.AccesscodeControllerState _state;

        public string State { get; set; }
        public string Phonenumber { get; set; }

        public string Message
        {
            get
            {
                switch (_state)
                {
                    case var s when s == Verified:
                        return $"The phone number {Phonenumber} is now verified";

                    case var s when s == Unverified:
                        return "Wrong access code, retry";

                    case var s when s == Denied:
                        return "Access denied";

                    default:
                        throw new NotImplementedException($"State: {State}");
                }
            }
        }

        public VerifyResponse(AccesscodeContext.AccesscodeControllerState state)
        {
            _state = state;
            State = _state.Name;
        }

        public VerifyResponse()
        { }
    }
}