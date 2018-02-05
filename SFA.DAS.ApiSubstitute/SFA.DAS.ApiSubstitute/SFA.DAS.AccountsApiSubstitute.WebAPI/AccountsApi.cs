using SFA.DAS.ApiSubstitute.WebAPI;


namespace SFA.DAS.AccountsApiSubstitute.WebAPI
{
    public class AccountsApi : WebApiSubstitute
    {
        public string BaseAddress { get; private set; }

        public AccountsApiMessageHandler AccountsApiMessageHandler { get; private set; }

        public AccountsApi(AccountsApiMessageHandler apiMessageHandler) : base(apiMessageHandler)
        {
            BaseAddress = apiMessageHandler.BaseAddress;
            AccountsApiMessageHandler = apiMessageHandler;
        }
    }
}
