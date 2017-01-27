using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Events.Api.Types;

namespace SFA.DAS.Events.Api.Client.UnitTests.EventsApiAccountTests
{
    [TestFixture]
    public class WhenICreateAnAccountEvent : EventsApiTestBase
    {
        [Test]
        public async Task ThenAccountEventIsCreated()
        {
            var url = $"{BaseUrl}api/events/accounts";

            var @event = new AccountEvent {  ResourceUri = "/api/accounts/ABC123", Event = "Test" };
            var expectedData = JsonConvert.SerializeObject(@event);

            await Api.CreateAccountEvent(@event);

            SecureHttpClient.Verify(x => x.PostAsync(url, expectedData, ClientToken), Times.Once);
        }
    }
}
