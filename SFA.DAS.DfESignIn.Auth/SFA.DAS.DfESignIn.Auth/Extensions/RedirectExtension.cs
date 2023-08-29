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
    }
}