using System;

namespace SFA.DAS.EmployerUrlHelper
{
    public class LinkGenerator : ILinkGenerator
    {
        private readonly EmployerUrlConfiguration _options;

        public LinkGenerator(EmployerUrlConfiguration options)
        {
            _options = options;
        }

        #region Accounts
        
        public string Account(string accountHashedId) => Accounts("teams", accountHashedId);
        public string NotificationSettings() => Accounts("settings/notifications");
        public string PayeSchemes(string accountHashedId) => Accounts("schemes", accountHashedId);
        public string RenameAccount(string accountHashedId) => Accounts("rename", accountHashedId);
        public string YourAccounts() => Accounts("service/accounts");
        public string YourOrganisationsAndAgreements(string accountHashedId) => Accounts("agreements", accountHashedId);
        public string YourTeam(string accountHashedId) => Accounts("teams/view", accountHashedId);

        private string Accounts(string path) => Action(_options.AccountsBaseUrl, path);
        private string Accounts(string path, string accountHashedId) => AccountAction(_options.AccountsBaseUrl, path, accountHashedId);
        
        #endregion Accounts
        
        #region Commitments

        public string Apprentices(string accountHashedId) => Commitments("apprentices/home", accountHashedId);
        public string CohortDetails(string accountHashedId, string commitmentHashedId) => Commitments($"apprentices/{commitmentHashedId}/details", accountHashedId);
        
        private string Commitments(string path, string accountHashedId) => AccountAction(_options.CommitmentsBaseUrl, path, accountHashedId);

        #endregion Commitments

        #region Portal

        public string Help() => Portal("service/help");
        public string Homepage() => Portal(null);
        public string Privacy() => Portal("service/privacy");

        private string Portal(string path) => Action(_options.PortalBaseUrl, path);

        #endregion Portal
        
        #region Recruit
        
        public string Recruit(string accountHashedId) => Recruit(null, accountHashedId);
        
        private string Recruit(string path, string accountHashedId) => AccountAction(_options.RecruitBaseUrl, path, accountHashedId);
        
        #endregion Recruit
        
        private string AccountAction(string baseUrl, string path, string accountHashedId)
        {
            if (string.IsNullOrWhiteSpace(accountHashedId))
            {
                throw new ArgumentException($"Value cannot be null or white space", nameof(accountHashedId));
            }
            
            return Action(baseUrl, $"accounts/{accountHashedId}/{path}");
        }

        private string Action(string baseUrl, string path)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                throw new ArgumentException("Value cannot be null or white space", nameof(baseUrl));
            }

            var trimmedBaseUrl = baseUrl.TrimEnd('/');
            var trimmedPath = path?.TrimEnd('/');
            var trimmedUrl = $"{trimmedBaseUrl}/{trimmedPath}".TrimEnd('/');

            return trimmedUrl;
        }
    }
}