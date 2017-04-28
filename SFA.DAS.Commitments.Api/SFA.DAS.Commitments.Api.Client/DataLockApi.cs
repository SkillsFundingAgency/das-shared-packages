using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.Commitments.Api.Client.Configuration;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.DataLock;
using SFA.DAS.Commitments.Api.Types.DataLock.Types;

namespace SFA.DAS.Commitments.Api.Client
{
    public class DataLockApi : HttpClientBase, IDataLockApi
    {
        private readonly ICommitmentsApiClientConfiguration _configuration;

        public DataLockApi(ICommitmentsApiClientConfiguration configuration) : base(configuration.ClientToken)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            _configuration = configuration;
        }

        public async Task<DataLockStatus> GetDataLock(long apprenticeshipId, long dataLockEventId)
        {
            var url = $"{_configuration.BaseUrl}api/apprenticeships/{apprenticeshipId}/datalocks/{dataLockEventId}";
            return await GetDataLock(url);
        }

        public async Task<List<DataLockStatus>> GetDataLocks(long apprenticeshipId)
        {
            var url = $"{_configuration.BaseUrl}api/apprenticeships/{apprenticeshipId}/datalocks";
            return await GetDataLocks(url);
        }

        public async Task PatchDataLock(long apprenticeshipId, long dataLockEventId, DataLockTriageSubmission triageSubmission)
        {
            var url = $"{_configuration.BaseUrl}api/apprenticeships/{apprenticeshipId}/datalocks/{dataLockEventId}";
            await PatchDataLock(url, triageSubmission);
        }

        private async Task<DataLockStatus> GetDataLock(string url)
        {
            var content = await GetAsync(url);
            return JsonConvert.DeserializeObject<DataLockStatus>(content);
        }

        private async Task<List<DataLockStatus>> GetDataLocks(string url)
        {
            var content = await GetAsync(url);
            return JsonConvert.DeserializeObject<List<DataLockStatus>>(content);
        }

        private async Task PatchDataLock(string url, DataLockTriageSubmission triageSubmission)
        {
            var data = JsonConvert.SerializeObject(triageSubmission);
            await PatchAsync(url, data);
        }
    }
}
