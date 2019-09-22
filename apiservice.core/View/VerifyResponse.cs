using System;
using static AccesscodeContext.AuthMap;

namespace apiservice.View
{
    public class VerifyResponse
    {
        private readonly AccesscodeContext.AccesscodeControllerState _state;

        public string State { get { return _state.Name; } }
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
                        return "The access code was wrong, retry";

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
        }
    }
}