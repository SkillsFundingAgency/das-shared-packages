using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;

namespace SFA.DAS.OidcMiddleware.GovUk.UnitTests
{
    public class MessageHandler
    {
        public static Mock<HttpMessageHandler> SetupMessageHandlerMock(HttpResponseMessage response, Uri baseUrl, HttpMethod httpMethod)
        {
            var httpMessageHandler = new Mock<HttpMessageHandler>();
            httpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(c =>
                        c.RequestUri != null
                        && c.Method.Equals(httpMethod)
                        && c.RequestUri.Equals(baseUrl)
                    ),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync((HttpRequestMessage request, CancellationToken token) => response);
            return httpMessageHandler;
        }
    }
}
