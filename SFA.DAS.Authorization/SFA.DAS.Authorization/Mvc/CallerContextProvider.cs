using System;
using System.Web;
using SFA.DAS.Authentication;
using SFA.DAS.Hashing;
using SFA.DAS.HashingService;

namespace SFA.DAS.Authorization.Mvc
{
    public class CallerContextProvider : ICallerContextProvider
    {
        private static readonly string Key = typeof(CallerContext).FullName;

        private readonly HttpContextBase _httpContext;
        private readonly IAuthenticationService _authenticationService;
        private readonly IHashingService _hashingService;

        public CallerContextProvider(HttpContextBase httpContext, IAuthenticationService authenticationService, IHashingService hashingService)
        {
            _httpContext = httpContext;
            _authenticationService = authenticationService;
            _hashingService = hashingService;
        }

        public ICallerContext GetCallerContext()
        {
            if (_httpContext.Items.Contains(Key))
            {
                return _httpContext.Items[Key] as CallerContext;
            }

            var accountHashedId = GetAccountHashedId();
            var accountId = accountHashedId == null ? null : GetAccountId(accountHashedId);
            var userRef = GetUserRef();

            var requestContext = new CallerContext
            {
                AccountHashedId = accountHashedId,
                AccountId = accountId,
                UserRef = userRef
            };

            _httpContext.Items[Key] = requestContext;

            return requestContext;
        }

        private string GetAccountHashedId()
        {
            if (!_httpContext.Request.RequestContext.RouteData.Values.TryGetValue("HashedAccountId", out var accountHashedId))
            {
                return null;
            }

            return (string)accountHashedId;
        }

        private long? GetAccountId(string accountHashedId)
        {
            if (!_hashingService.TryDecodeValue(accountHashedId, out var accountId))
            {
                throw new UnauthorizedAccessException();
            }

            return accountId;
        }

        private Guid? GetUserRef()
        {
            if (!_authenticationService.IsUserAuthenticated())
            {
                return null;
            }

            if (!_authenticationService.TryGetClaimValue("sub", out var userRefClaimValue))
            {
                throw new UnauthorizedAccessException();
            }

            if (!Guid.TryParse(userRefClaimValue, out var userRef))
            {
                throw new UnauthorizedAccessException();
            }

            return userRef;
        }
    }
}