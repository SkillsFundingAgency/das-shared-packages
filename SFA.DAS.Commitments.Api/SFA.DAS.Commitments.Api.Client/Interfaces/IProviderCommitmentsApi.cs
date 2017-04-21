using System.Collections.Generic;
using System.Threading.Tasks;

using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Commitment;

namespace SFA.DAS.Commitments.Api.Client.Interfaces
{
    public interface IProviderCommitmentsApi
    {
        Task<List<CommitmentListItem>> GetProviderCommitments(long providerId);
        Task<CommitmentView> GetProviderCommitment(long providerId, long commitmentId);
        Task<List<Apprenticeship>> GetProviderApprenticeships(long providerId);
        Task<Apprenticeship> GetProviderApprenticeship(long providerId, long apprenticeshipId);

        Task PatchProviderCommitment(long providerId, long commitmentId, CommitmentSubmission submission);
        Task DeleteProviderCommitment(long providerId, long commitmentId, DeleteRequest deleteRequest);

        Task CreateProviderApprenticeship(long providerId, long commitmentId, ApprenticeshipRequest apprenticeship);
        Task UpdateProviderApprenticeship(long providerId, long commitmentId, long apprenticeshipId, ApprenticeshipRequest apprenticeship);
        Task BulkUploadApprenticeships(long providerId, long commitmentId, BulkApprenticeshipRequest bulkRequest);
        Task DeleteProviderApprenticeship(long providerId, long apprenticeshipId, DeleteRequest deleteRequest);

        Task CreateApprenticeshipUpdate(long providerId, long apprenticeshipId, ApprenticeshipUpdateRequest apprenticeshipUpdateRequest);
        Task<ApprenticeshipUpdate> GetPendingApprenticeshipUpdate(long providerId, long apprenticeshipId);
        Task PatchApprenticeshipUpdate(long providerId, long apprenticeshipId, ApprenticeshipUpdateSubmission submission);
    }
}
