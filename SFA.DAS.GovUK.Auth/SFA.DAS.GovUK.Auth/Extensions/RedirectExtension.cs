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
    
    }    
}

