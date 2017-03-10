using System.Collections.Generic;
using System.Threading.Tasks;

using SFA.DAS.Commitments.Api.Types.Validation;

namespace SFA.DAS.Commitments.Api.Client.Interfaces
{
    public interface IValidation
    {
        Task<OverlappingApprenticeship> ValidateOverlapping(ApprenticeshipOverlapValidationRequest apprenticeshipOverlapValidation);

        Task<IEnumerable<OverlappingApprenticeship>> ValidateOverlapping(IEnumerable<ApprenticeshipOverlapValidationRequest> apprenticeshipOverlapValidation);
    }
}
