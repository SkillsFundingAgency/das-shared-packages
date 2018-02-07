using System;
using NUnit.Framework;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using SFA.DAS.HmrcApiSubstitute.WebAPI;
using HMRC.ESFA.Levy.Api.Types;
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
                    var jsonresponse = await client.GetAsync(baseAddress + apiMessageHandlers.DefaultGetEmploymentStatusEndPoint);
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
                    var jsonresponse = await client.GetAsync(baseAddress + apiMessageHandlers.DefaultGetEmploymentStatusEndPoint);
                    var response = JsonConvert.DeserializeObject<EmploymentStatus>(jsonresponse.Content.ReadAsStringAsync().Result);
                    Assert.AreEqual(false, response.Employed);
                }
            }
        }

        [Test]
        public async Task CanUseHmrcClientPackage()
        {
            var stubresponse = new ObjectCreator().Create<EmploymentStatus>(x => { x.Employed = true; x.Empref = "111/ABC00001"; x.Nino = "AB956885B"; x.FromDate = new DateTime(2017, 12, 14); x.ToDate = new DateTime(2018, 02, 06); });

            apiMessageHandlers.SetupGetEmploymentStatus(stubresponse,"111/ABC00001", "AB956885B", new DateTime(2017, 12, 14), new DateTime(2018, 02, 06));

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

        [Test]
        public void CanUseGetEmploymentStatusEndpointWithNullFromAndToDate()
        {
            var expectedendpoint = $"apprenticeship-levy/epaye/111/ABC00001/employed/AB956885B";
            var actualendpoint = apiMessageHandlers.GetEmploymentStatus("111/ABC00001", "AB956885B");
            Assert.AreEqual(expectedendpoint, actualendpoint);
        }

        [Test]
        public void CanUseGetEmploymentStatusEndpointWithNullFromDate()
        {
            var expectedendpoint = $"apprenticeship-levy/epaye/111/ABC00001/employed/AB956885B?toDate=2018-02-06";
            var actualendpoint = apiMessageHandlers.GetEmploymentStatus("111/ABC00001", "AB956885B", null, new DateTime(2018, 02, 06));
            Assert.AreEqual(expectedendpoint, actualendpoint);
        }

        [Test]
        public void CanUseGetEmploymentStatusEndpointWithNullToDate()
        {
            var expectedendpoint = $"apprenticeship-levy/epaye/111/ABC00001/employed/AB956885B?fromDate=2017-12-14";
            var actualendpoint = apiMessageHandlers.GetEmploymentStatus("111/ABC00001", "AB956885B", new DateTime(2017, 12, 14));
            Assert.AreEqual(expectedendpoint, actualendpoint);
        }
        [Test]
        public void CanUseGetEmploymentStatusEndpointWithFromAndToDate()
        {
            var expectedendpoint = $"apprenticeship-levy/epaye/111/ABC00001/employed/AB956885B?fromDate=2017-12-14&toDate=2018-02-06";
            var actualendpoint = apiMessageHandlers.GetEmploymentStatus("111/ABC00001", "AB956885B", new DateTime(2017, 12, 14), new DateTime(2018, 02, 06));
            Assert.AreEqual(expectedendpoint, actualendpoint);
        }
    }
}
