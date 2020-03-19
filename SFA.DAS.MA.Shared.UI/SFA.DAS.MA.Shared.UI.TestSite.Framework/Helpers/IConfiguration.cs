namespace SFA.DAS.MA.Shared.UI.TestSite.Framework.Helpers
{
    public interface IConfiguration
    {
        string CdnBaseUrl { get; }
        string EmployerAccountsBaseUrl { get; }
        string EmployerCommitmentsBaseUrl { get; }
        string EmployerFinanceBaseUrl { get; }
        string EmployerRecruitBaseUrl { get; }
        string EmployerPortalBaseUrl { get; }
    }
}