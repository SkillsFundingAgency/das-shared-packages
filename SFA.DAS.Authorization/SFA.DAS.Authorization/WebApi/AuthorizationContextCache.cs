using System.Net.Http;

namespace SFA.DAS.Authorization.WebApi
{
    public class AuthorizationContextCache : IAuthorizationContextCache
    {
        private static readonly string Key = typeof(AuthorizationContext).FullName;

        private readonly HttpRequestMessage _httpRequestMessage;

        public AuthorizationContextCache(HttpRequestMessage httpRequestMessage)
        {
            _httpRequestMessage = httpRequestMessage;
        }

        public IAuthorizationContext GetAuthorizationContext()
        {
            if (_httpRequestMessage.Properties.ContainsKey(Key))
            {
                return _httpRequestMessage.Properties[Key] as AuthorizationContext;
            }

            return null;
        }

        public void SetAuthorizationContext(IAuthorizationContext authorizationContext)
        {
            _httpRequestMessage.Properties[Key] = authorizationContext;
        }
    }
}