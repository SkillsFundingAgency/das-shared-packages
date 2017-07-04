using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Commitments.Api.Types.DataLock;

namespace SFA.DAS.Commitments.Api.Client.Interfaces
{
    public interface IDataLockApi
    {
        [Obsolete("Not in use")]
        Task<DataLockStatus> GetDataLock(long apprenticeshipId, long dataLockEventId);

        [Obsolete("Moved to employer / provider api")]
        Task<List<DataLockStatus>> GetDataLocks(long apprenticeshipId);

        /// <summary>
        /// Get all <c>DataLockStatus</c> for an apprenticeship sorted in 2 lists.
        /// <para />
        /// </summary>
        /// <param name="apprenticeshipId"></param>
        /// <returns></returns>
        [Obsolete("Moved to employer / provider api")]
        Task<DataLockSummary> GetDataLockSummary(long apprenticeshipId);

        [Obsolete("Moved to provider api")]
        Task PatchDataLock(long apprenticeshipId, long dataLockEventId, DataLockTriageSubmission triageSubmission);

        /// <summary>
        /// Setting <c>TriageStatus</c> for all Price only <c>DataLockStatus</c>
        /// </summary>
        /// <param name="apprenticeshipId"></param>
        /// <param name="triageSubmission"></param>
        /// <returns></returns>
        [Obsolete("Moved to provider api")]
        Task PatchDataLocks(long apprenticeshipId, DataLockTriageSubmission triageSubmission);

        /// <summary>
        /// Approve or reject all <c>DataLockStatus</c> with a TriageStatus for an Apprenticeship
        /// </summary>
        /// <param name="apprenticeshipId"></param>
        /// <param name="submission"></param>
        /// <returns></returns>
        [Obsolete("Moved to employer api")]
        Task PatchDataLocks(long apprenticeshipId, DataLocksTriageResolutionSubmission submission);
    }
}