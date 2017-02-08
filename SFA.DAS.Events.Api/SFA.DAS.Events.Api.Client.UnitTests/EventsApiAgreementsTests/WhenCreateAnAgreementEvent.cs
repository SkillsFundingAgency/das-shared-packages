using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Events.Api.Types;

namespace SFA.DAS.Events.Api.Client.UnitTests.EventsApiAgreementsTests
{
    [TestFixture]
    public class WhenICreateAnAgreementEvent : EventsApiTestBase
    {
        [Test]
        public async Task ThenAgreementEventIsCreated()
        {
            var url = $"{BaseUrl}api/events/engagements";

            var @event = new AgreementEvent { ContractType = "MainProvider", Event = "Test", ProviderId = "ZZZ999" };
            var expectedData = JsonConvert.SerializeObject(@event);

            await Api.CreateAgreementEvent(@event);

            SecureHttpClient.Verify(x => x.PostAsync(url, expectedData, ClientToken), Times.Once);
        }
    }
}
