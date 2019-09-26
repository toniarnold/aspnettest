namespace apiservice.View
{
    /// <summary>
    /// Response Dto interface for deserializing both AuthenticateResponse and
    /// MessageREsponse without computed properties.
    /// </summary>
    public interface IMessageResponse
    {
        public string State { get; }
        public string Message { get; }
    }
}