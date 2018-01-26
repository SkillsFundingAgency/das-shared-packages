using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApiSubstitute.WebAPI.MessageHandlers
{
    public abstract class DelegateHandler : DelegatingHandler
    {

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return await DoSendAsync(request);
        }

        protected abstract Task<HttpResponseMessage> DoSendAsync(HttpRequestMessage request);
    }
}