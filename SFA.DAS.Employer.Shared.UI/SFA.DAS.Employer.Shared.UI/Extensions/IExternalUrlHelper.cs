using System;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Options;
using SFA.DAS.Employer.Shared.UI.Models;

namespace SFA.DAS.Employer.Shared.UI.Extensions
{
    public interface IExternalUrlHelper
    {
        string GenerateUrl(UrlParameters urlParameters);
    }

    public class ExternalUrlHelper : IExternalUrlHelper
    {
        private readonly EmployerSharedUIConfiguration _options;
        private readonly bool _isLocal;
        private const char _slash = '/';

        public ExternalUrlHelper(IOptions<EmployerSharedUIConfiguration> options)
        {
            _options = options?.Value ?? new EmployerSharedUIConfiguration();
            var env = _options.ResourceEnvironmentName;
            _isLocal = !string.IsNullOrEmpty(env) && env.StartsWith("LoCAL", StringComparison.OrdinalIgnoreCase);
        }

        public string GenerateUrl(UrlParameters urlParameters)
        {
            var localPort = GetLocalPorts(urlParameters.SubDomain);

            var baseUrl = _isLocal && !string.IsNullOrEmpty(localPort)
                ? FormatBaseUrl($"https://localhost:{localPort}/", null, urlParameters.Folder)
                : FormatBaseUrl(_options.DashboardUrl, urlParameters.SubDomain, urlParameters.Folder);

            return FormatUrl(baseUrl, urlParameters);
        }

        private string GetLocalPorts(string subDomain)
        {
            if (_options.LocalPorts != null || string.IsNullOrEmpty(subDomain)) { return null; }
            return _options.LocalPorts.TryGetValue(subDomain, out var port)
                ? port.ToString() : null;
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

            return urlString.ToString().TrimEnd(_slash);
        }

        private static string FormatBaseUrl(string url, string subDomain = "", string folder = "")
        {
            var returnUrl = url.EndsWith(_slash.ToString())
                ? url
                : $"{url}{_slash}";

            if (!string.IsNullOrEmpty(subDomain))
            {
                var uri = new Uri(returnUrl);

                var hostparts = uri.Host.Split('.').ToList();

                if (hostparts.Count >= 3)
                {
                    if (!hostparts[0].Contains("-"))
                    {
                        hostparts.RemoveAt(0);
                    }
                    hostparts.Insert(0, subDomain);
                }               

                var updatedhost = string.Join(".", hostparts);

                returnUrl = $"{uri.Scheme}://{updatedhost}/";
            }

            if (!string.IsNullOrEmpty(folder))
            {
                returnUrl = $"{returnUrl}{folder}/";
            }

            return returnUrl;
        }
    }
}