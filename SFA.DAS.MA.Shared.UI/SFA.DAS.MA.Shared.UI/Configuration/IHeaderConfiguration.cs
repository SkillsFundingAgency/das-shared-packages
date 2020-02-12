
namespace SFA.DAS.MA.Shared.UI.Configuration
{
    public interface IHeaderConfiguration
    {
        string EmployerCommitmentsBaseUrl { get; set; }
        string EmployerFinanceBaseUrl { get; set; }
        string EmployerAccountsBaseUrl { get; set; }
        string IdentityServerBaseUrl { get; set; }
        string EmployerRecruitBaseUrl { get; set; }
        string ClientId { get; set; }
        Authorization.Services.IAuthorizationService AuthorizationService { get; set; }
    }
}
