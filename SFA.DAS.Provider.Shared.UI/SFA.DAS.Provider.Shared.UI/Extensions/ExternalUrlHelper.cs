using System.Text;
using Microsoft.Extensions.Options;
using SFA.DAS.Provider.Shared.UI.Models;

namespace SFA.DAS.Provider.Shared.UI.Extensions
{
    public interface IExternalUrlHelper
    {
        string GenerateUrl(UrlParameters urlParameters);
    }
    public class ExternalUrlHelper : IExternalUrlHelper
    {
        private readonly ProviderSharedUIConfiguration _options;

        public ExternalUrlHelper(IOptions<ProviderSharedUIConfiguration> options)
        {
            _options = options.Value;
        }

        public string GenerateUrl(UrlParameters urlParameters)
        {
            var baseUrl = _options.DashboardUrl;

            return FormatUrl(baseUrl, urlParameters);
        }

        private static string FormatUrl(string baseUrl, UrlParameters urlParameters)
        {
            var urlString = new StringBuilder();

            urlString.Append(FormatBaseUrl(baseUrl, urlParameters.SubDomain, urlParameters.Folder));

            if (!string.IsNullOrEmpty(urlParameters.Id))
            {
                urlString.Append($"{urlParameters.Id}/");
            }

            if (!string.IsNullOrEmpty(urlParameters.Controller))
            {
                urlString.Append($"{urlParameters.Controller}/");
            }

            if (!string.IsNullOrEmpty(urlParameters.Action))
            {
                urlString.Append($"{urlParameters.Action}/");
            }

            if (!string.IsNullOrEmpty(urlParameters.QueryString))
            {
                return $"{urlString.ToString().TrimEnd('/')}{urlParameters.QueryString}";
            }

            return urlString.ToString().TrimEnd('/');
        }

        private static string FormatBaseUrl(string url, string subDomain = "", string folder = "")
        {
            var returnUrl = url.EndsWith("/")
                ? url
                : url + "/";

            if (!string.IsNullOrEmpty(subDomain))
            {
                returnUrl = returnUrl.Replace("https://", $"https://{subDomain}.");
            }

            if (!string.IsNullOrEmpty(folder))
            {
                returnUrl = $"{returnUrl}{folder}/";
            }

            return returnUrl;
        }
    }
}