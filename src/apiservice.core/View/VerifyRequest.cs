namespace apiservice.View
{
    public class VerifyRequest
    {
        public VerifyRequest(string accesscode)
        {
            Accesscode = accesscode;
        }

        public string Accesscode { get; set; }
    }
}