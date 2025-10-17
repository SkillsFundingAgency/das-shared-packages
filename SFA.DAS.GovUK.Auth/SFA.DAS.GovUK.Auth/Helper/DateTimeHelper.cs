using System;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.GovUK.Auth.Helper
{
    [ExcludeFromCodeCoverage]
    public class DateTimeHelper : IDateTimeHelper
    {
        public DateTime Now => DateTime.Now;
        public DateTime UtcNow => DateTime.UtcNow;

        public DateTimeOffset NowOffset => DateTimeOffset.Now;
        public DateTimeOffset UtcNowOffset => DateTimeOffset.UtcNow;
    }
}
