using System;

namespace SFA.DAS.Commitments.Api.Types.Validation
{
    public class ApprenticeshipOverlapValidationRequest
    {
        public long? ApprenticeshipId { get; set; }

        public string Uln { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}