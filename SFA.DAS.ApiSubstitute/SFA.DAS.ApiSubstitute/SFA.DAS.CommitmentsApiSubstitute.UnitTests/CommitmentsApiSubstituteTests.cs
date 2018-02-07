using NUnit.Framework;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using SFA.DAS.CommitmentsApiSubstitute.WebAPI;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.ApiSubstitute.Utilities;

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
                    var jsonresponse = await client.GetAsync(baseAddress + apiMessageHandlers.DefaultGetProviderApprenticeshipEndPoint);
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
                    var jsonresponse = await client.GetAsync(baseAddress + apiMessageHandlers.DefaultGetProviderApprenticeshipEndPoint);
                    var response = JsonConvert.DeserializeObject<Apprenticeship>(jsonresponse.Content.ReadAsStringAsync().Result);
                    Assert.AreEqual(32169854, response.Id);
                }
            }
        }

        [Test]
        public async Task CanSetupGetProviderApprenticeship()
        {
            long ProviderId = 10002457;
            long ApprenticeshipId = 45784125;

            var apprenticeship = new ObjectCreator().Create<Apprenticeship>(x => { x.ProviderId = ProviderId; x.Id = ApprenticeshipId; });
            
            apiMessageHandlers.SetupGetProviderApprenticeship(ProviderId, ApprenticeshipId, apprenticeship);

            using (CommitmentsApi webApi = new CommitmentsApi(apiMessageHandlers))
            {
                using (HttpClient client = new HttpClient())
                {
                    var jsonresponse = await client.GetAsync(baseAddress + apiMessageHandlers.GetProviderApprenticeship(ProviderId, ApprenticeshipId));
                    var response = JsonConvert.DeserializeObject<Apprenticeship>(jsonresponse.Content.ReadAsStringAsync().Result);
                    Assert.AreEqual(ProviderId, response.ProviderId);
                }
            }
        }
    }
}
