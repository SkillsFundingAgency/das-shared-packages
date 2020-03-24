namespace SFA.DAS.MA.Shared.UI.TestSite.Framework.Helpers
{
    public class Configuration : IConfiguration
    {
        public string CdnBaseUrl => "https://das-at-frnt-end.azureedge.net";
        public string EmployerAccountsBaseUrl => "https://employerAccount";
        public string EmployerCommitmentsBaseUrl => "https://employerCommitment";
        public string EmployerFinanceBaseUrl => "https://employerFinance";
        public string EmployerRecruitBaseUrl => "https://employerRecruit";
        public string EmployerPortalBaseUrl => "https://employerPortal";
    }
}