using System.Collections.Generic;

namespace SFA.DAS.Employer.Shared.UI.Configuration
{
    public class MaRoutes
    {
        public static Dictionary<string, string> Accounts => new Dictionary<string,string>
        {
            {"AccountsTeamsView", "/accounts/{0}/teams/view"},
            {"Cookies", "/cookieConsent"},
            {"PrivacyV2", "/service/{0}/privacy"},
            {"AccountsAgreements", "/accounts/{0}/agreements"},
            {"AccountsFinance", "/accounts/{0}/finance"},
            {"AccountsSchemes", "/accounts/{0}/schemes"},
            {"ChangePasswordReturn", "/service/password/change"},
            {"ChangeEmailReturn", "/service/email/change"},
            {"Notifications", "/settings/notifications"},
            {"Accounts", "/service/accounts"},
            {"TermsOfUse", "/service/termsAndConditions/overview"},
            {"AccountsHome", "/accounts/{0}/teams"},
            {"RenameAccount", "/accounts/{0}/rename"},
            {"Privacy", "/service/privacy"},
            {"Help", "/service/help"},
            {"CookiesInAccount", "/accounts/{0}/cookieConsent"}
        };

        public static Dictionary<string, string> CommitmentsV2 => new Dictionary<string,string>
        {
            {"ApprenticesHome", "/{0}"}
        };

        public static Dictionary<string, string> Recruit => new Dictionary<string,string>
        {
            {"NotificationsManage", "/accounts/{0}/notifications-manage"},
            {"RecruitHome", "/accounts/{0}"}
        };
        public static Dictionary<string, string> Finance => new Dictionary<string,string>
        {
            {"AccountsFinance", "/accounts/{0}/finance"}
        };
        public static Dictionary<string, string> Identity => new Dictionary<string, string>
        {
            {"ChangePassword", "/account/changepassword?clientId={0}&returnUrl={1}" },
            {"TermsAndConditions", "/TermsAndConditions" },
            {"ChangeEmailAddress", "/account/changeemail?clientId={0}&returnUrl={1}" }
        };

        public static Dictionary<string, string> EmployerProfile => new Dictionary<string, string>()
        {
            { "ChangeLoginDetails", "/accounts/{0}/user/change-sign-in-details" },
            { "UpdateUserDetails", "/user/add-user-details" }
        };
    }
}