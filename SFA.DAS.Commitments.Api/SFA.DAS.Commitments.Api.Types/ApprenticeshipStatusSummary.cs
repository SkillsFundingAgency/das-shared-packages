using System;

namespace SFA.DAS.Commitments.Api.Types
{
    public sealed class ApprenticeshipStatusSummary
    {
        public string LegalEntityIdentifier { get; set; }

        public int PendingApprovalCount { get; set; }
        public int ActiveCount { get; set; }
        public int PausedCount { get; set; }
        public int WithdrawnCount { get; set; }
        public int CompletedCount { get; set; }
        public int DeletedCount { get; set; }
    }
}
