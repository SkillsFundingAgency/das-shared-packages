using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Commitments.Api.Types;

namespace SFA.DAS.Commitments.Api.Client
{
    public interface ICommitmentsApi
    {
        Task<List<CommitmentListItem>> GetEmployerCommitments(long employerAccountId);
        Task<Commitment> GetEmployerCommitment(long employerAccountId, long commitmentId);
        Task<Commitment> CreateEmployerCommitment(long employerAccountId, Commitment commitment);
        Task PatchEmployerCommitment(long employerAccountId, long commitmentId, LastAction lastAction);
        Task<Apprenticeship> GetEmployerApprenticeship(long employerAccountId, long apprenticeshipId);
        Task UpdateEmployerApprenticeship(long employerAccountId, long commitmentId, long apprenticeshipId, Apprenticeship apprenticeship);
        Task PatchEmployerApprenticeship(long employerAccountId, long commitmentId, long apprenticeshipId, PaymentStatus paymentStatus);
        Task CreateEmployerApprenticeship(long employerAccountId, long commitmentId, Apprenticeship apprenticeship);
        Task DeleteEmployerApprenticeship(long employerAccountId, long apprenticeshipId);
        Task DeleteEmployerCommitment(long employerAccountId, long commitmentId);

        Task<List<CommitmentListItem>> GetProviderCommitments(long providerId);
        Task<Commitment> GetProviderCommitment(long providerId, long commitmentId);
        Task<Apprenticeship> GetProviderApprenticeship(long providerId, long apprenticeshipId);
        Task CreateProviderApprenticeship(long providerId, long commitmentId, Apprenticeship apprenticeship);
        Task UpdateProviderApprenticeship(long providerId, long commitmentId, long apprenticeshipId, Apprenticeship apprenticeship);
        Task PatchProviderCommitment(long providerId, long commitmentId, LastAction lastAction);
        Task BulkUploadApprenticeships(long providerId, long commitmentId, IList<Apprenticeship> apprenticeships);
        Task DeleteProviderApprenticeship(long providerId, long apprenticeshipId);
        Task DeleteProviderCommitment(long providerId, long commitmentId);
    }
}
