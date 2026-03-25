using System;
using System.Net.Http;
using System.Threading;
using Moq.Protected;

namespace SFA.DAS.Apim.Shared.UnitTests.Infrastructure.InternalApi
{
    public static class MessageHandler
    {
        public static Mock<HttpMessageHandler> SetupMessageHandlerMock(HttpResponseMessage response, string baseUrl, string httpMethod = "get")
        {
            var method = HttpMethod.Get;
            if (httpMethod.Equals("get", StringComparison.CurrentCultureIgnoreCase))
            {
                method = HttpMethod.Get;
            }
            else if (httpMethod.Equals("post", StringComparison.CurrentCultureIgnoreCase))
            {
                method = HttpMethod.Post;
            }
            else if (httpMethod.Equals("delete", StringComparison.CurrentCultureIgnoreCase))
            {
                method = HttpMethod.Delete;
            }
            else if (httpMethod.Equals("patch", StringComparison.CurrentCultureIgnoreCase))
            {
                method = HttpMethod.Patch;
            }
            else if (httpMethod.Equals("put", StringComparison.CurrentCultureIgnoreCase))
            {
                method = HttpMethod.Put;
            }

            var httpMessageHandler = new Mock<HttpMessageHandler>();
            httpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(c =>
                        c.Method.Equals(method)
                        && c.RequestUri.AbsoluteUri.Equals(baseUrl)),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync((HttpRequestMessage request, CancellationToken token) => response);
            return httpMessageHandler;
        }
    }
}