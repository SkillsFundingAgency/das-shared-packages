using SFA.DAS.Commitments.Api.Types.DataLock.Types;

namespace SFA.DAS.Commitments.Api.Types.DataLock
{
    public class DataLockTriageSubmission
    {
        public TriageStatus TriageStatus { get; set; }
        public string UserId { get; set; }
    }
}
