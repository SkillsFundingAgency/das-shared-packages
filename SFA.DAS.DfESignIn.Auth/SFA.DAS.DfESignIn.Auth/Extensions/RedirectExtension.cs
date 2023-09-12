using SFA.DAS.DfESignIn.Auth.Enums;

namespace SFA.DAS.DfESignIn.Auth.Extensions
{
    public static class RedirectExtension
    {
        public static string GetEnvironmentAndDomain(string environment, ClientName clientName)
        {
            if (environment.ToLower() == "local")
            {
                return "";
            }

            var apprenticeshipsEducationGovUk = ".apprenticeships.education.gov.uk";
            if (clientName == ClientName.ProviderRoatp)
            {
                return environment.ToLower() == "prd"
                    ? $"{ClientName.ProviderRoatp.GetDescription().Split('|')[0]}{apprenticeshipsEducationGovUk}"
                    : $"{environment.ToLower()}-{ClientName.ProviderRoatp.GetDescription().Split('|')[1]}{apprenticeshipsEducationGovUk}";
            }

            return environment.ToLower() == "prd"
                ? $"{clientName.GetDescription()}{apprenticeshipsEducationGovUk}"
                : $"{environment.ToLower()}-{clientName.GetDescription()}{apprenticeshipsEducationGovUk}";


        }
        public static string GetSignedOutRedirectUrl(this string redirectUri, string environment, ClientName clientName)
        {
            if (!string.IsNullOrEmpty(redirectUri))
            {
                return redirectUri;
            }

            var environmentAndDomain = GetEnvironmentAndDomain(environment, clientName);
            return string.IsNullOrEmpty(environmentAndDomain) ? "/" : $"https://{environmentAndDomain}";
        }
    }
}