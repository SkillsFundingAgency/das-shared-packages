using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using SFA.DAS.Commitments.Api.Types.Apprenticeship;

namespace SFA.DAS.Commitments.Api.Client.Interfaces
{
    public interface IApprenticeshipApi
    {
        [Obsolete("Moved to provider / employer API")]
        Task<IEnumerable<PriceHistory>> GetPriceHistory(long apprenticeshipId);
    }
}
