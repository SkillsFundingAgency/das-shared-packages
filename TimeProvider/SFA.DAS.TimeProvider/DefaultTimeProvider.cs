using System;

namespace SFA.DAS.TimeProvider
{
    public class DefaultTimeProvider : DateTimeProvider
    {
        public override DateTime UtcNow => DateTime.UtcNow;
    }
}