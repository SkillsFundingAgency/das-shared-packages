using System.Collections.Generic;

namespace SFA.DAS.Commitments.Api.Types.DataLock
{
    public class DataLockSummary
    {
        public IEnumerable<DataLockStatus> DataLockWithCourseMismatch { get; set; }

        public IEnumerable<DataLockStatus> DataLockWithOnlyPriceMismatch { get; set; }
    }
}
