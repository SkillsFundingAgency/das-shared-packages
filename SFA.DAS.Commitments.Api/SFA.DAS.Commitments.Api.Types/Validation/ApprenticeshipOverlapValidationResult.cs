using System.Collections.Generic;

namespace SFA.DAS.Commitments.Api.Types.Validation
{
    public class ApprenticeshipOverlapValidationResult
    {
        public ApprenticeshipOverlapValidationRequest Self { get; set; }

        public IEnumerable<OverlappingApprenticeship> OverlappingApprenticeships { get; set; }
    }
}