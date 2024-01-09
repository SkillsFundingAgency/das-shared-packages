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

        public string AccountsLink(string path = null)
        {
            return Action(_configuration.AccountsBaseUrl, path);
        }

        public string CommitmentsLink(string path = null)
        {
            return Action(_configuration.CommitmentsBaseUrl, path);
        }

        public string CommitmentsV2Link(string path = null)
        {
            return Action(_configuration.CommitmentsV2BaseUrl, path);
        }

        public string FinanceLink(string path = null)
        {
            return Action(_configuration.FinanceBaseUrl, path);
        }

        public string PortalLink(string path = null)
        {
            return Action(_configuration.PortalBaseUrl, path);
        }

        public string ProjectionsLink(string path = null)
        {
            return Action(_configuration.ProjectionsBaseUrl, path);
        }

        public string RecruitLink(string path = null)
        {
            return Action(_configuration.RecruitBaseUrl, path);
        }

        public string ReservationsLink(string path = null)
        {
            return Action(_configuration.ReservationsBaseUrl, path);
        }

        public string PublicSectorReportingLink(string path = null)
        {
            return Action(_configuration.PublicSectorReportingBaseUrl, path);
        }

        public string UsersLink(string path = null)
        {
            return Action(_configuration.UsersBaseUrl, path);
        }

        public string LevyTransferMatchingLink(string path = null)
        {
            return Action(_configuration.LevyTransferMatchingBaseUrl, path);
        }

        public string ApprenticeshipsLink(string path = null)
        {
            return Action(_configuration.ApprenticeshipsBaseUrl, path);
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