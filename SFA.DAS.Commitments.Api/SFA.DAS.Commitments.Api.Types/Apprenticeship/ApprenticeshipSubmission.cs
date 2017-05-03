using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
using System;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;

namespace SFA.DAS.Commitments.Api.Types.Apprenticeship
{
    public sealed class ApprenticeshipSubmission
    {
        public PaymentStatus PaymentStatus { get; set; }
        public DateTime DateOfChange { get; set; }
        public string UserId { get; set; }
        public LastUpdateInfo LastUpdatedByInfo { get; set; }
    }
}