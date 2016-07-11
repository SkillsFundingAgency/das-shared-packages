using SFA.DAS.Messaging.Syndication.Hal;

namespace SyndicationHost.Hal
{
    public class PageLinkBuilder : IHalPageLinkBuilder
    {
        public string PreviousPage(int pageNumber)
        {
            return "/hal?page=" + pageNumber;
        }
        public string NextPage(int pageNumber)
        {
            return "/hal?page=" + pageNumber;
        }
        public string FirstPage(int pageNumber)
        {
            return "/hal?page=" + pageNumber;
        }
        public string LastPage(int pageNumber)
        {
            return "/hal?page=" + pageNumber;
        }
    }
}