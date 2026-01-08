using System;

namespace SFA.DAS.GovUK.Auth.Helper
{
    public interface IDateTimeHelper
    {
        DateTime Now { get; }
        DateTime UtcNow { get;  }

        DateTimeOffset NowOffset { get; }
        DateTimeOffset UtcNowOffset { get;  }
    }
}