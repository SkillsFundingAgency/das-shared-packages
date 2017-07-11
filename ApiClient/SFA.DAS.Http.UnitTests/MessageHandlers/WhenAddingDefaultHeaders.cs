using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Http.MessageHandlers;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.Http.UnitTests.MessageHandlers
{
    [TestFixture]
    public class WhenAddingDefaultHeaders
    {
        [Test]
        public async Task ThenAcceptHeaderShouldBeAdded()
        {
            var handler = new DefaultHeadersHandler { InnerHandler = new StubResponseHandler() };

            var client = new HttpClient(handler);

            var response = await client.GetAsync("http://localhost/test");

            response.RequestMessage.Headers.Accept.Any(x => x.MediaType == "application/json").Should().BeTrue();
        }

    }
}
