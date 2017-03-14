using SFA.DAS.Commitments.Api.Types.Validation.Types;

namespace SFA.DAS.Commitments.Api.Types.Validation
{
    public class OverlappingApprenticeship
    {
        public Apprenticeship.Apprenticeship Apprenticeship { get; set; }

        public long EmployerAccountId { get; set; }

        public long ProviderId { get; set; }

        public string ProviderName { get; set; }

        public string LegalEntityName { get; set; }

        public ValidationFailReason ValidationFailReason { get; set; }
    }
}
