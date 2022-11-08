namespace apiservice.View
{
    public class AuthenticateRequest
    {
        public AuthenticateRequest(string phonenumber)
        {
            Phonenumber = phonenumber;
        }

        public string Phonenumber { get; set; }
    }
}