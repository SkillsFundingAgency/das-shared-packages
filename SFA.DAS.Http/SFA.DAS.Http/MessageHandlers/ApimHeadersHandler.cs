using SFA.DAS.Http.Configuration;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Http.MessageHandlers
{
    public sealed class ApimHeadersHandler : DelegatingHandler
    {
        private readonly IApimClientConfiguration _config;

        public ApimHeadersHandler(IApimClientConfiguration config)
        {
            _config = config;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Add("Ocp-Apim-Subscription-Key", _config.SubscriptionKey);
            request.Headers.Add("X-Version", _config.ApiVersion);

            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
