using SFA.DAS.MA.Shared.UI.Configuration;

namespace SFA.DAS.MA.Shared.UI.Services
{
    public class UrlHelper : IUrlHelper
    {  
        public string GetPath(string baseUrl, string path = "")
        {
            var trimmedBaseUrl = baseUrl?.TrimEnd('/') ?? string.Empty;

            return $"{trimmedBaseUrl}/{path}".TrimEnd('/');
        }

        public string GetPath(IUserContext userContext, string baseUrl, string path = "", string prefix = "accounts")
        {
            prefix = string.IsNullOrEmpty(prefix) ? string.Empty : prefix + '/';
            var accountPath = userContext.HashedAccountId == null ? $"{prefix}{path}" : $"{prefix}{userContext.HashedAccountId}/{path}";

            var trimmedBaseUrl = baseUrl?.TrimEnd('/') ?? string.Empty;

            return $"{trimmedBaseUrl}/{accountPath}".TrimEnd('/');
        }
    }
}
