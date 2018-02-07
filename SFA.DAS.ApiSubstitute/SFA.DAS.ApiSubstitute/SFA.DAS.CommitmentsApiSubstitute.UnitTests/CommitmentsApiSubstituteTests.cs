using NUnit.Framework;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using SFA.DAS.CommitmentsApiSubstitute.WebAPI;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;

namespace SFA.DAS.CommitmentsApiSubstitute.UnitTests
{
    [TestFixture]
    public class CommitmentsApiSubstituteTests
    {

        private string baseAddress;

        private CommitmentsApiMessageHandler apiMessageHandlers;

        [SetUp]
        public void SetUp()
        {
            baseAddress = "http://localhost:9008/";
            apiMessageHandlers = new CommitmentsApiMessageHandler(baseAddress);
        }

        [Test]
        public async Task CanUseDefaultResponse()
        {
            using (CommitmentsApi webApi = new CommitmentsApi(apiMessageHandlers))
            {
                using (HttpClient client = new HttpClient())
                {
                    var jsonresponse = await client.GetAsync(baseAddress + apiMessageHandlers.GetProviderApprenticeship);
                    var response = JsonConvert.DeserializeObject<Apprenticeship>(jsonresponse.Content.ReadAsStringAsync().Result);
                    Assert.AreEqual(45784125, response.Id);
                }
            }
        }

        [Test]
        public async Task CanOverrideDefaultResponseForGetProviderApprenticeship()
        {
            apiMessageHandlers.OverrideGetProviderApprenticeship(new Apprenticeship {Id = 32169854 });

            using (CommitmentsApi webApi = new CommitmentsApi(apiMessageHandlers))
            {
                using (HttpClient client = new HttpClient())
                {
                    var jsonresponse = await client.GetAsync(baseAddress + apiMessageHandlers.GetProviderApprenticeship);
                    var response = JsonConvert.DeserializeObject<Apprenticeship>(jsonresponse.Content.ReadAsStringAsync().Result);
                    Assert.AreEqual(32169854, response.Id);
                }
            }
        }
    }
}
