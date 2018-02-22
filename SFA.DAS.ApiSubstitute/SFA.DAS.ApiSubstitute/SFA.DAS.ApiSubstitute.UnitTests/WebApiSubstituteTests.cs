using Moq;
using NUnit.Framework;
using Newtonsoft.Json;
using SFA.DAS.ApiSubstitute.WebAPI;
using SFA.DAS.ApiSubstitute.WebAPI.MessageHandlers;
using System.Net.Http;
using System.Threading.Tasks;
using SFA.DAS.NLog.Logger;
using System.Web;

namespace SFA.DAS.ApiSubstitute.UnitTests
{
    [TestFixture]
    public class WebApiSubstituteTests
    {
        [Test]
        public void CanUseWebApiSubstituteWithNLogLogger()
        {
            const string eventsApibaseAddress = "http://localhost:9004";
            var expected = new TestAccount(1);
            string endPoint = "/events";
            string route = eventsApibaseAddress + endPoint;

            ApiMessageHandlers eventsapiMessageHandlers = new ApiMessageHandlers(eventsApibaseAddress, new NLogLogger(typeof(WebApiSubstituteTests), null, null));
            eventsapiMessageHandlers.SetupGet(endPoint, expected);

            using (WebApiSubstitute webApiSubstitute = new WebApiSubstitute(eventsapiMessageHandlers))
            {
                using (HttpClient client = new HttpClient())
                {
                    Assert.DoesNotThrowAsync(async () => await client.GetAsync(route));
                }
            }
        }
        
        [Test]
        public async Task CanUseWebApiSubstituteWithNLogLoggerForUnkownRequest()
        {
            const string eventsApibaseAddress = "http://localhost:9004";
            var expected = new TestAccount(1);
            string endPoint = "/events";
            string route = eventsApibaseAddress + endPoint;

            ApiMessageHandlers eventsapiMessageHandlers = new ApiMessageHandlers(eventsApibaseAddress, new NLogLogger(typeof(WebApiSubstituteTests), null, null));
            
            using (WebApiSubstitute webApiSubstitute = new WebApiSubstitute(eventsapiMessageHandlers))
            {
                using (HttpClient client = new HttpClient())
                {
                    var jsonresponse = await client.GetAsync(route);
                    Assert.Throws<HttpRequestException>(() => jsonresponse.EnsureSuccessStatusCode());
                    
                }
            }
        }

        [Test]
        public async Task WebApiSubstituteCanLogKnownRequest()
        {
            var logger = new Mock<ILog>();
            const string eventsApibaseAddress = "http://localhost:9004";
            var expected = new TestAccount(1);
            string endPoint = "/events";
            string route = eventsApibaseAddress + endPoint;

            ApiMessageHandlers eventsapiMessageHandlers = new ApiMessageHandlers(eventsApibaseAddress, logger.Object);
            eventsapiMessageHandlers.SetupGet(endPoint, expected);

            using (WebApiSubstitute webApiSubstitute = new WebApiSubstitute(eventsapiMessageHandlers))
            {
                using (HttpClient client = new HttpClient())
                {
                    var jsonresponse = await client.GetAsync(route);
                    var response = JsonConvert.DeserializeObject<TestAccount>(jsonresponse.Content.ReadAsStringAsync().Result);
                    logger.Verify(l => l.Info($"Response configured for {route}"), Times.Once);
                }
            }
        }

        [Test]
        public async Task WebApiSubstituteCanLogUnKnownRequest()
        {
            var logger = new Mock<ILog>();
            const string eventsApibaseAddress = "http://localhost:9004";
            var expected = new TestAccount(1);
            string endPoint = "/events";
            string route = eventsApibaseAddress + endPoint;

            ApiMessageHandlers eventsapiMessageHandlers = new ApiMessageHandlers(eventsApibaseAddress, logger.Object);
            
            using (WebApiSubstitute webApiSubstitute = new WebApiSubstitute(eventsapiMessageHandlers))
            {
                using (HttpClient client = new HttpClient())
                {
                    var jsonresponse = await client.GetAsync(route);
                    var response = JsonConvert.DeserializeObject<TestAccount>(jsonresponse.Content.ReadAsStringAsync().Result);
                    logger.Verify(l => l.Warn($"Response is not configured for {route}"), Times.Once);
                }
            }
        }

        [Test]
        public async Task CanUseWebApiSubstitute()
        {
            const string eventsApibaseAddress = "http://localhost:9004";
            var expected = new TestAccount(1);
            string endPoint = "/events";
            string route = eventsApibaseAddress + endPoint;

            ApiMessageHandlers eventsapiMessageHandlers = new ApiMessageHandlers(eventsApibaseAddress);
            eventsapiMessageHandlers.SetupGet(endPoint, expected);

            using (WebApiSubstitute webApiSubstitute = new WebApiSubstitute(eventsapiMessageHandlers))
            {
                using (HttpClient client = new HttpClient())
                {
                    var jsonresponse = await client.GetAsync(route);
                    var response = JsonConvert.DeserializeObject<TestAccount>(jsonresponse.Content.ReadAsStringAsync().Result);
                    Assert.AreEqual(1, response.Accountid);
                }
            }
        }

        [Test]
        public async Task CanUseMultipleWebApiSubstitute()
        {
            const string eventsApibaseAddress = "http://localhost:9000";
            var expectedevents = new TestAccount(1);
            string eventsendPoint = "/events";
            string eventsroute = eventsApibaseAddress + eventsendPoint;
            ApiMessageHandlers eventsapiMessageHandlers = new ApiMessageHandlers(eventsApibaseAddress);
            eventsapiMessageHandlers.SetupGet(eventsendPoint, expectedevents);
            WebApiSubstitute eventswebApiSubstitute = new WebApiSubstitute(eventsapiMessageHandlers);

            const string accountsApibaseAddress = "http://localhost:9001";
            var expectedaccounts = new TestAccount(11);
            string accountssendPoint = "/accounts";
            string accountsroute = accountsApibaseAddress + accountssendPoint;
            ApiMessageHandlers accountsapiMessageHandlers = new ApiMessageHandlers(accountsApibaseAddress);
            accountsapiMessageHandlers.SetupGet(accountssendPoint, expectedaccounts);
            WebApiSubstitute accountswebApiSubstitute = new WebApiSubstitute(accountsapiMessageHandlers);

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
