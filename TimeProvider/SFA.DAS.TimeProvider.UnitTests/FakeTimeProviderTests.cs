using System;
using NUnit.Framework;

namespace SFA.DAS.TimeProvider.UnitTests
{
    [TestFixture]
    public class FakeTimeProviderTests
    {
        [TearDown]
        public void Teardown()
        {
            TimeProvider.ResetToDefault();
        }

        [Test]
        public void SettingTheProviderToFakeReturnsExpectedProvider()
        {
            var newDateTime = DateTime.Now.AddDays(-1);

            TimeProvider.Current = new FakeTimeProvider(newDateTime);

            Assert.That(TimeProvider.Current is FakeTimeProvider);
        }

        [Test]
        public void SettingTheDateReturnsThatDate()
        {
            var newDateTime = DateTime.Now.AddDays(-1);

            TimeProvider.Current = new FakeTimeProvider(newDateTime);

            Assert.That(TimeProvider.Current.UtcNow, Is.EqualTo(newDateTime));
        }

        [Test]
        public void ResetingTheProviderReturnsTheCurrentDate()
        {
            var newDateTime = DateTime.Now.AddDays(-1);

            TimeProvider.Current = new FakeTimeProvider(newDateTime);

            TimeProvider.ResetToDefault();

            Assert.That(TimeProvider.Current.UtcNow, Is.Not.EqualTo(newDateTime));
            Assert.That(TimeProvider.Current.UtcNow, Is.EqualTo(DateTime.UtcNow).Within(1).Seconds);
            Assert.That(TimeProvider.Current is DefaultTimeProvider);
        }
    }
}