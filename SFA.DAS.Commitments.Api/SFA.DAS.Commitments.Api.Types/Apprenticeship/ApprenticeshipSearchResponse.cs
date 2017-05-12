using System.Collections.Generic;

namespace SFA.DAS.Commitments.Api.Types.Apprenticeship
{
    public sealed class ApprenticeshipSearchResponse
    {
        public IEnumerable<Apprenticeship> Apprenticeships { get; set; }

        public Facets Facets { get; set; }
    }
}