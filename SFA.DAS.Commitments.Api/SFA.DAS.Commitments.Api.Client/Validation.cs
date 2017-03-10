using System.Collections.Generic;
using System.Threading.Tasks;

using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Validation;

namespace SFA.DAS.Commitments.Api.Client
{
    public class Validation : IValidation
    {
        public Task<OverlappingApprenticeship> ValidateOverlapping(ApprenticeshipOverlapValidationRequest apprenticeshipOverlapValidation)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<OverlappingApprenticeship>> ValidateOverlapping(IEnumerable<ApprenticeshipOverlapValidationRequest> apprenticeshipOverlapValidation)
        {
            throw new System.NotImplementedException();
        }
    }
}