using System;
using NUnit.Framework;
using SFA.DAS.TokenServiceApiSubstitute.WebAPI;
using System.Threading.Tasks;
using System.Net.Http;
using SFA.DAS.TokenService.Api.Types;
using Newtonsoft.Json;

namespace SFA.DAS.TokenServiceApiSubstitute.UnitTests
{
    [TestFixture]
    public class TokenServiceApiUnitTests
    {

        private string baseAddress;

        private TokenServiceApiMessageHandler apiMessageHandlers;

        private TokenServiceApi webApi;

        [SetUp]
        public void SetUp()
        {
            baseAddress = "http://localhost:9007/";
            apiMessageHandlers = new TokenServiceApiMessageHandler(baseAddress);
            webApi = new TokenServiceApi(apiMessageHandlers);
        }

        [Test]
        public async Task CanUseDefaultResponse()
        {
            using (HttpClient client = new HttpClient())
            {
                var jsonresponse = await client.GetAsync(baseAddress + apiMessageHandlers.DefaultGetPrivilegedAccessTokenAsyncEndPoint);
                var response = JsonConvert.DeserializeObject<PrivilegedAccessToken>(jsonresponse.Content.ReadAsStringAsync().Result);
                Assert.AreEqual(DateTime.Now.AddDays(1).Date, response.ExpiryTime.Date);
            }
        }

        [Test]
        public async Task CanOverrideDefaultResponseForGetPrivilegedAccessTokenAsync()
        {
            apiMessageHandlers.OverrideGetPrivilegedAccessTokenAsync(new PrivilegedAccessToken { AccessCode = "32169854", ExpiryTime = DateTime.Now.AddDays(5) });

            using (HttpClient client = new HttpClient())
            {
                var jsonresponse = await client.GetAsync(baseAddress + apiMessageHandlers.DefaultGetPrivilegedAccessTokenAsyncEndPoint);
                var response = JsonConvert.DeserializeObject<PrivilegedAccessToken>(jsonresponse.Content.ReadAsStringAsync().Result);
                Assert.AreEqual(DateTime.Now.AddDays(5).Date, response.ExpiryTime.Date);
            }
        }

        [Test]
        public async Task CanSetupGetGetPrivilegedAccessTokenAsync()
        {
            
            apiMessageHandlers.SetupGetPrivilegedAccessTokenAsync(new PrivilegedAccessToken { AccessCode = "321", ExpiryTime = DateTime.Now.AddDays(15) });
            
            using (HttpClient client = new HttpClient())
            {
                var jsonresponse = await client.GetAsync(baseAddress + $"api/PrivilegedAccess");
                var response = JsonConvert.DeserializeObject<PrivilegedAccessToken>(jsonresponse.Content.ReadAsStringAsync().Result);
                Assert.AreEqual(DateTime.Now.AddDays(15).Date, response.ExpiryTime.Date);
            }
        }

        [TearDown]
        public void Teardown()
        {
            webApi?.Dispose();
        }
    }
}
