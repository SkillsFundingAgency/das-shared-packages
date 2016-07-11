namespace SFA.DAS.Messaging.Syndication.Hal
{
    public interface IHalPageLinkBuilder
    {
        string PreviousPage(int pageNumber);
        string NextPage(int pageNumber);
        string FirstPage(int pageNumber);
        string LastPage(int pageNumber);
    }
}
