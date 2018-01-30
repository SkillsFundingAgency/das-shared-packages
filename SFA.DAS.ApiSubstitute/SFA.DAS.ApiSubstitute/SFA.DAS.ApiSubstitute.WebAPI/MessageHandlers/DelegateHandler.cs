using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.ApiSubstitute.WebAPI.Extensions;

namespace SFA.DAS.ApiSubstitute.WebAPI.MessageHandlers
{
    public abstract class DelegateHandler : DelegatingHandler
    {
        public string BaseAddress;

        public DelegateHandler(string baseAddress)
        {
            BaseAddress = baseAddress.GetBaseUrl();
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return await DoSendAsync(request);
        }

        protected abstract Task<HttpResponseMessage> DoSendAsync(HttpRequestMessage request);
    }
}