using System;
using SFA.DAS.AutoConfiguration;
using SFA.DAS.EmployerUrlHelper.Configuration;

namespace SFA.DAS.EmployerUrlHelper
{
    public class LinkGenerator : ILinkGenerator
    {
        private readonly EmployerUrlHelperConfiguration _configuration;

        public LinkGenerator(EmployerUrlHelperConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string AccountsLink(string path)
        {
            return Action(_configuration.AccountsBaseUrl, path);
        }

        public string CommitmentsLink(string path)
        {
            return Action(_configuration.CommitmentsBaseUrl, path);
        }

        public string CommitmentsV2Link(string path)
        {
            return Action(_configuration.CommitmentsV2BaseUrl, path);
        }

        public string PortalLink(string path)
        {
            return Action(_configuration.PortalBaseUrl, path);
        }

        public string ProjectionsLink(string path)
        {
            return Action(_configuration.ProjectionsBaseUrl, path);
        }

        public string RecruitLink(string path)
        {
            return Action(_configuration.RecruitBaseUrl, path);
        }

        public string UsersLink(string path)
        {
            return Action(_configuration.UsersBaseUrl, path);
        }

        public string FinanceLink(string path)
        {
            return Action(_configuration.FinanceBaseUrl, path);
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