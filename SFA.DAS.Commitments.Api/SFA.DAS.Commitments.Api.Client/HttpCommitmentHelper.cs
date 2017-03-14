using System.Collections.Generic;
using System.Threading.Tasks;

using Newtonsoft.Json;

using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;

namespace SFA.DAS.Commitments.Api.Client
{
    internal class HttpCommitmentHelper : HttpClientBase, IHttpCommitmentHelper
    {
        internal HttpCommitmentHelper(string clientToken)
            : base(clientToken)
        {
        }

        public async Task<Commitment> PostCommitment(string url, CommitmentRequest commitment)
        {
            var data = JsonConvert.SerializeObject(commitment);
            var content = await PostAsync(url, data);

            return JsonConvert.DeserializeObject<Commitment>(content);
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

        public async Task<Commitment> GetCommitment(string url)
        {
            var content = await GetAsync(url);

            return JsonConvert.DeserializeObject<Commitment>(content);
        }

        public async Task<List<Apprenticeship>> GetApprenticeships(string url)
        {
            var content = await GetAsync(url);

            return JsonConvert.DeserializeObject<List<Apprenticeship>>(content);
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
    }
}
