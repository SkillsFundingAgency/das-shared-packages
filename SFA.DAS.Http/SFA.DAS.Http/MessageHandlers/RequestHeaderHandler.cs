using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Http.MessageHandlers
{
    public sealed class RequestHeaderHandler : DelegatingHandler
    {
        private readonly IGenerateRequestHeader _requestHeader;

        public RequestHeaderHandler(IGenerateRequestHeader requestHeader)
        {
            _requestHeader = requestHeader;
        }

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage message, CancellationToken cancellationToken)
        {
            var value = _requestHeader.Generate();

            message.Headers.Add(_requestHeader.Name, value);

            return await base.SendAsync(message, cancellationToken);
        }
    }
}