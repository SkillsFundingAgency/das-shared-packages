using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
using System;

namespace SFA.DAS.Commitments.Api.Types.Apprenticeship
{
    public sealed class ApprenticeshipSubmission
    {
        public PaymentStatus PaymentStatus { get; set; }

        public DateTime DateOfChange { get; set; }

        public string UserId { get; set; }
    }
}