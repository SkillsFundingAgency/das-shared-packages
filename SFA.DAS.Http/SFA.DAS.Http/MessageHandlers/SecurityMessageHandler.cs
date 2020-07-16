using SFA.DAS.Http.TokenGenerators;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Http.MessageHandlers
{
    public sealed class SecurityMessageHandler : DelegatingHandler
    {
        private readonly IGenerateBearerToken _generator;

        public SecurityMessageHandler(IGenerateBearerToken generator)
        {
            _generator = generator;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var bearerToken = await _generator.Generate().ConfigureAwait(false);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
