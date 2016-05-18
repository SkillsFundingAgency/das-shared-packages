using System;

namespace SFA.DAS.TimeProvider
{
    public class DefaultTimeProvider : TimeProvider
    {
        public override DateTime UtcNow => DateTime.UtcNow;
    }
}