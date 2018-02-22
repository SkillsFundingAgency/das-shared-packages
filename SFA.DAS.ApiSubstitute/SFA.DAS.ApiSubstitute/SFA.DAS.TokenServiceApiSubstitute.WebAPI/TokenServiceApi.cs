using SFA.DAS.ApiSubstitute.WebAPI;

namespace SFA.DAS.TokenServiceApiSubstitute.WebAPI
{
    public class TokenServiceApi : WebApiSubstitute
    {
        public string BaseAddress { get; private set; }

        public TokenServiceApiMessageHandler TokenServiceApiMessageHandler { get; private set; }

        public TokenServiceApi(TokenServiceApiMessageHandler apiMessageHandler) : base(apiMessageHandler)
        {
            BaseAddress = apiMessageHandler.BaseAddress;
            TokenServiceApiMessageHandler = apiMessageHandler;
        }
    }
}
