using System.Collections.Generic;

namespace SFA.DAS.Commitments.Api.Types.Apprenticeship
{
    public sealed class ApprenticeshipSearchResponse
    {
        public IEnumerable<Apprenticeship> Apprenticeships { get; set; }

        public Facets Facets { get; set; }

        public int TotalApprenticeships { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public int TotalPages => (TotalApprenticeships + PageSize - 1) / PageSize;
    }
}