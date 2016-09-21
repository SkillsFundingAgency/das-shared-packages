using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Commitments.Api.Types;

namespace SFA.DAS.Commitments.Api.Client
{
    public interface ICommitmentsApi
    {
        Task<List<CommitmentListItem>> GetEmployerCommitments(long employerAccountId);
        Task<Commitment> GetEmployerCommitment(long employerAccountId, long commitmentId);
        Task CreateEmployerCommitment(long employerAccountId, Commitment commitment);
        Task PatchEmployerCommitment(long employerAccountId, long commitmentId, CommitmentStatus status);
        Task<Apprenticeship> GetEmployerApprenticeship(long employerAccountId, long commitmentId, long apprenticeshipId);
        Task UpdateEmployerApprenticeship(long employerAccountId, long commitmentId, long apprenticeshipId, Apprenticeship apprenticeship);
        Task PatchEmployerApprenticeship(long employerAccountId, long commitmentId, long apprenticeshipId, ApprenticeshipStatus status);


        Task<List<CommitmentListItem>> GetProviderCommitments(long providerId);
        Task<Commitment> GetProviderCommitment(long providerId, long commitmentId);
        Task<Apprenticeship> GetProviderApprenticeship(long providerId, long commitmentId, long apprenticeshipId);
        Task CreateProviderApprenticeship(long providerId, long commitmentId, Apprenticeship apprenticeship);
        Task UpdateProviderApprenticeship(long providerId, long commitmentId, long apprenticeshipId, Apprenticeship apprenticeship);
        Task PatchProviderCommitment(long employerAccountId, long commitmentId, CommitmentStatus status);
    }
}