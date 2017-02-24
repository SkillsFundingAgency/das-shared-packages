using System;

namespace SFA.DAS.Commitments.Api.Types.History
{
    public class CommitmentHistoryItem
    {
        public long CommitmentId { get; set; }

        public long UserId { get; set; }

        public UserRole UpdatedByRole { get; set; }

        public CommitmentChangeType ChangeType { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}