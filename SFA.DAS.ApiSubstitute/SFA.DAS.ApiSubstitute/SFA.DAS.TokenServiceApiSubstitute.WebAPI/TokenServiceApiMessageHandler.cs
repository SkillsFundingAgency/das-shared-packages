using System;
using SFA.DAS.ApiSubstitute.Utilities;
using SFA.DAS.ApiSubstitute.WebAPI.MessageHandlers;
using SFA.DAS.TokenService.Api.Types;
using System.Net;

namespace SFA.DAS.TokenServiceApiSubstitute.WebAPI
{
    public class TokenServiceApiMessageHandler : ApiMessageHandlers
    {

        public string DefaultGetPrivilegedAccessTokenAsyncEndPoint { get; set; }

        private IObjectCreator _objectCreator;

        public TokenServiceApiMessageHandler(string baseAddress) : base(baseAddress)
        {
            _objectCreator = new ObjectCreator();
            ConfigureDefaultResponse();
        }

        private void ConfigureDefaultResponse()
        {
            ConfigureGetPrivilegedAccessTokenAsync();
        }

        public void OverrideGetPrivilegedAccessTokenAsync(PrivilegedAccessToken response, HttpStatusCode httpStatusCode = HttpStatusCode.OK)
        {
            SetupCall(DefaultGetPrivilegedAccessTokenAsyncEndPoint, httpStatusCode, response);
        }

        public void SetupGetPrivilegedAccessTokenAsync(PrivilegedAccessToken response, HttpStatusCode httpStatusCode = HttpStatusCode.OK)
        {
            SetupCall(GetPrivilegedAccessTokenAsync(), httpStatusCode, response);
        }
        
        private void ConfigureGetPrivilegedAccessTokenAsync()
        {
            DefaultGetPrivilegedAccessTokenAsyncEndPoint = GetPrivilegedAccessTokenAsync();

            SetupGet(DefaultGetPrivilegedAccessTokenAsyncEndPoint, HttpStatusCode.OK, new PrivilegedAccessToken { AccessCode = "AccessCode", ExpiryTime = DateTime.Now.AddDays(1) });
        }

        private string GetPrivilegedAccessTokenAsync()
        {
            return $"api/PrivilegedAccess";
        }
    }
}