using System;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Http.Logging;
using SFA.DAS.Http.MessageHandlers;
using SFA.DAS.Http.UnitTests.Fakes;
using SFA.DAS.Testing;

namespace SFA.DAS.Http.UnitTests.MessageHandlers
{
    [TestFixture]
    [Parallelizable]
    public class LoggingMessageHandlerTests : FluentTest<LoggingMessageHandlerTestsFixture>
    {
        [Test]
        public Task SendAsync_WhenSendingARequest_ThenShouldLog()
        {
            return TestAsync(
                f => f.SendAsync(),
                f => f.Logger.Verify(l => 
                    l.Log(
                        LogLevel.Information,
                        EventIds.SendingRequest,
                        It.IsAny<object>(),
                        null,
                        It.IsAny<Func<object, Exception, string>>()),
                    Times.Once));
        }

        [Test]
        public Task SendAsync_WhenReceivingAResponse_ThenShouldLog()
        {
            return TestAsync(
                f => f.SendAsync(),
                f => f.Logger.Verify(l => 
                        l.Log(
                            LogLevel.Information,
                            EventIds.ReceivedResponse,
                            It.IsAny<object>(),
                            null,
                            It.IsAny<Func<object, Exception, string>>()),
                    Times.Once));
        }

        [Test]
        public Task SendAsync_WhenSendingARequest_ThenShouldReturnResponse()
        {
            return TestAsync(f => f.SendAsync(), (f, r) => r.Should().Be(f.HttpResponseMessage));
        }
    }

    public class LoggingMessageHandlerTestsFixture
    {
        public Mock<ILogger<LoggingMessageHandler>> Logger { get; set; }
        public LoggingMessageHandler LoggingMessageHandler { get; set; }
        public HttpClient HttpClient { get; set; }
        public FakeHttpMessageHandler InnerMessageHandler { get; set; }
        public HttpResponseMessage HttpResponseMessage { get; set; }

        public LoggingMessageHandlerTestsFixture()
        {
            Logger = new Mock<ILogger<LoggingMessageHandler>>();
            HttpResponseMessage = new HttpResponseMessage();
            InnerMessageHandler = new FakeHttpMessageHandler { HttpResponseMessage = HttpResponseMessage };
            LoggingMessageHandler = new LoggingMessageHandler(Logger.Object) { InnerHandler = InnerMessageHandler };
            HttpClient = new HttpClient(LoggingMessageHandler);

            Logger.Setup(l => l.IsEnabled(LogLevel.Information)).Returns(true);
        }

        public Task<HttpResponseMessage> SendAsync()
        {
            return HttpClient.GetAsync("https://foo.bar");
        }
    }
}