using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Commitments.Api.Types;

namespace SFA.DAS.Commitments.Api.Client
{
    public interface ICommitmentsApi
    {
        Task<List<CommitmentListItem>> GetEmployerCommitments(long employerAccountId);
        Task<Commitment> GetEmployerCommitment(long employerAccountId, long commitmentId);
        Task<List<Apprenticeship>> GetEmployerApprenticeships(long employerAccountId);
        Task<Apprenticeship> GetEmployerApprenticeship(long employerAccountId, long apprenticeshipId);

        Task<Commitment> CreateEmployerCommitment(long employerAccountId, CommitmentRequest commitment);
        Task PatchEmployerCommitment(long employerAccountId, long commitmentId, CommitmentSubmission submission);
        Task DeleteEmployerCommitment(long employerAccountId, long commitmentId, string userId);

        Task CreateEmployerApprenticeship(long employerAccountId, long commitmentId, ApprenticeshipRequest apprenticeship);
        Task UpdateEmployerApprenticeship(long employerAccountId, long commitmentId, long apprenticeshipId, ApprenticeshipRequest apprenticeship);
        Task PatchEmployerApprenticeship(long employerAccountId, long commitmentId, long apprenticeshipId, ApprenticeshipSubmission apprenticeshipSubmission);
        Task DeleteEmployerApprenticeship(long employerAccountId, long apprenticeshipId, string userId);

        Task<List<CommitmentListItem>> GetProviderCommitments(long providerId);
        Task<Commitment> GetProviderCommitment(long providerId, long commitmentId);
        Task<List<Apprenticeship>> GetProviderApprenticeships(long providerId);
        Task<Apprenticeship> GetProviderApprenticeship(long providerId, long apprenticeshipId);

        Task PatchProviderCommitment(long providerId, long commitmentId, CommitmentSubmission submission);
        Task DeleteProviderCommitment(long providerId, long commitmentId, string userId);

        Task CreateProviderApprenticeship(long providerId, long commitmentId, ApprenticeshipRequest apprenticeship);
        Task UpdateProviderApprenticeship(long providerId, long commitmentId, long apprenticeshipId, ApprenticeshipRequest apprenticeship);
        Task BulkUploadApprenticeships(long providerId, long commitmentId, BulkApprenticeshipRequest bulkRequest);
        Task DeleteProviderApprenticeship(long providerId, long apprenticeshipId, string userId);
        Task<Relationship> GetRelationship(long providerId, long employerAccountId, string legalEntityId);
        Task<Relationship> GetRelationshipByCommitment(long providerId, long commitmentId);
        Task PatchRelationship(long providerId, long employerAccountId, string legalEntityId, RelationshipRequest relationshipRequest);
    }
}
