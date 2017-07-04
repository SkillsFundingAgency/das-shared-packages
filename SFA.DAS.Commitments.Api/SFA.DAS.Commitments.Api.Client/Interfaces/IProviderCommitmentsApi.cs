using System.Collections.Generic;
using System.Threading.Tasks;

using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.DataLock;

namespace SFA.DAS.Commitments.Api.Client.Interfaces
{
    public interface IProviderCommitmentsApi
    {
        Task<List<CommitmentListItem>> GetProviderCommitments(long providerId);
        Task<CommitmentView> GetProviderCommitment(long providerId, long commitmentId);
        Task<List<Apprenticeship>> GetProviderApprenticeships(long providerId);
        Task<ApprenticeshipSearchResponse> GetProviderApprenticeships(long providerId, ApprenticeshipSearchQuery apprenticeshipSearchQuery);
        Task<Apprenticeship> GetProviderApprenticeship(long providerId, long apprenticeshipId);

        Task PatchProviderCommitment(long providerId, long commitmentId, CommitmentSubmission submission);
        Task DeleteProviderCommitment(long providerId, long commitmentId, DeleteRequest deleteRequest);

        Task CreateProviderApprenticeship(long providerId, long commitmentId, ApprenticeshipRequest apprenticeship);
        Task UpdateProviderApprenticeship(long providerId, long commitmentId, long apprenticeshipId, ApprenticeshipRequest apprenticeship);
        Task BulkUploadApprenticeships(long providerId, long commitmentId, BulkApprenticeshipRequest bulkRequest);
        Task DeleteProviderApprenticeship(long providerId, long apprenticeshipId, DeleteRequest deleteRequest);

        Task<long> BulkUploadFile(long providerId, BulkUploadFileRequest bulkUploadFileRequest);
        Task<string> BulkUploadFile(long providerId, long bulkUploadFileId);

        Task CreateApprenticeshipUpdate(long providerId, long apprenticeshipId, ApprenticeshipUpdateRequest apprenticeshipUpdateRequest);
        Task<ApprenticeshipUpdate> GetPendingApprenticeshipUpdate(long providerId, long apprenticeshipId);
        Task PatchApprenticeshipUpdate(long providerId, long apprenticeshipId, ApprenticeshipUpdateSubmission submission);

        Task<IEnumerable<PriceHistory>> GetPriceHistory(long providerId, long apprenticeshipId);

        Task<List<DataLockStatus>> GetDataLocks(long providerId, long apprenticeshipId);
        Task<DataLockSummary> GetDataLockSummary(long providerId, long apprenticeshipId);
        Task PatchDataLock(long providerId, long apprenticeshipId, long dataLockEventId, DataLockTriageSubmission triageSubmission);
        Task PatchDataLocks(long providerId, long apprenticeshipId, DataLockTriageSubmission triageSubmission);
    }
}
