using SFA.DAS.ApiSubstitute.Utilities;
using SFA.DAS.ApiSubstitute.WebAPI.MessageHandlers;
using SFA.DAS.EAS.Account.Api.Types;
using System.Collections.Generic;

namespace SFA.DAS.AccountsApiSubstitute.WebAPI
{
    public class AccountsApiMessageHandler : ApiMessageHandlers
    {
        private IObjectCreator _objectCreator;

        public string GetAccount => $"api/accounts/{AccountId}";

        public string GetAccountUsingHashedId => $"api/accounts/{HashedAccountId}";

        public const long AccountId = 8080;
        public const string HashedAccountId = "VD96WD";
        public const string PayeScheme = "111/ABC00001";
        public string Href = $"api/accounts/{AccountId}/payescheme/{PayeScheme}";

        public AccountsApiMessageHandler(string baseAddress) : base(baseAddress)
        {
            _objectCreator = new ObjectCreator();
            ConfigureDefaultResponse();
        }

        public void ConfigureDefaultResponse()
        {
            ConfigureGetAccount();
            ConfigureGetAccountUsingHashedId();
        }

        public void OverrideGetAccount<T>(T response)
        {
            SetupPut(GetAccount);
            SetupGet(GetAccount, response);
        }
        public void OverrideGetAccountUsingHashedId<T>(T response)
        {
            SetupPut(GetAccountUsingHashedId);
            SetupGet(GetAccountUsingHashedId, response);
        }

        private void ConfigureGetAccount()
        {
            var payeschemes = new List<ResourceViewModel> { new ResourceViewModel { Id = PayeScheme, Href = Href } };
            var resourceList = new ResourceList(payeschemes);
            var accounts = _objectCreator.Create<AccountDetailViewModel>(x => { x.AccountId = AccountId; x.PayeSchemes = resourceList; });

            SetupGet(GetAccount, accounts);
        }
        private void ConfigureGetAccountUsingHashedId()
        {
            var payeschemes = new List<ResourceViewModel> { new ResourceViewModel { Id = PayeScheme, Href = Href } };
            var resourceList = new ResourceList(payeschemes);
            var accounts = _objectCreator.Create<AccountDetailViewModel>(x => { x.HashedAccountId = HashedAccountId; x.PayeSchemes = resourceList; });

            SetupGet(GetAccountUsingHashedId, accounts);
        }
    }
}