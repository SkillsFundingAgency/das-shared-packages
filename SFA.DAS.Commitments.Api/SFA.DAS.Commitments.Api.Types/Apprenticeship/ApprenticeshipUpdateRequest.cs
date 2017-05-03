using SFA.DAS.Commitments.Api.Types.Commitment.Types;

namespace SFA.DAS.Commitments.Api.Types.Apprenticeship
{
    public class ApprenticeshipUpdateRequest
    {
        public ApprenticeshipUpdate ApprenticeshipUpdate { get; set; }
        public string UserId { get; set; }
        public LastUpdateInfo LastUpdatedByInfo { get; set; }
    }
}
