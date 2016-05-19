using System;
using NUnit.Framework;

namespace SFA.DAS.TimeProvider.UnitTests
{
    [TestFixture]
    public class DefaultTimeProviderTests
    {
        [Test]
        public void VerifyDefaultProviderIsUsed()
        {
            Assert.That(DateTimeProvider.Current is DefaultTimeProvider);
        }

        [Test]
        public void VerifyCurrentReturnsExpectedDateTime()
        {
            Assert.That(DateTimeProvider.Current.UtcNow, Is.EqualTo(DateTime.UtcNow).Within(1).Seconds);
        }
    }
}
