using System;

namespace SFA.DAS.TimeProvider.UnitTests
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