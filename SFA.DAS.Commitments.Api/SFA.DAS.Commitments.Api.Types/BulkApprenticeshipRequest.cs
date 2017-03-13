using System.Collections.Generic;

namespace SFA.DAS.Commitments.Api.Types
{
    public sealed class BulkApprenticeshipRequest
    {
        public IList<Apprenticeship.Apprenticeship> Apprenticeships { get; set; }

        public string UserId { get; set; }
    }
}
