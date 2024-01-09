namespace SFA.DAS.Employer.Shared.UI
{
    internal class LinkGenerator
    {
        private readonly string _environment;
        private readonly string _environmentUsers;
        private readonly string _domainPart;

        public LinkGenerator(string environment)
        {
            _environment = environment.ToLower() == "prd" ? "manage-apprenticeships" : $"{environment.ToLower()}-eas.apprenticeships";
            _environmentUsers = environment.ToLower() == "prd" ? "beta" : environment.ToLower();
            _domainPart = environment.ToLower() == "prd" ?  "service" : "education";
        }
        public string AccountsLink(string route)
        {
            return $"https://accounts.{_environment}.{_domainPart}.gov.uk{route}";
        }

        public string FinanceLink(string route)
        {
            return $"https://finance.{_environment}.{_domainPart}.gov.uk{route}";
        }

        public string CommitmentsV2Link(string route)
        {
            return $"https://approvals.{_environment}.{_domainPart}.gov.uk{route}";
        }

        public string RecruitLink(string route)
        {
            return $"https://recruit.{_environment}.{_domainPart}.gov.uk{route}";
        }

        public string UsersLink(string route)
        {
            return $"https://{_environmentUsers}-login.apprenticeships.education.gov.uk{route}";
        }

        public string EmployersProfiles(string route)
        {
            return $"https://employerprofiles.{_environment}.{_domainPart}.gov.uk{route}";
        }

        public string ApprenticeshipsLink(string route)
        {
            return $"https://apprenticeshipdetails.{_environment}.{_domainPart}.gov.uk{route}";
        }
    }
}