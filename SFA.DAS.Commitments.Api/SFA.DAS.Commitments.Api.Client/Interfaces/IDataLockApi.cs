using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Commitments.Api.Types.DataLock;

namespace SFA.DAS.Commitments.Api.Client.Interfaces
{
    public interface IDataLockApi
    {
        Task<DataLockStatus> GetDataLock(long apprenticeshipId, long dataLockEventId);
        Task<List<DataLockStatus>> GetDataLocks(long apprenticeshipId);
        Task PatchDataLock(long apprenticeshipId, long dataLockEventId, DataLockTriageSubmission triageSubmission);
    }
}
