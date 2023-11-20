namespace SFA.DAS.GovUK.Auth.Extensions
{
    public static class RedirectExtension
    {
        public static string GetEnvironmentAndDomain(this string redirectUri, string environment)
        {
            if (environment.ToLower() == "local")
            {
                return "";
            }
            if (!string.IsNullOrEmpty(redirectUri))
            {
                return redirectUri;
            }
            var environmentPart = environment.ToLower() == "prd" ? "manage-apprenticeships" : $"{environment.ToLower()}-eas.apprenticeships";
            var domainPart = environment.ToLower() == "prd" ?  "service" : "education";

            return $"{environmentPart}.{domainPart}.gov.uk";
        }
        
        public static string GetSignedOutRedirectUrl(this string redirectUri, string environment)
        {
            if (!string.IsNullOrEmpty(redirectUri))
            {
                return redirectUri;
            }
            
            return $"https://employerprofiles.{"".GetEnvironmentAndDomain(environment)}/service/user-signed-out";
        }

        public static string GetAccountSuspendedRedirectUrl(string environment)
        {   
            return $"https://employerprofiles.{"".GetEnvironmentAndDomain(environment)}/service/account-unavailable";
        }
    
        public static string GetStubSignInRedirectUrl(this string redirectUrl, string environment)
        {
            if (environment.ToLower() == "local" || environment.ToLower() == "prd")
            {
                return string.Empty;
            }

            if (!string.IsNullOrEmpty(redirectUrl))
            {
                return redirectUrl;
            }
            
            return $"https://employerprofiles.{environment.ToLower()}-eas.apprenticeships.education.gov.uk/service/account-details";
        }
    }    
}

