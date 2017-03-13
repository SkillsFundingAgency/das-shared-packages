using SFA.DAS.Commitments.Api.Types.Commitment.Types;

namespace SFA.DAS.Commitments.Api.Types
{
    public sealed class CommitmentSubmission
    {
        public LastAction Action { get; set; }

        public LastUpdateInfo LastUpdatedByInfo { get; set; }

        public string UserId { get; set; }
    }
}
