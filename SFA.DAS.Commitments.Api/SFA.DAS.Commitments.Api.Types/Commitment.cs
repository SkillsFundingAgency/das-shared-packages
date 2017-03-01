using System.Collections.Generic;

namespace SFA.DAS.Commitments.Api.Types
{
    public sealed class Commitment
    {
        public Commitment()
        {
            Apprenticeships = new List<Apprenticeship>();
            EmployerLastUpdateInfo = new LastUpdateInfo();
            ProviderLastUpdateInfo = new LastUpdateInfo();
        }

        public long Id { get; set; }
        public string Reference { get; set; }
        public long EmployerAccountId { get; set; }
        public string LegalEntityId { get; set; }
        public string LegalEntityName { get; set; }
        public string LegalEntityAddress { get; set; }
        public OrganisationType LegalEntityOrganisationType { get; set; }
        public long? ProviderId { get; set; }
        public string ProviderName { get; set; }
        public CommitmentStatus CommitmentStatus { get; set; }
        public EditStatus EditStatus { get; set; }
        public List<Apprenticeship> Apprenticeships { get; set; }
        public AgreementStatus AgreementStatus { get; set; }
        public LastAction LastAction { get; set; }
        public bool CanBeApproved { get; set; }
        public LastUpdateInfo EmployerLastUpdateInfo { get; set; }
        public LastUpdateInfo ProviderLastUpdateInfo { get; set; }
    }
}
