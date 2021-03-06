﻿using SFA.DAS.ApiSubstitute.Utilities;
using SFA.DAS.ApiSubstitute.WebAPI.MessageHandlers;
using SFA.DAS.EAS.Account.Api.Types;
using System.Collections.Generic;
using System.Net;

namespace SFA.DAS.AccountsApiSubstitute.WebAPI
{
    public class AccountsApiMessageHandler : ApiMessageHandlers
    {
        private string DefaultGetAccountEndPoint { get; set; }

        private string DefaultGetAccountUsingHashedIdEndPoint { get; set; }

        private IObjectCreator _objectCreator;

        public long AccountId => 8080;
        public string HashedAccountId => "VD96WD";
        public string PayeScheme => "111/ABC00001";
        public string Href => $"api/accounts/{AccountId}/payescheme/{PayeScheme}";

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

        public void OverrideGetAccount(AccountDetailViewModel response, HttpStatusCode httpStatusCode = HttpStatusCode.OK)
        {
            SetupCall(DefaultGetAccountEndPoint, httpStatusCode, response);
        }
        public void OverrideGetAccountUsingHashedId(AccountDetailViewModel response, HttpStatusCode httpStatusCode = HttpStatusCode.OK)
        {
            SetupCall(DefaultGetAccountUsingHashedIdEndPoint, httpStatusCode, response);
        }

        public void SetupGetAccount(long accountid, AccountDetailViewModel response, HttpStatusCode httpStatusCode = HttpStatusCode.OK)
        {
            SetupCall(GetAccount(accountid), httpStatusCode, response);
        }

        public void SetupGetAccountUsingHashedId(string hashedAccountId, AccountDetailViewModel response, HttpStatusCode httpStatusCode = HttpStatusCode.OK)
        {
            SetupCall(GetAccountUsingHashedId(hashedAccountId), httpStatusCode, response);
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

        private string GetAccount(long accountid)
        {
            return $"api/accounts/{accountid}";
        }

        private string GetAccountUsingHashedId(string hashedAccountId)
        {
            return $"api/accounts/{hashedAccountId}";
        }
    }
}