using Microsoft.Owin.Hosting;
using System;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.ApiSubstitute.WebAPI.OwinSelfHost;
using SFA.DAS.ApiSubstitute.WebAPI.MessageHandlers;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net;

namespace SFA.DAS.ApiSubstitute.UnitTests
{
    [TestFixture]
    public class ApiSubstituteTests : IDisposable
    {
        private IDisposable _webApi;
        string baseAddress = "http://localhost:9002";

        ApiMessageHandlers _apiMessageHandlers;
        TestAccount expectedresponce;
        string route;
        string endpoint;

        [SetUp]
        public void TestInitialize()
        {
            _apiMessageHandlers = new ApiMessageHandlers(baseAddress);
            _webApi = WebApp.Start(baseAddress, appBuilder => new WebApiStartUp().Configuration(appBuilder, _apiMessageHandlers));
            expectedresponce = new TestAccount(1);
            endpoint = "/account";
            route = baseAddress + endpoint;
        }

        [Test]
        public async Task CanHostWebApi()
        {
            
            _apiMessageHandlers.SetupGet(endpoint, expectedresponce);
            using (HttpClient client = new HttpClient())
            {
                var jsonresponse = await client.GetAsync(route);
                jsonresponse.EnsureSuccessStatusCode();
            }
        }

        [Test]
        public async Task CanUseApiMessageHandlersSetUpGet()
        {
            _apiMessageHandlers.SetupGet(endpoint, expectedresponce);
            using (HttpClient client = new HttpClient())
            {
                var jsonresponse = await client.GetAsync(route);
                var response = JsonConvert.DeserializeObject<TestAccount>(jsonresponse.Content.ReadAsStringAsync().Result);
                Assert.AreEqual(1, response.Accountid);
            }
        }

        [Test]
        public async Task CanUseApiMessageHandlersSetUpPut()
        {
            _apiMessageHandlers.SetupGet(endpoint, expectedresponce);
            _apiMessageHandlers.SetupPut(endpoint);
            using (HttpClient client = new HttpClient())
            {
                var jsonresponse = await client.GetAsync(route);
                Assert.AreEqual(HttpStatusCode.NotFound, jsonresponse.StatusCode);
            }
        }

        [Test]
        public async Task CanUseApiMessageHandlersSetUpPatch()
        {
            _apiMessageHandlers.SetupPatch(endpoint, expectedresponce);
            using (HttpClient client = new HttpClient())
            {
                var jsonresponse = await client.GetAsync(route);
                var response = JsonConvert.DeserializeObject<TestAccount>(jsonresponse.Content.ReadAsStringAsync().Result);
                Assert.AreEqual(1, response.Accountid);
            }
        }

        [Test]
        public async Task CanUseApiMessageHandlersClearSetup()
        {
            _apiMessageHandlers.SetupGet(endpoint, expectedresponce);
            _apiMessageHandlers.ClearSetup();
            using (HttpClient client = new HttpClient())
            {
                var jsonresponse = await client.GetAsync(route);
                Assert.AreEqual(HttpStatusCode.NotFound, jsonresponse.StatusCode);
            }
        }


        [Test]
        public async Task CanUseApiMessageHandlersSetUpGetWithResponceCode()
        {
            _apiMessageHandlers.SetupGet<object>(endpoint, HttpStatusCode.BadRequest, null);
            using (HttpClient client = new HttpClient())
            {
                var jsonresponse = await client.GetAsync(route);
                Assert.AreEqual(HttpStatusCode.BadRequest, jsonresponse.StatusCode);
            }
        }

        [TearDown]
        public void TestCleanup()
        {
            Dispose();
        }

        public void Dispose()
        {
            _webApi.Dispose();
        }
    }
}
