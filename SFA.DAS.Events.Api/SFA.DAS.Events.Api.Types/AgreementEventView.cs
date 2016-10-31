using System;

namespace SFA.DAS.Events.Api.Types
{
    public class AgreementEventView
    {
        public long Id { get; set; }
        public string Event { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ProviderId { get; set; }
    }
}
