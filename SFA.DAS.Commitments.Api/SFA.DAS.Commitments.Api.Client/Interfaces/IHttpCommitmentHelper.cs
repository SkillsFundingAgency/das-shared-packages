using System.Collections.Generic;
using System.Threading.Tasks;

using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;

namespace SFA.DAS.Commitments.Api.Client.Interfaces
{
    public interface IHttpCommitmentHelper
    {
        Task<CommitmentView> PostCommitment(string url, CommitmentRequest commitment);

        Task PatchCommitment(string url, CommitmentSubmission submision);

        Task PutCommitment(string url, CommitmentStatus commitmentStatus);

        Task PatchApprenticeship(string url, ApprenticeshipSubmission apprenticeshipSubmission);

        Task<List<CommitmentListItem>> GetCommitments(string url);

        Task<CommitmentView> GetCommitment(string url);

        Task<List<Apprenticeship>> GetApprenticeships(string url);

        Task<ApprenticeshipSearchResponse> GetApprenticeships(string url, ApprenticeshipSearchQuery apprenticeshipQuery);

        Task<Apprenticeship> GetApprenticeship(string url);

        Task PutApprenticeship(string url, ApprenticeshipRequest apprenticeship);

        Task<Apprenticeship> PostApprenticeship(string url, ApprenticeshipRequest apprenticeship);

        Task<Apprenticeship> PostApprenticeships(string url, BulkApprenticeshipRequest bulkRequest);

        Task DeleteApprenticeship(string url, DeleteRequest deleteRequest);

        Task DeleteCommitment(string url, DeleteRequest deleteRequest);

        Task<string> GetAsync(string url);

        Task<string> PostAsync(string url, string data);

        Task<string> PutAsync(string url, string data);

        Task<string> PatchAsync(string url, string data);

        Task DeleteAsync(string url, string data);

        Task PostApprenticeshipUpdate(string url, ApprenticeshipUpdateRequest apprenticeshipUpdate);

        Task<ApprenticeshipUpdate> GetApprenticeshipUpdate(string url);

        Task PatchApprenticeshipUpdate(string url, ApprenticeshipUpdateSubmission submission);
    }
}