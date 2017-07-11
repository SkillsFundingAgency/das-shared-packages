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

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage message, CancellationToken cancellationToken)
        {
            var bearerToken = await _generator.Generate();

            message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

            return await base.SendAsync(message, cancellationToken);
        }
    }
}
