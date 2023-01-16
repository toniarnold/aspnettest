namespace asplib.Services
{
    /// <summary>
    /// Services a Guid to correlate the scope of the initial full HTTP
    /// request/response with the distinct scope of the HTTP request initiating
    /// the SignalR connection.
    /// This is the recommended pattern to "Pass tokens to a Blazor Server app" according to
    /// https://learn.microsoft.com/en-us/aspnet/core/blazor/security/server/additional-scenarios?view=aspnetcore-6.0#pass-tokens-to-a-blazor-server-app
    /// </summary>
    public class ScopeCorrelation
    {
        private Guid _guid = Guid.Empty;

        public Guid Guid
        {
            get
            {
                if (_guid == Guid.Empty)
                {
                    throw new Exception(
                        "No provider.SetScope(CorrelationGuid) in OnInitializedAsync() of App.razor");
                }
                return _guid;
            }
            set
            {
                if (value == Guid.Empty)
                {
                    throw new Exception(
                        "No param-CorrelationGuid=\"@Guid.NewGuid()\" in the App component of _Host.cshtml");
                }
                else if (_guid != Guid.Empty)
                {
                    throw new Exception("The correlation Guid is immutable");
                }
                _guid = value;
            }
        }
    }
}