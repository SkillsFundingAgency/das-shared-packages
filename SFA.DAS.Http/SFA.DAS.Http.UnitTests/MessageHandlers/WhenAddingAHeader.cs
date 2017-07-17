using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;
using NUnit.Framework;

namespace SFA.DAS.Http.UnitTests.MessageHandlers
{
    [TestFixture]
    public class WhenAddingAHeader
    {
        [Test]
        public async Task ThenAcceptHeaderShouldBeAdded()
        {
            var client = new HttpClientBuilder().WithHeaders(new IFakeLogHeaderGenerator()).Build();
            var response = await client.GetAsync("http://localhost/test");

            response.RequestMessage.Headers
                .GetValues("HeaderName").All(m => m.Contains("log-value"))
                .Should().BeTrue();
        }
    }
}
