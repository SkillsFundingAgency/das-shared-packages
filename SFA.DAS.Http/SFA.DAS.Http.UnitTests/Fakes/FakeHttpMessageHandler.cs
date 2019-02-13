using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Http.UnitTests.Fakes
{
    //todo: rename from Fake, so it's not confused with ms fakes?
    public class FakeHttpMessageHandler : DelegatingHandler
    {
        public HttpResponseMessage HttpResponseMessage { get; set; }
        public HttpRequestMessage HttpRequestMessage { get; private set; }
        
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpRequestMessage = request;
            return Task.FromResult(HttpResponseMessage ?? throw new InvalidOperationException($"Value for {nameof(HttpResponseMessage)} cannot be null"));
        }
    }
}