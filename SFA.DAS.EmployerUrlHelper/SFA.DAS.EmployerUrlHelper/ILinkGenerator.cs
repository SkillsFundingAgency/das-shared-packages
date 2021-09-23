namespace SFA.DAS.EmployerUrlHelper
{
    public interface ILinkGenerator
    {
        string AccountsLink(string path = null);
        string CommitmentsLink(string path = null);
        string CommitmentsV2Link(string path = null);
        string FinanceLink(string path = null);
        string PortalLink(string path = null);
        string ProjectionsLink(string path = null);
        string RecruitLink(string path = null);
        string ReservationsLink(string path = null);
        string PublicSectorReportingLink(string path = null);
        string UsersLink(string path = null);
        string LevyTransferMatchingLink(string path = null);
    }
}