using NUnit.Framework;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.AccountsApiSubstitute.WebAPI;

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
                    var jsonresponse = await client.GetAsync(baseAddress + apiMessageHandlers.GetAccount);
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
                    var jsonresponse = await client.GetAsync(baseAddress + apiMessageHandlers.GetAccountUsingHashedId);
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
                    var jsonresponse = await client.GetAsync(baseAddress + apiMessageHandlers.GetAccount);
                    var response = JsonConvert.DeserializeObject<AccountDetailViewModel>(jsonresponse.Content.ReadAsStringAsync().Result);
                    Assert.AreEqual(9090, response.AccountId);
                }
            }
        }
    }
}

