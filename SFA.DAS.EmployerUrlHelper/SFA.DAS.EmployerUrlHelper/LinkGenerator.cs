using System;
using SFA.DAS.AutoConfiguration;
using SFA.DAS.EmployerUrlHelper.Configuration;

namespace SFA.DAS.EmployerUrlHelper
{
    public class LinkGenerator : ILinkGenerator
    {
        private readonly Lazy<EmployerUrlHelperConfiguration> _configuration;

        public LinkGenerator(IAutoConfigurationService autoConfigurationService)
        {
            _configuration = new Lazy<EmployerUrlHelperConfiguration>(autoConfigurationService.Get<EmployerUrlHelperConfiguration>);
        }

        public string AccountsLink(string path)
        {
            return Action(_configuration.Value.AccountsBaseUrl, path);
        }

        public string CommitmentsLink(string path)
        {
            return Action(_configuration.Value.CommitmentsBaseUrl, path);
        }

        public string CommitmentsV2Link(string path)
        {
            return Action(_configuration.Value.CommitmentsV2BaseUrl, path);
        }

        public string PortalLink(string path)
        {
            return Action(_configuration.Value.PortalBaseUrl, path);
        }

        public string ProjectionsLink(string path)
        {
            return Action(_configuration.Value.ProjectionsBaseUrl, path);
        }

        public string RecruitLink(string path)
        {
            return Action(_configuration.Value.RecruitBaseUrl, path);
        }

        public string UsersLink(string path)
        {
            return Action(_configuration.Value.UsersBaseUrl, path);
        }

        private string Action(string baseUrl, string path)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                throw new ArgumentException("Value cannot be null or white space", nameof(baseUrl));
            }

            var trimmedBaseUrl = baseUrl.TrimEnd('/');
            var trimmedPath = path?.Trim('/');
            var trimmedUrl = $"{trimmedBaseUrl}/{trimmedPath}".TrimEnd('/');

            return trimmedUrl;
        }
    }
}