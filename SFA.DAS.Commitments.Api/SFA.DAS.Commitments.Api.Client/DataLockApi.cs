using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.Commitments.Api.Client.Configuration;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.DataLock;

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

        [Obsolete("Not in use")]
        public async Task<DataLockStatus> GetDataLock(long apprenticeshipId, long dataLockEventId)
        {
            var url = $"{_configuration.BaseUrl}api/apprenticeships/{apprenticeshipId}/datalocks/{dataLockEventId}";
            return await GetData<DataLockStatus>(url);
        }

        [Obsolete("Moved to employer / provider api")]
        public async Task<List<DataLockStatus>> GetDataLocks(long apprenticeshipId)
        {
            var url = $"{_configuration.BaseUrl}api/apprenticeships/{apprenticeshipId}/datalocks";
            return await GetData<List<DataLockStatus>>(url);
        }

        [Obsolete("Moved to employer / provider api")]
        public async Task<DataLockSummary> GetDataLockSummary(long apprenticeshipId)
        {
            var url = $"{_configuration.BaseUrl}api/apprenticeships/{apprenticeshipId}/datalocksummary";
            return await GetData<DataLockSummary>(url);
        }

        [Obsolete("Moved to provider api")]
        public async Task PatchDataLock(long apprenticeshipId, long dataLockEventId, DataLockTriageSubmission triageSubmission)
        {
            var url = $"{_configuration.BaseUrl}api/apprenticeships/{apprenticeshipId}/datalocks/{dataLockEventId}";
            await PatchModel(url, triageSubmission);
        }

        [Obsolete("Moved to provider api")]
        public async Task PatchDataLocks(long apprenticeshipId, DataLockTriageSubmission triageSubmission)
        {
            var url = $"{_configuration.BaseUrl}api/apprenticeships/{apprenticeshipId}/datalocks";
            await PatchModel(url, triageSubmission);
        }

        [Obsolete("Moved to employer api")]
        public async Task PatchDataLocks(long apprenticeshipId, DataLocksTriageResolutionSubmission submission)
        {
            var url = $"{_configuration.BaseUrl}api/apprenticeships/{apprenticeshipId}/datalocks/resolve";
            await PatchModel(url, submission);
        }
    }
}