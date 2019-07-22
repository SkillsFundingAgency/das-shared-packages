namespace SFA.DAS.EmployerUrlHelper
{
    public interface ILinkGenerator
    {
        string AccountsLink(string path);
        string CommitmentsLink(string path);
        string CommitmentsV2Link(string path);
        string PortalLink(string path);
        string ProjectionsLink(string path);
        string RecruitLink(string path);
        string UsersLink(string path);
    }
}