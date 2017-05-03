using System.Collections.Generic;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;

namespace SFA.DAS.Commitments.Api.Types
{
    public sealed class BulkApprenticeshipRequest
    {
        public IList<Apprenticeship.Apprenticeship> Apprenticeships { get; set; }
        public string UserId { get; set; }
        public LastUpdateInfo LastUpdatedByInfo { get; set; }
    }
}
