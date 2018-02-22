using SFA.DAS.ApiSubstitute.WebAPI;

namespace SFA.DAS.TokenServiceApiSubstitute.WebAPI
{
    public class TokenServiceApi : WebApiSubstitute
    {
        public string BaseAddress { get; private set; }

        public TokenServiceApiMessageHandler AccountsApiMessageHandler { get; private set; }

        public TokenServiceApi(TokenServiceApiMessageHandler apiMessageHandler) : base(apiMessageHandler)
        {
            BaseAddress = apiMessageHandler.BaseAddress;
            AccountsApiMessageHandler = apiMessageHandler;
        }
    }
}
