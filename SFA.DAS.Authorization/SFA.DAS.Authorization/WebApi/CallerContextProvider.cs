using System;
using System.Net.Http;
using SFA.DAS.Hashing;
using SFA.DAS.HashingService;

namespace SFA.DAS.Authorization.WebApi
{
    public class CallerContextProvider : ICallerContextProvider
    {
        private static readonly string Key = typeof(CallerContext).FullName;

        private readonly HttpRequestMessage _httpRequestMessage;
        private readonly IHashingService _hashingService;

        public CallerContextProvider(HttpRequestMessage httpRequestMessage, IHashingService hashingService)
        {
            _httpRequestMessage = httpRequestMessage;
            _hashingService = hashingService;
        }

        public ICallerContext GetCallerContext()
        {
            if (_httpRequestMessage.Properties.ContainsKey(Key))
            {
                return (CallerContext)_httpRequestMessage.Properties[Key];
            }

            var accountHashedId = GetAccountHashedId();
            var accountId = accountHashedId == null ? null : GetAccountId(accountHashedId);

            var requestContext = new CallerContext
            {
                AccountHashedId = accountHashedId,
                AccountId = accountId,
                UserRef = null
            };

            _httpRequestMessage.Properties[Key] = requestContext;

            return requestContext;
        }

        private string GetAccountHashedId()
        {
            if (!_httpRequestMessage.GetRouteData().Values.TryGetValue("hashedAccountId", out var accountHashedId))
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
    }
}