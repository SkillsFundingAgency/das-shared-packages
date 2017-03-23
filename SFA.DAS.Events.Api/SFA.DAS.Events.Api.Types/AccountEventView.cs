using System;

namespace SFA.DAS.Events.Api.Types
{
    public class AccountEventView : IEventView
    {
        public long Id { get; set; }
        public string Event { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ResourceUri { get; set; }
        public string Type => this.GetType().Name;
    }
}
