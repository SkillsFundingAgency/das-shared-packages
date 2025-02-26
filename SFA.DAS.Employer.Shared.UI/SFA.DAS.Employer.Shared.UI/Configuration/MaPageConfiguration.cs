namespace SFA.DAS.Employer.Shared.UI.Configuration
{
    public class MaPageConfiguration
    {
        public MaPageConfiguration(string accountsOidcClientId, string localLogoutRouteName)
        {
            LocalLogoutRouteName = localLogoutRouteName;
            AccountsOidcClientId = accountsOidcClientId;
        }
        public string AccountsOidcClientId { get; }
        public string LocalLogoutRouteName { get; }
    }
}