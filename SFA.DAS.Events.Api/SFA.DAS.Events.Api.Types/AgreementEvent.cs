using System;

namespace SFA.DAS.Events.Api.Types
{
    public class AgreementEvent
    {
        public string Event { get; set; }
        public string ProviderId { get; set; }
        public string ContractType { get; set; }
    }
}
