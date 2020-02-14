using System;
using SFA.DAS.Authorization.Services;

namespace SFA.DAS.MA.Shared.UI.Configuration
{
    public class HeaderConfiguration : IHeaderConfiguration
    {
        public string ManageApprenticeshipsBaseUrl { get; set; }
        public string ApplicationBaseUrl { get; set; }
        public string EmployerCommitmentsBaseUrl { get; set; }
        public string EmployerFinanceBaseUrl { get; set; }        
        public string AuthenticationAuthorityUrl { get; set; }
        public string ClientId { get; set; }
        public string EmployerRecruitBaseUrl { get; set; }
        public IAuthorizationService AuthorizationService { get; set; }
        public Uri SignOutUrl { get; set; }
        public Uri ChangeEmailReturnUrl { get; set; }
        public Uri ChangePasswordReturnUrl { get; set; }
    }
}
