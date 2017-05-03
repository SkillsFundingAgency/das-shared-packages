using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;

namespace SFA.DAS.Commitments.Api.Types.Apprenticeship
{
    public sealed class ApprenticeshipUpdateSubmission
    {
        public ApprenticeshipUpdateStatus UpdateStatus { get; set; }
        public string UserId { get; set; }
        public LastUpdateInfo LastUpdatedByInfo { get; set; }
    }
}