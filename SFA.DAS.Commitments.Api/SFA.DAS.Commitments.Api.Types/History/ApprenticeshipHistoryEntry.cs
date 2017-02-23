using System;

namespace SFA.DAS.Commitments.Api.Types.History
{
    public class ApprenticeshipHistoryItem
    {
        public long CommitmentId { get; set; }

        public long ApprenticeshipId { get; set; }

        public long UpdaterId { get; set; }

        public UserRole UpdatedByRole { get; set; }

        public ApprenticeshipChangeType ChangeType { get; set; }

        public PaymentStatus Status { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}