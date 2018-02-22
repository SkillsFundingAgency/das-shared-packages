using NUnit.Framework;
using SFA.DAS.EventsApiSubstitute.WebAPI;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

namespace SFA.DAS.EventsApiSubstitute.UnitTests
{
    [TestFixture]
    public class EventsApiUnitTests
    {
        private string baseAddress;

        private EventsApiMessageHandler apiMessageHandlers;

        [SetUp]
        public void SetUp()
        {
            baseAddress = "http://localhost:9004/";
            apiMessageHandlers = new EventsApiMessageHandler(baseAddress);
        }

        [Test]
        public async Task CanUseDefaultResponse()
        {
            using (EventsApi webApi = new EventsApi(apiMessageHandlers))
            {
                using (HttpClient client = new HttpClient())
                {
                    var jsonresponse = await client.GetAsync(baseAddress + apiMessageHandlers.DefaultCreateGenericEventEndPoint);
                    var response = JsonConvert.DeserializeObject<object>(jsonresponse.Content.ReadAsStringAsync().Result);
                    Assert.AreEqual(string.Empty, response);
                }
            }
        }

        [Test]
        public async Task CanOverrideCreateGenericEvent()
        {
            apiMessageHandlers.OverrideCreateGenericEvent("OverrideCreateGenericEvent");

            using (EventsApi webApi = new EventsApi(apiMessageHandlers))
            {
                using (HttpClient client = new HttpClient())
                {
                    var jsonresponse = await client.GetAsync(baseAddress + apiMessageHandlers.DefaultCreateGenericEventEndPoint);
                    var response = JsonConvert.DeserializeObject<object>(jsonresponse.Content.ReadAsStringAsync().Result);
                    Assert.AreEqual("OverrideCreateGenericEvent", response);
                }
            }
        }

        [Test]
        public async Task CanSetupCreateGenericEvent()
        {

            apiMessageHandlers.SetupCreateGenericEvent("SetupCreateGenericEvent");

            using (EventsApi webApi = new EventsApi(apiMessageHandlers))
            {
                using (HttpClient client = new HttpClient())
                {
                    var jsonresponse = await client.GetAsync(baseAddress + $"api/events/create");
                    var response = JsonConvert.DeserializeObject<object>(jsonresponse.Content.ReadAsStringAsync().Result);
                    Assert.AreEqual("SetupCreateGenericEvent", response);
                }
            }
        }
    }
}
