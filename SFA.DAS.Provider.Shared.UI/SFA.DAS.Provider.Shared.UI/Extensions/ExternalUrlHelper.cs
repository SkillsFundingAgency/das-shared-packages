using System;
using System.IO;
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
            _isLocal = (_configuration["ResourceEnvironmentName"]?.StartsWith("LOCAL", StringComparison.CurrentCultureIgnoreCase)).GetValueOrDefault();
        }

        public string GenerateUrl(UrlParameters urlParameters)
        {
            var localPort = _configuration.GetSection("LocalPorts")?[urlParameters.SubDomain];

            var baseUrl = _isLocal && !string.IsNullOrEmpty(localPort)
                ? FormatBaseUrl($"https://localhost:{localPort}/", null, urlParameters.Folder)
                : FormatBaseUrl(_options.DashboardUrl, urlParameters.SubDomain, urlParameters.Folder);

            return FormatUrl(baseUrl, urlParameters);
        }

        private static string FormatUrl(string baseUrl, UrlParameters urlParameters)
        {
            var urlString = new StringBuilder();

            urlString.Append(baseUrl);

            if (!string.IsNullOrEmpty(urlParameters.RelativeRoute))
            {
                return Path.Combine(urlString.ToString(), urlParameters.RelativeRoute);
            }

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