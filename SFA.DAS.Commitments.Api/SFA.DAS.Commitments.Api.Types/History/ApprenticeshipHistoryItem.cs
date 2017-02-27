using System;

namespace SFA.DAS.Commitments.Api.Types.History
{
    public class ApprenticeshipHistoryItem
    {
        public long ApprenticeshipId { get; set; }

        public long UserId { get; set; }

        public UserRole UpdatedByRole { get; set; }

        public ApprenticeshipChangeType ChangeType { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}