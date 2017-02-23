﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.Commitments.Api.Client.Configuration;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.History;

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

        public async Task<Commitment> CreateEmployerCommitment(long employerAccountId, Commitment commitment)
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

        public async Task PatchEmployerApprenticeship(long employerAccountId, long commitmentId, long apprenticeshipId, PaymentStatus paymentStatus)
        {
            var url = $"{_configuration.BaseUrl}api/employer/{employerAccountId}/commitments/{commitmentId}/apprenticeships/{apprenticeshipId}";

            await PatchApprenticeship(url, paymentStatus);
        }

        public async Task DeleteEmployerApprenticeship(long employerAccountId, long apprenticeshipId)
        {
            var url = $"{_configuration.BaseUrl}api/employer/{employerAccountId}/apprenticeships/{apprenticeshipId}";

            await DeleteApprenticeship(url);
        }

        public async Task DeleteEmployerCommitment(long employerAccountId, long commitmentId)
        {
            var url = $"{_configuration.BaseUrl}api/employer/{employerAccountId}/commitments/{commitmentId}";

            await DeleteCommitment(url);
        }

        public async Task<List<CommitmentHistoryItem>> GetEmployerCommitmentHistory(long employerAccountId, long commitmentId)
        {
            var url = $"{_configuration.BaseUrl}api/employer/{employerAccountId}/commitments/{commitmentId}/history";
            return await GetCommitmentHistory(url);
        }

        public async Task<List<ApprenticeshipHistoryItem>> GetEmployerApprenticeshipHistory(long employerAccountId, long apprenticeshipId)
        {
            var url = $"{_configuration.BaseUrl}api/employer/{employerAccountId}/apprenticeships/{apprenticeshipId}/history";
            return await GetApprenticehipHistory(url);
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

        public async Task BulkUploadApprenticeships(long providerId, long commitmentId, IList<Apprenticeship> apprenticeships)
        {
            var url = $"{_configuration.BaseUrl}api/provider/{providerId}/commitments/{commitmentId}/apprenticeships/bulk";

            await PostApprenticeships(url, apprenticeships);
        }

        public async Task DeleteProviderApprenticeship(long providerId, long apprenticeshipId)
        {
            var url = $"{_configuration.BaseUrl}api/provider/{providerId}/apprenticeships/{apprenticeshipId}";

            await DeleteApprenticeship(url);
        }

        public async Task DeleteProviderCommitment(long providerId, long commitmentId)
        {
            var url = $"{_configuration.BaseUrl}api/provider/{providerId}/commitments/{commitmentId}";

            await DeleteCommitment(url);
        }

        public async Task<List<CommitmentHistoryItem>> GetProviderCommitmentHistory(long providerId, long commitmentId)
        {
            var url = $"{_configuration.BaseUrl}api/provider/{providerId}/commitments/{commitmentId}/history";
            return await GetCommitmentHistory(url);
        }
        public async Task<List<ApprenticeshipHistoryItem>> GetProviderApprenticeshipHistory(long providerId, long apprenticeshipId)
        {
            var url = $"{_configuration.BaseUrl}api/provider/{providerId}/apprenticeships/{apprenticeshipId}/history";
            return await GetApprenticehipHistory(url);
        }

        private async Task<Commitment> PostCommitment(string url, Commitment commitment)
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

        private async Task PatchApprenticeship(string url, PaymentStatus paymentStatus)
        {
            var data = JsonConvert.SerializeObject(paymentStatus);
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

        private async Task<Apprenticeship> PostApprenticeships(string url, IList<Apprenticeship> apprenticeships)
        {
            var data = JsonConvert.SerializeObject(apprenticeships);
            var content = await PostAsync(url, data);

            return JsonConvert.DeserializeObject<Apprenticeship>(content);
        }

        private async Task DeleteApprenticeship(string url)
        {
            await DeleteAsync(url);
        }

        private async Task DeleteCommitment(string url)
        {
            await DeleteAsync(url);
        }

        private async Task<List<CommitmentHistoryItem>> GetCommitmentHistory(string url)
        {
            var content = await GetAsync(url);

            return JsonConvert.DeserializeObject<List<CommitmentHistoryItem>>(content);
        }

        private async Task<List<ApprenticeshipHistoryItem>> GetApprenticehipHistory(string url)
        {
            var content = await GetAsync(url);

            return JsonConvert.DeserializeObject<List<ApprenticeshipHistoryItem>>(content);
        }
    }
}
