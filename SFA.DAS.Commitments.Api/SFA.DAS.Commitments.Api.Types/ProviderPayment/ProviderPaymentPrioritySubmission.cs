using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using System.Collections.Generic;

namespace SFA.DAS.Commitments.Api.Types.ProviderPayment
{
    public sealed class ProviderPaymentPrioritySubmission
    {
        public IList<ProviderPaymentPriorityUpdateItem> Priorities { get; set; }
        public string UserId { get; set; }
        public LastUpdateInfo LastUpdatedByInfo { get; set; }
    }
}