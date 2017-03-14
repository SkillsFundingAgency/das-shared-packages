using System.Collections.Generic;
using System.Threading.Tasks;

using SFA.DAS.Commitments.Api.Types.Validation;

namespace SFA.DAS.Commitments.Api.Client.Interfaces
{
    public interface IValidationApi
    {
        Task<ApprenticeshipOverlapValidationResult> ValidateOverlapping(ApprenticeshipOverlapValidationRequest apprenticeshipOverlapValidation);

        Task<IEnumerable<ApprenticeshipOverlapValidationResult>> ValidateOverlapping(IEnumerable<ApprenticeshipOverlapValidationRequest> apprenticeshipOverlapValidation);
    }
}
