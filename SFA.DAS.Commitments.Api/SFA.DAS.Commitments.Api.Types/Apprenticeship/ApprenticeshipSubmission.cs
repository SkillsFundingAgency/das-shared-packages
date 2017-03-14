using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;

namespace SFA.DAS.Commitments.Api.Types.Apprenticeship
{
    public sealed class ApprenticeshipSubmission
    {
        public PaymentStatus PaymentStatus { get; set; }

        public string UserId { get; set; }
    }
}