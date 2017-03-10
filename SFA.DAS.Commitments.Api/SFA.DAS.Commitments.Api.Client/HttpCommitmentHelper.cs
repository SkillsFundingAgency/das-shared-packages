using System.Collections.Generic;
using System.Threading.Tasks;

using Newtonsoft.Json;

using SFA.DAS.Commitments.Api.Types;

namespace SFA.DAS.Commitments.Api.Client
{
    public interface IHttpCommitmentHelper
    {
        Task<Commitment> PostCommitment(string url, CommitmentRequest commitment);

        Task PatchCommitment(string url, CommitmentSubmission submision);

        Task PutCommitment(string url, CommitmentStatus commitmentStatus);

        Task PatchApprenticeship(string url, ApprenticeshipSubmission apprenticeshipSubmission);

        Task<List<CommitmentListItem>> GetCommitments(string url);

        Task<Commitment> GetCommitment(string url);

        Task<List<Apprenticeship>> GetApprenticeships(string url);

        Task<Apprenticeship> GetApprenticeship(string url);

        Task PutApprenticeship(string url, ApprenticeshipRequest apprenticeship);

        Task<Apprenticeship> PostApprenticeship(string url, ApprenticeshipRequest apprenticeship);

        Task<Apprenticeship> PostApprenticeships(string url, BulkApprenticeshipRequest bulkRequest);

        Task DeleteApprenticeship(string url, DeleteRequest deleteRequest);

        Task DeleteCommitment(string url, DeleteRequest deleteRequest);

        Task<string> GetAsync(string url);

        Task<string> PostAsync(string url, string data);

        Task<string> PutAsync(string url, string data);

        Task<string> PatchAsync(string url, string data);

        Task DeleteAsync(string url, string data);
    }

    public class HttpCommitmentHelper : HttpClientBase, IHttpCommitmentHelper
    {
        public HttpCommitmentHelper(string clientToken)
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
