using System;

namespace SFA.DAS.Events.Api.Types
{
    public class PriceHistory
    {
        public decimal TotalCost { get; set; }

        public DateTime EffectiveFrom { get; set; }

        public DateTime? EffectiveTo { get; set; }
    }
}
