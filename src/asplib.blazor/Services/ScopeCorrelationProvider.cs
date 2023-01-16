using Microsoft.Extensions.DependencyInjection;

namespace asplib.Services
{
    public class ScopeCorrelationProvider
    {
        private IServiceProvider _serviceProvider;

        public ScopeCorrelationProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Transfer the correlation Guid from the initial HTTP request scope to
        /// the SignalR request scope. To be called in OnInitializedAsync of the
        /// global App.razor.
        /// </summary>
        /// <param name="correlationGuidParam"></param>
        public void SetScope(Guid correlationGuidParam)
        {
            var signalRScope = _serviceProvider.GetRequiredService<ScopeCorrelation>();
            signalRScope.Guid = correlationGuidParam;
        }
    }
}