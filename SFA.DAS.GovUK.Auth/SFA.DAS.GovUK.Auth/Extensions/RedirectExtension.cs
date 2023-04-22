namespace SFA.DAS.GovUK.Auth.Extensions
{
    public static class RedirectExtension
    {
        public static string GetSignedOutRedirectUrl(this string redirectUri, string environment)
        {
            if (!string.IsNullOrEmpty(redirectUri))
            {
                return redirectUri;
            }
            
            var environmentPart = environment.ToLower() == "prd" ? "manage-apprenticeships" : $"{environment.ToLower()}-eas.apprenticeships";
            var domainPart = environment.ToLower() == "prd" ?  "service" : "education";
            
            return $"https://employerprofiles.{environmentPart}.{domainPart}.gov.uk/service/user-signed-out";
        }

        public static string GetAccountSuspendedRedirectUrl(string environment)
        {
            var environmentPart = environment.ToLower() == "prd" ? "manage-apprenticeships" : $"{environment.ToLower()}-eas.apprenticeships";
            var domainPart = environment.ToLower() == "prd" ?  "service" : "education";
            
            return $"https://employerprofiles.{environmentPart}.{domainPart}.gov.uk/service/account-unavailable";
        }
    
        public static string GetStubSignInRedirectUrl(string environment)
        {
            if (environment.ToLower() == "local" || environment.ToLower() == "prd")
            {
                return string.Empty;
            }
            
            return $"https://employerprofiles.{environment.ToLower()}-eas.apprenticeships.education.gov.uk/service/account-details";
        }
    }    
}

