using System;

namespace SFA.DAS.Commitments.Api.Types.DataLock.Types
{
    [Flags]
    public enum DataLockErrorCode
    {
        None = 0,
        Dlock01 = 1,
        Dlock02 = 2,
        Dlock03 = 4,
        Dlock04 = 8,
        Dlock05 = 16,
        Dlock06 = 32,
        Dlock07 = 64,
        Dlock08 = 128,
        Dlock09 = 256,
        Dlock10 = 512
    }
}
