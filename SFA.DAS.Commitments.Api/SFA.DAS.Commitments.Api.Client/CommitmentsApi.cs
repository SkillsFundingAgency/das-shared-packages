using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.Commitments.Api.Client.Configuration;
using SFA.DAS.Commitments.Api.Types;

namespace SFA.DAS.Commitments.Api.Client
{
    public class CommitmentsApi : HttpClientBase, ICommitmentsApi
    {
        private readonly ICommitmentsApiClientConfiguration _configuration;

        public CommitmentsApi(ICommitmentsApiClientConfiguration configuration)
            : base(configuration.ClientToken)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));
            _configuration = configuration;
        }

        public async Task<Commitment> CreateEmployerCommitment(long employerAccountId, CommitmentRequest commitment)
        {
            var url = $"{_configuration.BaseUrl}api/employer/{employerAccountId}/commitments";

            return await PostCommitment(url, commitment);
        }

        public async Task<List<CommitmentListItem>> GetEmployerCommitments(long employerAccountId)
        {
            var url = $"{_configuration.BaseUrl}api/employer/{employerAccountId}/commitments";

            return await GetCommitments(url);
        }

        public async Task<Commitment> GetEmployerCommitment(long employerAccountId, long commitmentId)
        {
            var url = $"{_configuration.BaseUrl}api/employer/{employerAccountId}/commitments/{commitmentId}";

            return await GetCommitment(url);
        }

        public async Task<List<Apprenticeship>> GetEmployerApprenticeships(long employerAccountId)
        {
            var url = $"{_configuration.BaseUrl}api/employer/{employerAccountId}/apprenticeships/";

            return await GetApprenticeships(url);
        }

        public async Task<Apprenticeship> GetEmployerApprenticeship(long employerAccountId, long apprenticeshipId)
        {
            var url = $"{_configuration.BaseUrl}api/employer/{employerAccountId}/apprenticeships/{apprenticeshipId}";

            return await GetApprenticeship(url);
        }

        public async Task PatchEmployerCommitment(long employerAccountId, long commitmentId, CommitmentSubmission submission)
        {
            var url = $"{_configuration.BaseUrl}api/employer/{employerAccountId}/commitments/{commitmentId}";

            await PatchCommitment(url, submission);
        }

        public async Task PatchProviderCommitment(long providerId, long commitmentId, CommitmentSubmission submission)
        {
            var url = $"{_configuration.BaseUrl}api/provider/{providerId}/commitments/{commitmentId}";

            await PatchCommitment(url, submission);
        }

        public async Task CreateEmployerApprenticeship(long employerAccountId, long commitmentId, ApprenticeshipRequest apprenticeship)
        {
            var url = $"{_configuration.BaseUrl}api/employer/{employerAccountId}/commitments/{commitmentId}/apprenticeships";

            await PostApprenticeship(url, apprenticeship);
        }

        public async Task UpdateEmployerApprenticeship(long employerAccountId, long commitmentId, long apprenticeshipId, ApprenticeshipRequest apprenticeship)
        {
            var url = $"{_configuration.BaseUrl}api/employer/{employerAccountId}/commitments/{commitmentId}/apprenticeships/{apprenticeshipId}";

            await PutApprenticeship(url, apprenticeship);
        }

        public async Task PatchEmployerApprenticeship(long employerAccountId, long commitmentId, long apprenticeshipId, ApprenticeshipSubmission apprenticeshipSubmission)
        {
            var url = $"{_configuration.BaseUrl}api/employer/{employerAccountId}/commitments/{commitmentId}/apprenticeships/{apprenticeshipId}";

            await PatchApprenticeship(url, apprenticeshipSubmission);
        }

        public async Task DeleteEmployerApprenticeship(long employerAccountId, long apprenticeshipId, DeleteRequest deleteRequest)
        {
            var url = $"{_configuration.BaseUrl}api/employer/{employerAccountId}/apprenticeships/{apprenticeshipId}";

            await DeleteApprenticeship(url, deleteRequest);
        }

        public async Task DeleteEmployerCommitment(long employerAccountId, long commitmentId, DeleteRequest deleteRequest)
        {
            var url = $"{_configuration.BaseUrl}api/employer/{employerAccountId}/commitments/{commitmentId}";

            await DeleteCommitment(url, deleteRequest);
        }

        public async Task<List<Apprenticeship>> GetProviderApprenticeships(long providerId)
        {
            var url = $"{_configuration.BaseUrl}api/provider/{providerId}/apprenticeships/";

            return await GetApprenticeships(url);
        }

        public async Task<Apprenticeship> GetProviderApprenticeship(long providerId, long apprenticeshipId)
        {
            var url = $"{_configuration.BaseUrl}api/provider/{providerId}/apprenticeships/{apprenticeshipId}";

            return await GetApprenticeship(url);
        }

        public async Task CreateProviderApprenticeship(long providerId, long commitmentId, ApprenticeshipRequest apprenticeship)
        {
            var url = $"{_configuration.BaseUrl}api/provider/{providerId}/commitments/{commitmentId}/apprenticeships";

            await PostApprenticeship(url, apprenticeship);
        }

        public async Task UpdateProviderApprenticeship(long providerId, long commitmentId, long apprenticeshipId, ApprenticeshipRequest apprenticeship)
        {
            var url = $"{_configuration.BaseUrl}api/provider/{providerId}/commitments/{commitmentId}/apprenticeships/{apprenticeshipId}";

            await PutApprenticeship(url, apprenticeship);
        }

        public async Task<List<CommitmentListItem>> GetProviderCommitments(long providerId)
        {
            var url = $"{_configuration.BaseUrl}api/provider/{providerId}/commitments";

            return await GetCommitments(url);
        }

        public async Task<Commitment> GetProviderCommitment(long providerId, long commitmentId)
        {
            var url = $"{_configuration.BaseUrl}api/provider/{providerId}/commitments/{commitmentId}";

            return await GetCommitment(url);
        }

        public async Task BulkUploadApprenticeships(long providerId, long commitmentId, BulkApprenticeshipRequest bulkRequest)
        {
            var url = $"{_configuration.BaseUrl}api/provider/{providerId}/commitments/{commitmentId}/apprenticeships/bulk";

            await PostApprenticeships(url, bulkRequest);
        }

        public async Task DeleteProviderApprenticeship(long providerId, long apprenticeshipId, DeleteRequest deleteRequest)
        {
            var url = $"{_configuration.BaseUrl}api/provider/{providerId}/apprenticeships/{apprenticeshipId}";

            await DeleteApprenticeship(url, deleteRequest);
        }

        public async Task DeleteProviderCommitment(long providerId, long commitmentId, DeleteRequest deleteRequest)
        {
            var url = $"{_configuration.BaseUrl}api/provider/{providerId}/commitments/{commitmentId}";

            await DeleteCommitment(url, deleteRequest);
        }

        public async Task<Relationship> GetRelationship(long providerId, long employerAccountId, string legalEntityId)
        {
            var url = $"{_configuration.BaseUrl}api/provider/{providerId}/relationships/{employerAccountId}/{legalEntityId}";
            return await GetRelationship(url);
        }

        public async Task PatchRelationship(long providerId, long employerAccountId, string legalEntityId, RelationshipRequest relationshipRequest)
        {
            var url = $"{_configuration.BaseUrl}api/provider/{providerId}/relationships/{employerAccountId}/{legalEntityId}";
            await PatchRelationship(url, relationshipRequest);
        }

        public async Task<Relationship> GetRelationshipByCommitment(long providerId, long commitmentId)
        {
            var url = $"{_configuration.BaseUrl}api/provider/{providerId}/relationships/{commitmentId}";
            return await GetRelationship(url);
        }

        private async Task<Commitment> PostCommitment(string url, CommitmentRequest commitment)
        {
            var data = JsonConvert.SerializeObject(commitment);
            var content = await PostAsync(url, data);

            return JsonConvert.DeserializeObject<Commitment>(content);
        }

        private async Task PatchCommitment(string url, CommitmentSubmission submision)
        {
            var data = JsonConvert.SerializeObject(submision);
            await PatchAsync(url, data);
        }

        private async Task PutCommitment(string url, CommitmentStatus commitmentStatus)
        {
            var data = JsonConvert.SerializeObject(commitmentStatus);
            await PutAsync(url, data);
        }

        private async Task PatchApprenticeship(string url, ApprenticeshipSubmission apprenticeshipSubmission)
        {
            var data = JsonConvert.SerializeObject(apprenticeshipSubmission);
            await PatchAsync(url, data);
        }

        private async Task<List<CommitmentListItem>> GetCommitments(string url)
        {
            var content = await GetAsync(url);

            return JsonConvert.DeserializeObject<List<CommitmentListItem>>(content);
        }

        private async Task<Commitment> GetCommitment(string url)
        {
            var content = await GetAsync(url);

            return JsonConvert.DeserializeObject<Commitment>(content);
        }

        private async Task<List<Apprenticeship>> GetApprenticeships(string url)
        {
            var content = await GetAsync(url);

            return JsonConvert.DeserializeObject<List<Apprenticeship>>(content);
        }

        private async Task<Apprenticeship> GetApprenticeship(string url)
        {
            var content = await GetAsync(url);

            return JsonConvert.DeserializeObject<Apprenticeship>(content);
        }

        private async Task PutApprenticeship(string url, ApprenticeshipRequest apprenticeship)
        {
            var data = JsonConvert.SerializeObject(apprenticeship);
            await PutAsync(url, data);
        }

        private async Task<Apprenticeship> PostApprenticeship(string url, ApprenticeshipRequest apprenticeship)
        {
            var data = JsonConvert.SerializeObject(apprenticeship);
            var content = await PostAsync(url, data);

            return JsonConvert.DeserializeObject<Apprenticeship>(content);
        }

        private async Task<Apprenticeship> PostApprenticeships(string url, BulkApprenticeshipRequest bulkRequest)
        {
            var data = JsonConvert.SerializeObject(bulkRequest);
            var content = await PostAsync(url, data);

            return JsonConvert.DeserializeObject<Apprenticeship>(content);
        }

        private async Task DeleteApprenticeship(string url, DeleteRequest deleteRequest)
        {
            var data = JsonConvert.SerializeObject(deleteRequest);
            await DeleteAsync(url, data);
        }

        private async Task DeleteCommitment(string url, DeleteRequest deleteRequest)
        {
            var data = JsonConvert.SerializeObject(deleteRequest);
            await DeleteAsync(url, data);
        }

        private async Task<Relationship> GetRelationship(string url)
        {
            var content = await GetAsync(url);
            return JsonConvert.DeserializeObject<Relationship>(content);
        }

        private async Task PatchRelationship(string url, RelationshipRequest relationshipRequest)
        {
            var data = JsonConvert.SerializeObject(relationshipRequest);
            await PatchAsync(url, data);
        }
    }
}
