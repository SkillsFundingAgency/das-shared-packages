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

            var apprenticeshipsEducationGovUk = "apprenticeships.education.gov.uk";
            if (clientName == ClientName.RoatpServiceAdmin || clientName == ClientName.ServiceAdmin)
            {
                return apprenticeshipsEducationGovUk;
            }
            apprenticeshipsEducationGovUk = $".{apprenticeshipsEducationGovUk}";
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

            if (clientName == ClientName.ServiceAdminAan)
            {
                return environment.ToLower() == "prd"
                    ? $"admin-aan{apprenticeshipsEducationGovUk}"
                    : $"{environment.ToLower()}-{clientName.GetDescription()}{apprenticeshipsEducationGovUk}";
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
            
            if (clientName == ClientName.RoatpServiceAdmin || clientName == ClientName.ServiceAdmin)
            {
                environmentAndDomain = environment.ToLower() == "prd"
                    ? $"{ClientName.ServiceAdmin.GetDescription()}.apprenticeships.education.gov.uk"
                    : $"{environment.ToLower()}-{ClientName.ServiceAdmin.GetDescription()}.apprenticeships.education.gov.uk";
            }

            
            return string.IsNullOrEmpty(environmentAndDomain) ? "/" : $"https://{environmentAndDomain}";
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

            return $"https://{environment.ToLower()}-pas.apprenticeships.education.gov.uk";
        }

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
            var environmentPart = environment.ToLower() == "prd" ? "providers" : $"{environment.ToLower()}-pas.apprenticeships";
            var domainPart = environment.ToLower() == "prd" ? "service" : "education";

            return $"{environmentPart}.{domainPart}.gov.uk";
        }
    }
}