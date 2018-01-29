using Microsoft.Owin.Hosting;
using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SFA.DAS.ApiSubstitute.WebAPI.OwinSelfHost;
using SFA.DAS.ApiSubstitute.WebAPI.MessageHandlers;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net;

namespace SFA.DAS.ApiSubstitute.UnitTests
{
    [TestClass]
    public class ApiSubstituteTests : IDisposable
    {
        private IDisposable _webApi;
        string baseAddress = "http://localhost:9000";

        ApiMessageHandlers _apiMessageHandlers;
        TestAccount expectedresponce;
        string route;

        [TestInitialize]
        public void TestInitialize()
        {
            _apiMessageHandlers = new ApiMessageHandlers();
            _webApi = WebApp.Start(baseAddress, appBuilder => new WebApiStartUp().Configuration(appBuilder, _apiMessageHandlers));
            expectedresponce = new TestAccount(1);
            route = baseAddress + "/account";
        }

        [TestMethod]
        public async Task CanHostWebApi()
        {
            
            _apiMessageHandlers.SetupGet(route, expectedresponce);
            using (HttpClient client = new HttpClient())
            {
                var jsonresponse = await client.GetAsync(route);
                jsonresponse.EnsureSuccessStatusCode();
            }
        }

        [TestMethod]
        public async Task CanUseApiMessageHandlersSetUpGet()
        {
            _apiMessageHandlers.SetupGet(route, expectedresponce);
            using (HttpClient client = new HttpClient())
            {
                var jsonresponse = await client.GetAsync(route);
                var response = JsonConvert.DeserializeObject<TestAccount>(jsonresponse.Content.ReadAsStringAsync().Result);
                Assert.AreEqual(1, response.Accountid);
            }
        }

        [TestMethod]
        public async Task CanUseApiMessageHandlersSetUpPut()
        {
            _apiMessageHandlers.SetupGet(route, expectedresponce);
            _apiMessageHandlers.SetupPut(route);
            using (HttpClient client = new HttpClient())
            {
                var jsonresponse = await client.GetAsync(route);
                Assert.AreEqual(HttpStatusCode.NotFound, jsonresponse.StatusCode);
            }
        }


        [TestMethod]
        public async Task CanUseApiMessageHandlersClearSetup()
        {
            _apiMessageHandlers.SetupGet(route, expectedresponce);
            _apiMessageHandlers.ClearSetup();
            using (HttpClient client = new HttpClient())
            {
                var jsonresponse = await client.GetAsync(route);
                Assert.AreEqual(HttpStatusCode.NotFound, jsonresponse.StatusCode);
            }
        }


        [TestMethod]
        public async Task CanUseApiMessageHandlersSetUpGetWithResponceCode()
        {
            _apiMessageHandlers.SetupGet<object>(route, HttpStatusCode.BadRequest, null);
            using (HttpClient client = new HttpClient())
            {
                var jsonresponse = await client.GetAsync(route);
                Assert.AreEqual(HttpStatusCode.BadRequest, jsonresponse.StatusCode);
            }
        }

        [TestCleanup]
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
