using System;

namespace SFA.DAS.Commitments.Api.Types.History
{
    public class CommitmentHistoryItem
    {
        public long CommitmentId { get; set; }

        public long UpdaterId { get; set; }

        public UserRole UpdatedByRole { get; set; }

        public CommitmentChangeType ChangeType { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}