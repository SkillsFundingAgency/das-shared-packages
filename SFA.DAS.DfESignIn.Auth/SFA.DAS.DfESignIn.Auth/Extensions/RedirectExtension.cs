namespace SFA.DAS.DfESignIn.Auth.Extensions
{
    public static class RedirectExtension
    {
        public static string GetEnvironmentAndDomain(string environment)
        {
            if (environment.ToLower() == "local")
            {
                return "";
            }

            var environmentPart = environment.ToLower() == "prd"
                ? "providers.apprenticeships"
                : $"{environment.ToLower()}-pas.apprenticeships";

            return $"{environmentPart}.education.gov.uk";
        }
        public static string GetSignedOutRedirectUrl(this string redirectUri, string environment)
        {
            if (!string.IsNullOrEmpty(redirectUri))
            {
                return redirectUri;
            }

            var environmentAndDomain = GetEnvironmentAndDomain(environment);
            return string.IsNullOrEmpty(environmentAndDomain) ? "/" : $"https://{environmentAndDomain}";
        }
    }
}