using System;

namespace SFA.DAS.Commitments.Api.Types.Apprenticeship
{
    public class PriceHistory
    {
        public long ApprenticeshipId { get; set; }

        public decimal Cost { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime? ToDate { get; set; }
    }
}
