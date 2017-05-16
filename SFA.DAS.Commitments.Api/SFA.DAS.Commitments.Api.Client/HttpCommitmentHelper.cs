using System.Collections.Generic;
using System.Threading.Tasks;

using Newtonsoft.Json;

using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;

namespace SFA.DAS.Commitments.Api.Client
{
    internal class HttpCommitmentHelper : HttpClientBase, IHttpCommitmentHelper
    {
        internal HttpCommitmentHelper(string clientToken)
            : base(clientToken)
        {
        }

        public async Task<CommitmentView> PostCommitment(string url, CommitmentRequest commitment)
        {
            var data = JsonConvert.SerializeObject(commitment);
            var content = await PostAsync(url, data);

            return JsonConvert.DeserializeObject<CommitmentView>(content);
        }

        public async Task PatchCommitment(string url, CommitmentSubmission submision)
        {
            var data = JsonConvert.SerializeObject(submision);
            await PatchAsync(url, data);
        }

        public async Task PutCommitment(string url, CommitmentStatus commitmentStatus)
        {
            var data = JsonConvert.SerializeObject(commitmentStatus);
            await PutAsync(url, data);
        }

        public async Task PatchApprenticeship(string url, ApprenticeshipSubmission apprenticeshipSubmission)
        {
            var data = JsonConvert.SerializeObject(apprenticeshipSubmission);
            await PatchAsync(url, data);
        }

        public async Task<List<CommitmentListItem>> GetCommitments(string url)
        {
            var content = await GetAsync(url);

            return JsonConvert.DeserializeObject<List<CommitmentListItem>>(content);
        }

        public async Task<CommitmentView> GetCommitment(string url)
        {
            var content = await GetAsync(url);

            return JsonConvert.DeserializeObject<CommitmentView>(content);
        }

        public async Task<List<Apprenticeship>> GetApprenticeships(string url)
        {
            var content = await GetAsync(url);

            return JsonConvert.DeserializeObject<List<Apprenticeship>>(content);
        }

        public async Task<ApprenticeshipSearchResponse> GetApprenticeships(string url, ApprenticeshipSearchQuery apprenticeshipQuery)
        {
            var result = await GetAsync(url, apprenticeshipQuery);

            return JsonConvert.DeserializeObject<ApprenticeshipSearchResponse>(result);
        }

        public async Task<Apprenticeship> GetApprenticeship(string url)
        {
            var content = await GetAsync(url);

            return JsonConvert.DeserializeObject<Apprenticeship>(content);
        }

        public async Task PutApprenticeship(string url, ApprenticeshipRequest apprenticeship)
        {
            var data = JsonConvert.SerializeObject(apprenticeship);
            await PutAsync(url, data);
        }

        public async Task<Apprenticeship> PostApprenticeship(string url, ApprenticeshipRequest apprenticeship)
        {
            var data = JsonConvert.SerializeObject(apprenticeship);
            var content = await PostAsync(url, data);

            return JsonConvert.DeserializeObject<Apprenticeship>(content);
        }

        public async Task<Apprenticeship> PostApprenticeships(string url, BulkApprenticeshipRequest bulkRequest)
        {
            var data = JsonConvert.SerializeObject(bulkRequest);
            var content = await PostAsync(url, data);

            return JsonConvert.DeserializeObject<Apprenticeship>(content);
        }

        public async Task DeleteApprenticeship(string url, DeleteRequest deleteRequest)
        {
            var data = JsonConvert.SerializeObject(deleteRequest);
            await DeleteAsync(url, data);
        }

        public async Task DeleteCommitment(string url, DeleteRequest deleteRequest)
        {
            var data = JsonConvert.SerializeObject(deleteRequest);
            await DeleteAsync(url, data);
        }

        public async Task PostApprenticeshipUpdate(string url, ApprenticeshipUpdateRequest apprenticeshipUpdateRequest)
        {
            var data = JsonConvert.SerializeObject(apprenticeshipUpdateRequest);
            await PostAsync(url, data);
        }

        public async Task<ApprenticeshipUpdate> GetApprenticeshipUpdate(string url)
        {
            var content = await GetAsync(url);
            return JsonConvert.DeserializeObject<ApprenticeshipUpdate>(content);
        }

        public async Task PatchApprenticeshipUpdate(string url, ApprenticeshipUpdateSubmission submission)
        {
            var data = JsonConvert.SerializeObject(submission);
            await PatchAsync(url, data);
        }
    }
}
