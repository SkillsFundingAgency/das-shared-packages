using Moq;
using Moq.Protected;

namespace SFA.DAS.GovUK.Auth.UnitTests;

public static class MessageHandler
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