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
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));
            _configuration = configuration;
        }

        public async Task CreateEmployerCommitment(long employerAccountId, Commitment commitment)
        {
            var url = $"{_configuration.BaseUrl}api/employer/{employerAccountId}/commitments";

            await PostCommitment(url, commitment);
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

        public async Task<Apprenticeship> GetEmployerApprenticeship(long employerAccountId, long commitmentId, long apprenticeshipId)
        {
            var url = $"{_configuration.BaseUrl}api/employer/{employerAccountId}/commitments/{commitmentId}/apprenticeships/{apprenticeshipId}";

            return await GetApprenticeship(url);
        }

        public async Task PatchEmployerCommitment(long employerAccountId, long commitmentId, CommitmentStatus status)
        {
            var url = $"{_configuration.BaseUrl}api/employer/{employerAccountId}/commitments/{commitmentId}";

            await PatchCommitment(url, status);
        }

        public async Task PatchProviderCommitment(long providerId, long commitmentId, CommitmentStatus status)
        {
            var url = $"{_configuration.BaseUrl}api/provider/{providerId}/commitments/{commitmentId}";

            await PatchCommitment(url, status);
        }

        public async Task CreateEmployerApprenticeship(long employerAccountId, long commitmentId, Apprenticeship apprenticeship)
        {
            var url = $"{_configuration.BaseUrl}api/employer/{employerAccountId}/commitments/{commitmentId}/apprenticeships";

            await PostApprenticeship(url, apprenticeship);
        }

        public async Task UpdateEmployerApprenticeship(long employerAccountId, long commitmentId, long apprenticeshipId, Apprenticeship apprenticeship)
        {
            var url = $"{_configuration.BaseUrl}api/employer/{employerAccountId}/commitments/{commitmentId}/apprenticeships/{apprenticeshipId}";

            await PutApprenticeship(url, apprenticeship);
        }

        public async Task PatchEmployerApprenticeship(long employerAccountId, long commitmentId, long apprenticeshipId, ApprenticeshipStatus status)
        {
            var url = $"{_configuration.BaseUrl}api/employer/{employerAccountId}/commitments/{commitmentId}/apprenticeships/{apprenticeshipId}";

            await PatchApprenticeship(url, status);
        }

        public async Task<Apprenticeship> GetProviderApprenticeship(long providerId, long commitmentId, long apprenticeshipId)
        {
            var url = $"{_configuration.BaseUrl}api/provider/{providerId}/commitments/{commitmentId}/apprenticeships/{apprenticeshipId}";

            return await GetApprenticeship(url);
        }

        public async Task CreateProviderApprenticeship(long providerId, long commitmentId, Apprenticeship apprenticeship)
        {
            var url = $"{_configuration.BaseUrl}api/provider/{providerId}/commitments/{commitmentId}/apprenticeships";

            await PostApprenticeship(url, apprenticeship);
        }

        public async Task UpdateProviderApprenticeship(long providerId, long commitmentId, long apprenticeshipId, Apprenticeship apprenticeship)
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

        private async Task<Commitment> PostCommitment(string url, Commitment commitment)
        {
            var data = JsonConvert.SerializeObject(commitment);
            var content = await PostAsync(url, data);

            return JsonConvert.DeserializeObject<Commitment>(content);
        }

        private async Task PatchCommitment(string url, CommitmentStatus change)
        {
            var data = JsonConvert.SerializeObject(change);
            await PatchAsync(url, data);
        }

        private async Task PatchApprenticeship(string url, ApprenticeshipStatus status)
        {
            var data = JsonConvert.SerializeObject(status);
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

        private async Task<Apprenticeship> GetApprenticeship(string url)
        {
            var content = await GetAsync(url);

            return JsonConvert.DeserializeObject<Apprenticeship>(content);
        }

        private async Task PutApprenticeship(string url, Apprenticeship apprenticeship)
        {
            var data = JsonConvert.SerializeObject(apprenticeship);
            var content = await PutAsync(url, data);
        }

        private async Task<Apprenticeship> PostApprenticeship(string url, Apprenticeship apprenticeship)
        {
            var data = JsonConvert.SerializeObject(apprenticeship);
            var content = await PostAsync(url, data);

            return JsonConvert.DeserializeObject<Apprenticeship>(content);
        }
    }
}