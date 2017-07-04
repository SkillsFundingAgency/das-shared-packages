using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Newtonsoft.Json;

using SFA.DAS.Commitments.Api.Client.Configuration;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.DataLock;

namespace SFA.DAS.Commitments.Api.Client
{
    public class ProviderCommitmentsApi : HttpClientBase, IProviderCommitmentsApi
    {
        private readonly ICommitmentsApiClientConfiguration _configuration;

        private readonly IHttpCommitmentHelper _commitmentHelper;

        public ProviderCommitmentsApi(ICommitmentsApiClientConfiguration configuration)
            : base(configuration.ClientToken)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));
            _configuration = configuration;

            _commitmentHelper = new HttpCommitmentHelper(configuration.ClientToken);
        }

        public async Task PatchProviderCommitment(long providerId, long commitmentId, CommitmentSubmission submission)
        {
            var url = $"{_configuration.BaseUrl}api/provider/{providerId}/commitments/{commitmentId}";

            await _commitmentHelper.PatchCommitment(url, submission);
        }

        public async Task<List<Apprenticeship>> GetProviderApprenticeships(long providerId)
        {
            var url = $"{_configuration.BaseUrl}api/provider/{providerId}/apprenticeships/";

            return await _commitmentHelper.GetApprenticeships(url);
        }

        public async Task<ApprenticeshipSearchResponse> GetProviderApprenticeships(long providerId, ApprenticeshipSearchQuery apprenticeshipSearchQuery)
        {
            var url = $"{_configuration.BaseUrl}api/provider/{providerId}/apprenticeships/search";

            return await _commitmentHelper.GetApprenticeships(url, apprenticeshipSearchQuery);
        }

        public async Task<Apprenticeship> GetProviderApprenticeship(long providerId, long apprenticeshipId)
        {
            var url = $"{_configuration.BaseUrl}api/provider/{providerId}/apprenticeships/{apprenticeshipId}";

            return await _commitmentHelper.GetApprenticeship(url);
        }

        public async Task CreateProviderApprenticeship(long providerId, long commitmentId, ApprenticeshipRequest apprenticeship)
        {
            var url = $"{_configuration.BaseUrl}api/provider/{providerId}/commitments/{commitmentId}/apprenticeships";

            await _commitmentHelper.PostApprenticeship(url, apprenticeship);
        }

        public async Task UpdateProviderApprenticeship(long providerId, long commitmentId, long apprenticeshipId, ApprenticeshipRequest apprenticeship)
        {
            var url = $"{_configuration.BaseUrl}api/provider/{providerId}/commitments/{commitmentId}/apprenticeships/{apprenticeshipId}";

            await _commitmentHelper.PutApprenticeship(url, apprenticeship);
        }

        public async Task<List<CommitmentListItem>> GetProviderCommitments(long providerId)
        {
            var url = $"{_configuration.BaseUrl}api/provider/{providerId}/commitments";

            return await _commitmentHelper.GetCommitments(url);
        }

        public async Task<CommitmentView> GetProviderCommitment(long providerId, long commitmentId)
        {
            var url = $"{_configuration.BaseUrl}api/provider/{providerId}/commitments/{commitmentId}";

            return await _commitmentHelper.GetCommitment(url);
        }

        public async Task BulkUploadApprenticeships(long providerId, long commitmentId, BulkApprenticeshipRequest bulkRequest)
        {
            var url = $"{_configuration.BaseUrl}api/provider/{providerId}/commitments/{commitmentId}/apprenticeships/bulk";

            await _commitmentHelper.PostApprenticeships(url, bulkRequest);
        }

        public async Task DeleteProviderApprenticeship(long providerId, long apprenticeshipId, DeleteRequest deleteRequest)
        {
            var url = $"{_configuration.BaseUrl}api/provider/{providerId}/apprenticeships/{apprenticeshipId}";

            await _commitmentHelper.DeleteApprenticeship(url, deleteRequest);
        }

        public async Task<long> BulkUploadFile(long providerId, BulkUploadFileRequest bulkUploadFileRequest)
        {
            var url = $"{_configuration.BaseUrl}api/provider/{providerId}/bulkupload";
            return await _commitmentHelper.PostBulkuploadFile(url, bulkUploadFileRequest);
        }

        public async Task<string> BulkUploadFile(long providerId, long bulkUploadFileId)
        {
            var url = $"{_configuration.BaseUrl}api/provider/{providerId}/bulkupload/{bulkUploadFileId}";
            return await _commitmentHelper.GetBulkuploadFile(url);
        }

        public async Task DeleteProviderCommitment(long providerId, long commitmentId, DeleteRequest deleteRequest)
        {
            var url = $"{_configuration.BaseUrl}api/provider/{providerId}/commitments/{commitmentId}";

            await _commitmentHelper.DeleteCommitment(url, deleteRequest);
        }

        public async Task CreateApprenticeshipUpdate(long providerId, long apprenticeshipId, ApprenticeshipUpdateRequest apprenticeshipUpdateRequest)
        {
            var url = $"{_configuration.BaseUrl}api/provider/{providerId}/apprenticeships/{apprenticeshipId}/update";

            await _commitmentHelper.PostApprenticeshipUpdate(url, apprenticeshipUpdateRequest);
        }

        public async Task<ApprenticeshipUpdate> GetPendingApprenticeshipUpdate(long providerId, long apprenticeshipId)
        {
            var url = $"{_configuration.BaseUrl}api/provider/{providerId}/apprenticeships/{apprenticeshipId}/update";

            return await _commitmentHelper.GetApprenticeshipUpdate(url);
        }

        public async Task PatchApprenticeshipUpdate(long providerId, long apprenticeshipId, ApprenticeshipUpdateSubmission submission)
        {
            var url = $"{_configuration.BaseUrl}api/provider/{providerId}/apprenticeships/{apprenticeshipId}/update";

            await _commitmentHelper.PatchApprenticeshipUpdate(url, submission);
        }

        public async Task<IEnumerable<PriceHistory>> GetPriceHistory(long providerId, long apprenticeshipId)
        {
            var url = $"{_configuration.BaseUrl}api/provider/{providerId}/apprenticeships/{apprenticeshipId}/prices";
            var content = await GetAsync(url);
            return JsonConvert.DeserializeObject<IEnumerable<PriceHistory>>(content);
        }

        public async Task<List<DataLockStatus>> GetDataLocks(long providerId, long apprenticeshipId)
        {
            var url = $"{_configuration.BaseUrl}api/provider/{providerId}/apprenticeships/{apprenticeshipId}/datalocks";
            return await GetData<List<DataLockStatus>>(url);
        }

        public async Task<DataLockSummary> GetDataLockSummary(long providerId, long apprenticeshipId)
        {
            var url = $"{_configuration.BaseUrl}api/provider/{providerId}/apprenticeships/{apprenticeshipId}/datalocksummary";
            return await GetData<DataLockSummary>(url);
        }

        public async Task PatchDataLock(long providerId, long apprenticeshipId, long dataLockEventId, DataLockTriageSubmission triageSubmission)
        {
            var url = $"{_configuration.BaseUrl}api/provider/{providerId}/apprenticeships/{apprenticeshipId}/datalocks/{dataLockEventId}";
            await PatchModel(url, triageSubmission);
        }

        public async Task PatchDataLocks(long providerId, long apprenticeshipId, DataLockTriageSubmission triageSubmission)
        {
            var url = $"{_configuration.BaseUrl}api/provider/{providerId}/apprenticeships/{apprenticeshipId}/datalocks";
            await PatchModel(url, triageSubmission);
        }
    }
}
