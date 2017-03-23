using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using SFA.DAS.Commitments.Api.Client.Configuration;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Commitment;

namespace SFA.DAS.Commitments.Api.Client
{
    public class EmployerCommitmentApi : HttpClientBase, IEmployerCommitmentApi
    {
        private readonly ICommitmentsApiClientConfiguration _configuration;

        private readonly IHttpCommitmentHelper _commitmentHelper;

        public EmployerCommitmentApi(ICommitmentsApiClientConfiguration configuration)
            : base(configuration.ClientToken)
        {
            if(configuration == null)
                throw new ArgumentException(nameof(configuration));
            _configuration = configuration;

            _commitmentHelper = new HttpCommitmentHelper(configuration.ClientToken);
        }
        public async Task<Commitment> CreateEmployerCommitment(long employerAccountId, CommitmentRequest commitment)
        {
            var url = $"{_configuration.BaseUrl}api/employer/{employerAccountId}/commitments";

            return await _commitmentHelper.PostCommitment(url, commitment);
        }

        public async Task<List<CommitmentListItem>> GetEmployerCommitments(long employerAccountId)
        {
            var url = $"{_configuration.BaseUrl}api/employer/{employerAccountId}/commitments";

            return await _commitmentHelper.GetCommitments(url);
        }

        public async Task<Commitment> GetEmployerCommitment(long employerAccountId, long commitmentId)
        {
            var url = $"{_configuration.BaseUrl}api/employer/{employerAccountId}/commitments/{commitmentId}";

            return await _commitmentHelper.GetCommitment(url);
        }

        public async Task<List<Apprenticeship>> GetEmployerApprenticeships(long employerAccountId)
        {
            var url = $"{_configuration.BaseUrl}api/employer/{employerAccountId}/apprenticeships/";

            return await _commitmentHelper.GetApprenticeships(url);
        }

        public async Task<Apprenticeship> GetEmployerApprenticeship(long employerAccountId, long apprenticeshipId)
        {
            var url = $"{_configuration.BaseUrl}api/employer/{employerAccountId}/apprenticeships/{apprenticeshipId}";

            return await _commitmentHelper.GetApprenticeship(url);
        }

        public async Task PatchEmployerCommitment(long employerAccountId, long commitmentId, CommitmentSubmission submission)
        {
            var url = $"{_configuration.BaseUrl}api/employer/{employerAccountId}/commitments/{commitmentId}";

            await _commitmentHelper.PatchCommitment(url, submission);
        }

        public async Task CreateEmployerApprenticeship(long employerAccountId, long commitmentId, ApprenticeshipRequest apprenticeship)
        {
            var url = $"{_configuration.BaseUrl}api/employer/{employerAccountId}/commitments/{commitmentId}/apprenticeships";

            await _commitmentHelper.PostApprenticeship(url, apprenticeship);
        }

        public async Task UpdateEmployerApprenticeship(long employerAccountId, long commitmentId, long apprenticeshipId, ApprenticeshipRequest apprenticeship)
        {
            var url = $"{_configuration.BaseUrl}api/employer/{employerAccountId}/commitments/{commitmentId}/apprenticeships/{apprenticeshipId}";

            await _commitmentHelper.PutApprenticeship(url, apprenticeship);
        }

        public async Task PatchEmployerApprenticeship(long employerAccountId, long commitmentId, long apprenticeshipId, ApprenticeshipSubmission apprenticeshipSubmission)
        {
            var url = $"{_configuration.BaseUrl}api/employer/{employerAccountId}/commitments/{commitmentId}/apprenticeships/{apprenticeshipId}";

            await _commitmentHelper.PatchApprenticeship(url, apprenticeshipSubmission);
        }

        public async Task DeleteEmployerApprenticeship(long employerAccountId, long apprenticeshipId, DeleteRequest deleteRequest)
        {
            var url = $"{_configuration.BaseUrl}api/employer/{employerAccountId}/apprenticeships/{apprenticeshipId}";

            await _commitmentHelper.DeleteApprenticeship(url, deleteRequest);
        }

        public async Task DeleteEmployerCommitment(long employerAccountId, long commitmentId, DeleteRequest deleteRequest)
        {
            var url = $"{_configuration.BaseUrl}api/employer/{employerAccountId}/commitments/{commitmentId}";

            await _commitmentHelper.DeleteCommitment(url, deleteRequest);
        }

        public async Task CreateApprenticeshipUpdate(long employerAccountId, ApprenticeshipUpdateRequest apprenticeshipUpdateRequest)
        {
            var url = $"{_configuration.BaseUrl}api/employer/{employerAccountId}/apprenticeshipupdate";

            await _commitmentHelper.PostApprenticeshipUpdate(url, apprenticeshipUpdateRequest);
        }

        public async Task<ApprenticeshipUpdate> GetPendingApprenticeshipUpdate(long employerAccountId, long apprenticeshipId)
        {
            var url = $"{_configuration.BaseUrl}api/employer/{employerAccountId}/apprenticeshipupdate";

            return await _commitmentHelper.GetApprenticeshipUpdate(url);
        }
    }
}