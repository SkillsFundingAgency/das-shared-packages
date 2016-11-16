using System;
using System.Collections.Generic;

namespace SFA.DAS.Commitments.Api.Types
{
    public class Commitment
    {
        public Commitment()
        {
            Apprenticeships = new List<Apprenticeship>();
        }

        public long Id { get; set; }
        public string Reference { get; set; }
        public long EmployerAccountId { get; set; }
        public string EmployerAccountName { get; set; }
        public string LegalEntityId { get; set; }
        public string LegalEntityName { get; set; }
        public long? ProviderId { get; set; }
        public string ProviderName { get; set; }
        public CommitmentStatus CommitmentStatus { get; set; }
        public EditStatus EditStatus { get; set; }

        public List<Apprenticeship> Apprenticeships { get; set; }
    }
}
