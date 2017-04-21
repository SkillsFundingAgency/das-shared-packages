using System.Collections.Generic;

using SFA.DAS.Commitments.Api.Types.Commitment.Types;

namespace SFA.DAS.Commitments.Api.Types.Commitment
{
    public sealed class CommitmentView
    {
        public CommitmentView()
        {
            Apprenticeships = new List<Apprenticeship.Apprenticeship>();
            EmployerLastUpdateInfo = new LastUpdateInfo();
            ProviderLastUpdateInfo = new LastUpdateInfo();
        }

        public long Id { get; set; }
        public string Reference { get; set; }
        public long EmployerAccountId { get; set; }
        public string LegalEntityName { get; set; }
        public long? ProviderId { get; set; }
        public string ProviderName { get; set; }
        public EditStatus EditStatus { get; set; }
        public List<Apprenticeship.Apprenticeship> Apprenticeships { get; set; }
        public AgreementStatus AgreementStatus { get; set; }
        public LastAction LastAction { get; set; }
        public bool CanBeApproved { get; set; }
        public LastUpdateInfo EmployerLastUpdateInfo { get; set; }
        public LastUpdateInfo ProviderLastUpdateInfo { get; set; }
        public List<MessageView> Messages { get; set; }
    }
}
