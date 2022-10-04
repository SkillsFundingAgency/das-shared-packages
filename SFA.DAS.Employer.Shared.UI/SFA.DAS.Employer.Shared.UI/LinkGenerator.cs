namespace SFA.DAS.Employer.Shared.UI
{
    internal class LinkGenerator
    {
        private readonly string _environment;

        public LinkGenerator(string environment)
        {
            _environment = environment;
        }
        public string AccountsLink(string route)
        {
            return $"https://accounts.{_environment}-eas.apprenticeships.education.gov.uk{route}";
        }

        public string FinanceLink(string route)
        {
            return $"https://finance.{_environment}-eas.apprenticeships.education.gov.uk{route}";
        }

        public string CommitmentsV2Link(string route)
        {
            return $"https://approvals.{_environment}-eas.apprenticeships.education.gov.uk{route}";
        }

        public string RecruitLink(string route)
        {
            return $"https://recruit.{_environment}-eas.apprenticeships.education.gov.uk{route}";
        }

        public string UsersLink(string route)
        {
            return $"https://{_environment}-login.apprenticeships.education.gov.uk{route}";
        }
    }
}