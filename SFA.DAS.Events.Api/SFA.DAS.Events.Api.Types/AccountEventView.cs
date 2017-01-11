using System;

namespace SFA.DAS.Events.Api.Types
{
    public class AccountEventView
    {
        public long Id { get; set; }
        public string Event { get; set; }
        public DateTime CreatedOn { get; set; }
        public string DasAccountId { get; set; }
    }
}
