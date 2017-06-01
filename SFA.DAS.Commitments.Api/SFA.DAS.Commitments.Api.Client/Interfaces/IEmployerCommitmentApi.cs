using System.Collections.Generic;
using System.Threading.Tasks;

using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.ProviderPayment;

namespace SFA.DAS.Commitments.Api.Client.Interfaces
{
    public interface IEmployerCommitmentApi
    {
        Task<List<CommitmentListItem>> GetEmployerCommitments(long employerAccountId);
        Task<CommitmentView> GetEmployerCommitment(long employerAccountId, long commitmentId);
        Task<List<Apprenticeship>> GetEmployerApprenticeships(long employerAccountId);
        Task<ApprenticeshipSearchResponse> GetEmployerApprenticeships(long employerAccountId, ApprenticeshipSearchQuery apprenticeshipSearchQuery);
        Task<Apprenticeship> GetEmployerApprenticeship(long employerAccountId, long apprenticeshipId);
        Task<List<ApprenticeshipStatusSummary>> GetEmployerAccountSummary(long employerAccountId);

        Task<CommitmentView> CreateEmployerCommitment(long employerAccountId, CommitmentRequest commitment);
        Task PatchEmployerCommitment(long employerAccountId, long commitmentId, CommitmentSubmission submission);
        Task DeleteEmployerCommitment(long employerAccountId, long commitmentId, DeleteRequest deleteRequest);

        Task CreateEmployerApprenticeship(long employerAccountId, long commitmentId, ApprenticeshipRequest apprenticeship);
        Task UpdateEmployerApprenticeship(long employerAccountId, long commitmentId, long apprenticeshipId, ApprenticeshipRequest apprenticeship);
        Task PatchEmployerApprenticeship(long employerAccountId, long apprenticeshipId, ApprenticeshipSubmission apprenticeshipSubmission);
        Task DeleteEmployerApprenticeship(long employerAccountId, long apprenticeshipId, DeleteRequest deleteRequest);

        Task CreateApprenticeshipUpdate(long employerAccountId, long apprenticeshipId, ApprenticeshipUpdateRequest apprenticeshipUpdateRequest);
        Task<ApprenticeshipUpdate> GetPendingApprenticeshipUpdate(long employerAccountId, long apprenticeshipId);
        Task PatchApprenticeshipUpdate(long employerAccountId, long apprenticeshipId, ApprenticeshipUpdateSubmission submission);

        Task<IList<ProviderPaymentPriorityItem>> GetCustomProviderPaymentPriority(long employerAccountId);
        Task UpdateCustomProviderPaymentPriority(long employerAccountId, ProviderPaymentPrioritySubmission submission);
    }
}