using SFA.DAS.ApiSubstitute.Utilities;
using SFA.DAS.ApiSubstitute.WebAPI.MessageHandlers;
using SFA.DAS.EAS.Account.Api.Types;
using System.Collections.Generic;

namespace SFA.DAS.AccountsApiSubstitute.WebAPI
{
    public class AccountsApiMessageHandler : ApiMessageHandlers
    {
        public string DefaultGetAccountEndPoint { get; private set; }

        public string DefaultGetAccountUsingHashedIdEndPoint { get; private set; }

        private IObjectCreator _objectCreator;

        public string GetAccount(long accountid)
        {
            return $"api/accounts/{accountid}";
        }

        public string GetAccountUsingHashedId(string hashedAccountId)
        {
            return $"api/accounts/{hashedAccountId}";
        }

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
            SetupPatch(DefaultGetAccountEndPoint, response);
        }
        public void OverrideGetAccountUsingHashedId<T>(T response)
        {
            SetupPatch(DefaultGetAccountUsingHashedIdEndPoint, response);
        }

        public void SetupGetAccount<T>(long accountid, T response)
        {
            SetupPatch(GetAccount(accountid), response);
        }

        public void SetupGetAccountUsingHashedId<T>(string hashedAccountId, T response)
        {
            SetupPatch(GetAccountUsingHashedId(hashedAccountId), response);
        }

        private void ConfigureGetAccount()
        {
            var payeschemes = new List<ResourceViewModel> { new ResourceViewModel { Id = PayeScheme, Href = Href } };
            var resourceList = new ResourceList(payeschemes);
            var accounts = _objectCreator.Create<AccountDetailViewModel>(x => { x.AccountId = AccountId; x.PayeSchemes = resourceList; });

            DefaultGetAccountEndPoint = GetAccount(AccountId);

            SetupGet(DefaultGetAccountEndPoint, accounts);
        }

        private void ConfigureGetAccountUsingHashedId()
        {
            var payeschemes = new List<ResourceViewModel> { new ResourceViewModel { Id = PayeScheme, Href = Href } };
            var resourceList = new ResourceList(payeschemes);
            var accounts = _objectCreator.Create<AccountDetailViewModel>(x => { x.HashedAccountId = HashedAccountId; x.PayeSchemes = resourceList; });

            DefaultGetAccountUsingHashedIdEndPoint = GetAccountUsingHashedId(HashedAccountId);

            SetupGet(DefaultGetAccountUsingHashedIdEndPoint, accounts);
        }
    }
}