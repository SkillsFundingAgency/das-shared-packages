using System;
using SFA.DAS.TimeProvider;

namespace UsageSample
{
    public class FakeTimeProvider : DateTimeProvider
    {
        private readonly DateTime _currentTime;

        public FakeTimeProvider(DateTime currentTime)
        {
            _currentTime = currentTime;
        }

        public override DateTime UtcNow => _currentTime;
    }
}