using NUnit.Framework;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using SFA.DAS.ProviderEventsApiSubstitute.WebAPI;
using SFA.DAS.Provider.Events.Api.Types;


namespace SFA.DAS.ApiSubstitute.UnitTests
{
    [TestFixture]
    public class ProviderEventsApiSubstituteTests
    {

        private string baseAddress;

        private ProviderEventsApiMessageHandler apiMessageHandlers;

        [SetUp]
        public void SetUp()
        {
            baseAddress = "http://localhost:9000/";
            apiMessageHandlers = new ProviderEventsApiMessageHandler(baseAddress);
        }
        
        [Test]
        public async Task CanUseDefaultResponse()
        {
            using (ProviderEventsApi webApi = new ProviderEventsApi(apiMessageHandlers))
            {
                using (HttpClient client = new HttpClient())
                {
                    var jsonresponse = await client.GetAsync(baseAddress + apiMessageHandlers.GetSubmissionEventsEndPoint);
                    var response = JsonConvert.DeserializeObject<PageOfResults<SubmissionEvent>>(jsonresponse.Content.ReadAsStringAsync().Result);
                    Assert.AreEqual(45785214, response.Items[0].ApprenticeshipId);
                }
            }
        }

        [Test]
        public async Task CanOverrideDefaultResponse()
        {
            apiMessageHandlers.OverrideGetSubmissionEvents(new PageOfResults<SubmissionEvent> { Items = new[] { new SubmissionEvent {ApprenticeshipId = 45785333 } }, PageNumber = 1, TotalNumberOfPages = 1 });

            using (ProviderEventsApi webApi = new ProviderEventsApi(apiMessageHandlers))
            {
                using (HttpClient client = new HttpClient())
                {
                    var jsonresponse = await client.GetAsync(baseAddress + apiMessageHandlers.GetSubmissionEventsEndPoint);
                    var response = JsonConvert.DeserializeObject<PageOfResults<SubmissionEvent>>(jsonresponse.Content.ReadAsStringAsync().Result);
                    Assert.AreEqual(45785333, response.Items[0].ApprenticeshipId);
                }
            }
        }
    }
}
