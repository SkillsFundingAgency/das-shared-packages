using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;
using Newtonsoft.Json;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.AccountsApiSubstitute.WebAPI;
using SFA.DAS.ApiSubstitute.Utilities;

namespace SFA.DAS.AccountsApiSubstitute.UnitTests
{
    [TestFixture]
    public class AccountsApiSubstituteTests
    {
        private string baseAddress;

        private AccountsApiMessageHandler apiMessageHandlers;

        [SetUp]
        public void SetUp()
        {
            baseAddress = "http://localhost:9006/";
            apiMessageHandlers = new AccountsApiMessageHandler(baseAddress);
        }

        [Test]
        public async Task CanUseDefaultResponseForGetAccount()
        {
            using (AccountsApi webApi = new AccountsApi(apiMessageHandlers))
            {
                using (HttpClient client = new HttpClient())
                {
                    var jsonresponse = await client.GetAsync(baseAddress + $"api/accounts/{apiMessageHandlers.AccountId}");
                    var response = JsonConvert.DeserializeObject<AccountDetailViewModel>(jsonresponse.Content.ReadAsStringAsync().Result);
                    Assert.AreEqual("111/ABC00001", response.PayeSchemes[0].Id);
                    Assert.AreEqual(8080, response.AccountId);
                }
            }
        }


        [Test]
        public async Task CanUseDefaultResponseForGetAccountUsingHashedId()
        {
            using (AccountsApi webApi = new AccountsApi(apiMessageHandlers))
            {
                using (HttpClient client = new HttpClient())
                {
                    var jsonresponse = await client.GetAsync(baseAddress + $"api/accounts/{apiMessageHandlers.HashedAccountId}");
                    var response = JsonConvert.DeserializeObject<AccountDetailViewModel>(jsonresponse.Content.ReadAsStringAsync().Result);
                    Assert.AreEqual("111/ABC00001", response.PayeSchemes[0].Id);
                    Assert.AreEqual("VD96WD", response.HashedAccountId);
                }
            }
        }

        [Test]
        public async Task CanOverrideDefaultResponse()
        {
            apiMessageHandlers.OverrideGetAccount(new AccountDetailViewModel { AccountId = 9090 });

            using (AccountsApi webApi = new AccountsApi(apiMessageHandlers))
            {
                using (HttpClient client = new HttpClient())
                {
                    var jsonresponse = await client.GetAsync(baseAddress + $"api/accounts/{apiMessageHandlers.AccountId}");
                    var response = JsonConvert.DeserializeObject<AccountDetailViewModel>(jsonresponse.Content.ReadAsStringAsync().Result);
                    Assert.AreEqual(9090, response.AccountId);
                }
            }
        }

        [Test]
        public async Task CanSetupSetupGetAccount()
        {
            long accountid = 564781;

            var payeschemes = new List<ResourceViewModel> { new ResourceViewModel { Id = "111/ABC00011", Href = "" } };
            var resourceList = new ResourceList(payeschemes);
            var accounts = new ObjectCreator().Create<AccountDetailViewModel>(x => { x.AccountId = accountid; x.PayeSchemes = resourceList; });

            apiMessageHandlers.SetupGetAccount(accountid, accounts);

            using (AccountsApi webApi = new AccountsApi(apiMessageHandlers))
            {
                using (HttpClient client = new HttpClient())
                {
                    var jsonresponse = await client.GetAsync(baseAddress + $"api/accounts/{accountid}");
                    var response = JsonConvert.DeserializeObject<AccountDetailViewModel>(jsonresponse.Content.ReadAsStringAsync().Result);
                    Assert.AreEqual(accountid, response.AccountId);
                }
            }
        }
    }
}

