using System.Threading.Tasks;

using SFA.DAS.Commitments.Api.Types.Apprenticeship;

namespace SFA.DAS.Commitments.Api.Client.Interfaces
{
    public interface IApprenticeshipApi
    {
        Task<PriceHistory> GetPriceHistory(long apprenticeshipId);
    }
}
