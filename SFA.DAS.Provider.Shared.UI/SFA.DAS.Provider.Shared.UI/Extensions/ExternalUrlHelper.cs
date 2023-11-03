using System;
using System.Text;
using Microsoft.Extensions.Configuration;
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
        private readonly IConfiguration _configuration;
        private readonly ProviderSharedUIConfiguration _options;
        private readonly bool _isLocal;

        public ExternalUrlHelper(IOptions<ProviderSharedUIConfiguration> options, IConfiguration configuration)
        {
            _configuration = configuration;
            _options = options.Value;
            _isLocal = (_configuration["EnvironmentName"]?.StartsWith("LOCAL", StringComparison.CurrentCultureIgnoreCase)).GetValueOrDefault();
        }

        public string GenerateUrl(UrlParameters urlParameters)
        {
            var baseUrl = _isLocal && !string.IsNullOrEmpty(_configuration.GetSection("LocalPorts")[urlParameters.SubDomain]) ? "https://localhost" : _options.DashboardUrl;

            return FormatUrl(baseUrl, urlParameters);
        }

        private string FormatUrl(string baseUrl, UrlParameters urlParameters)
        {
            var urlString = new StringBuilder();

            urlString.Append(_isLocal
                ? FormatBaseUrlLocal(baseUrl, urlParameters.SubDomain, urlParameters.Folder)
                : FormatBaseUrl(baseUrl, urlParameters.SubDomain, urlParameters.Folder));

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

        private string FormatBaseUrlLocal(string url, string subDomain = "", string folder = "")
        {
            var localPort = _configuration.GetSection("LocalPorts")[subDomain];
            if (string.IsNullOrEmpty(localPort)) return FormatBaseUrl(url, subDomain, folder);

            var returnUrl = $"{url}:{localPort}/";

            if (!string.IsNullOrEmpty(folder))
            {
                returnUrl = $"{returnUrl}{folder}/";
            }

            return returnUrl;
        }
    }
}