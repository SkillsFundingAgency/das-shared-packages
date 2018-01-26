using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using SFA.DAS.ApiSubstitute.WebAPI;
using SFA.DAS.ApiSubstitute.WebAPI.MessageHandlers;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.ApiSubstitute.UnitTests
{
    [TestClass]
    public class WebApiSubstituteTests
    {

        [TestMethod]
        public async Task CanUseWebApiSubstitute()
        {
            const string eventsApibaseAddress = "http://localhost:9000";
            var expected = new TestAccount(1);
            string route = eventsApibaseAddress + "/events";

            ApiMessageHandlers eventsapiMessageHandlers = new ApiMessageHandlers();
            eventsapiMessageHandlers.SetupGet(route, expected);

            using (WebApiSubstitute webApiSubstitute = new WebApiSubstitute(eventsApibaseAddress, eventsapiMessageHandlers))
            {
                using (HttpClient client = new HttpClient())
                {
                    var jsonresponse = await client.GetAsync(route);
                    var response = JsonConvert.DeserializeObject<TestAccount>(jsonresponse.Content.ReadAsStringAsync().Result);
                    Assert.AreEqual(1, response.Accountid);
                }
            }
        }

        [TestMethod]
        public async Task CanUseMultipleWebApiSubstitute()
        {
            const string eventsApibaseAddress = "http://localhost:9000";
            var expectedevents = new TestAccount(1);
            string eventsroute = eventsApibaseAddress + "/events";
            ApiMessageHandlers eventsapiMessageHandlers = new ApiMessageHandlers();
            eventsapiMessageHandlers.SetupGet(eventsroute, expectedevents);
            WebApiSubstitute eventswebApiSubstitute = new WebApiSubstitute(eventsApibaseAddress, eventsapiMessageHandlers);

            const string accountsApibaseAddress = "http://localhost:9001";
            var expectedaccounts = new TestAccount(11);
            string accountsroute = accountsApibaseAddress + "/accounts";
            ApiMessageHandlers accountsapiMessageHandlers = new ApiMessageHandlers();
            accountsapiMessageHandlers.SetupGet(accountsroute, expectedaccounts);
            WebApiSubstitute accountswebApiSubstitute = new WebApiSubstitute(accountsApibaseAddress, accountsapiMessageHandlers);

            using (HttpClient client = new HttpClient())
            {
                var jsonresponse = await client.GetAsync(eventsroute);
                var response = JsonConvert.DeserializeObject<TestAccount>(jsonresponse.Content.ReadAsStringAsync().Result);
                Assert.AreEqual(1, response.Accountid);
            }
            using (HttpClient client = new HttpClient())
            {
                var jsonresponse = await client.GetAsync(accountsroute);
                var response = JsonConvert.DeserializeObject<TestAccount>(jsonresponse.Content.ReadAsStringAsync().Result);
                Assert.AreEqual(11, response.Accountid);
            }
            eventswebApiSubstitute.Dispose();
            accountswebApiSubstitute.Dispose();
        }
    }
}
