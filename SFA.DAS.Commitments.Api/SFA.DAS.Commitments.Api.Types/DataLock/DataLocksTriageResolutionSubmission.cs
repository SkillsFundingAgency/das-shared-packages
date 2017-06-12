using SFA.DAS.Commitments.Api.Types.DataLock.Types;

namespace SFA.DAS.Commitments.Api.Types.DataLock
{
    public class DataLocksTriageResolutionSubmission
    {
        public DataLockUpdateType DataLockUpdateType { get; set; }
        public TriageStatus TriageStatus { get; set; }
        public string UserId { get; set; }
    }
}