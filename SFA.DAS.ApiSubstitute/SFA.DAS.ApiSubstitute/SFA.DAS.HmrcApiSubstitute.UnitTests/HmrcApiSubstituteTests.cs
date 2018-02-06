using System;
using NUnit.Framework;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using SFA.DAS.HmrcApiSubstitute.WebAPI;
using HMRC.ESFA.Levy.Api.Types;
using System.Web;
using SFA.DAS.ApiSubstitute.Utilities;
using HMRC.ESFA.Levy.Api.Client;

namespace SFA.DAS.HmrcApiSubstitute.UnitTests
{
    [TestFixture]
    public class HmrcApiSubstituteTests
    {
        private string baseAddress;

        private HmrcApiMessageHandler apiMessageHandlers;

        [SetUp]
        public void SetUp()
        {
            baseAddress = "http://localhost:9010/";
            apiMessageHandlers = new HmrcApiMessageHandler(baseAddress);
        }

        [Test]
        public async Task CanUseDefaultResponseForGetEmploymentStatus()
        {
            using (HmrcApi webApi = new HmrcApi(apiMessageHandlers))
            {
                using (HttpClient client = new HttpClient())
                {
                    var jsonresponse = await client.GetAsync(baseAddress + apiMessageHandlers.GetEmploymentStatus);
                    var response = JsonConvert.DeserializeObject<EmploymentStatus>(jsonresponse.Content.ReadAsStringAsync().Result);
                    Assert.AreEqual(true, response.Employed);
                }
            }
        }

        [Test]
        public async Task CanOverrideDefaultResponse()
        {
            apiMessageHandlers.OverrideGetSubmissionEvents(new EmploymentStatus { Employed = false });

            using (HmrcApi webApi = new HmrcApi(apiMessageHandlers))
            {
                using (HttpClient client = new HttpClient())
                {
                    var jsonresponse = await client.GetAsync(baseAddress + apiMessageHandlers.GetEmploymentStatus);
                    var response = JsonConvert.DeserializeObject<EmploymentStatus>(jsonresponse.Content.ReadAsStringAsync().Result);
                    Assert.AreEqual(false, response.Employed);
                }
            }
        }

        [Test]
        public async Task CanUseHmrcClientPackage()
        {
            var Request = $"apprenticeship-levy/epaye/{"111/ABC00001"}/employed/AB956885B?fromDate=2017-12-14&toDate=2018-02-06";
            var stubresponse = new ObjectCreator().Create<EmploymentStatus>(x => { x.Employed = true; x.Empref = "111/ABC00001"; x.Nino = "AB956885B"; x.FromDate = new DateTime(2017, 12, 14); x.ToDate = new DateTime(2018, 02, 06); });

            apiMessageHandlers.SetupGet(Request, stubresponse);

            using (HmrcApi webApi = new HmrcApi(apiMessageHandlers))
            {
                using (HttpClient client = new HttpClient {BaseAddress = new Uri(baseAddress) })
                {
                    var hmrcclient = new ApprenticeshipLevyApiClient(client);
                    var jsonresponse = await hmrcclient.GetEmploymentStatus("111/ABC00001", "AB956885B", new DateTime(2017, 12, 14), new DateTime(2018, 02, 06));
                    Assert.AreEqual(true, jsonresponse.Employed);
                }
            }
        }
    }
}
