using System.Collections.Generic;

namespace SFA.DAS.Messaging.Syndication
{
    public class SyndicationPage<T>
    {
        public IEnumerable<T> Messages { get; set; }
        public int PageNumber { get; set; }
        public long TotalNumberOfMessages { get; set; }
    }
}
