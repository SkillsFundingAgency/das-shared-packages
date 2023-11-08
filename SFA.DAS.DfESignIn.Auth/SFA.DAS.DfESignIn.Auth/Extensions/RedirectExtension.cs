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
            if (clientName == ClientName.ProviderRoatp || clientName == ClientName.TraineeshipRoatp)
            {
                if (clientName == ClientName.TraineeshipRoatp)
                {
                    apprenticeshipsEducationGovUk = ".traineeships.education.gov.uk";
                }
                return environment.ToLower() == "prd"
                    ? $"{ClientName.ProviderRoatp.GetDescription().Split('|')[0]}{apprenticeshipsEducationGovUk}"
                    : $"{environment.ToLower()}-{ClientName.ProviderRoatp.GetDescription().Split('|')[1]}{apprenticeshipsEducationGovUk}";
            }

            if (clientName == ClientName.EmployerAccountsStaff)
            {
                var environmentPart = environment.ToLower() == "prd" ? "manage-apprenticeships" : $"{environment.ToLower()}-eas.apprenticeships";
                var domainPart = environment.ToLower() == "prd" ?  "service" : "education";

                return $"accounts.{environmentPart}.{domainPart}.gov.uk";
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