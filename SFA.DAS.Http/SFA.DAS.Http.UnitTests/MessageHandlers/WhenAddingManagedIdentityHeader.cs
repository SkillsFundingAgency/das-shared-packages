using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Http.MessageHandlers;
using SFA.DAS.Http.TokenGenerators;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.Http.UnitTests.MessageHandlers
{
    [TestFixture]
    public class WhenAddingManagedIdentityHeader
    {
        [Test]
        public async Task ThenBearerHeaderShouldBeAdded()
        {
            var mockTokenGenerator = new Mock<IManagedIdentityTokenGenerator>();
            mockTokenGenerator.Setup(x => x.Generate()).ReturnsAsync("TestTokenValue");

            var handler = new ManagedIdentityHeadersHandler(mockTokenGenerator.Object) { InnerHandler = new StubResponseHandler() };

            var client = new HttpClient(handler);

            var response = await client.GetAsync("http://localhost/test");

            response.RequestMessage.Headers.Authorization.Scheme.Should().Be("Bearer");
            response.RequestMessage.Headers.Authorization.Parameter.Should().Be("TestTokenValue");
        }
    }
}
