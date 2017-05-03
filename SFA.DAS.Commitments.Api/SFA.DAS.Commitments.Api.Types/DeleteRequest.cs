using SFA.DAS.Commitments.Api.Types.Commitment.Types;

namespace SFA.DAS.Commitments.Api.Types
{
    public class DeleteRequest
    {
        public string UserId { get; set; }
        public LastUpdateInfo LastUpdatedByInfo { get; set; }
    }
}