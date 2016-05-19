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
            DateTimeProvider.ResetToDefault();
        }

        [Test]
        public void SettingTheProviderToFakeReturnsExpectedProvider()
        {
            var newDateTime = DateTime.Now.AddDays(-1);

            DateTimeProvider.Current = new FakeTimeProvider(newDateTime);

            Assert.That(DateTimeProvider.Current is FakeTimeProvider);
        }

        [Test]
        public void SettingTheDateReturnsThatDate()
        {
            var newDateTime = DateTime.Now.AddDays(-1);

            DateTimeProvider.Current = new FakeTimeProvider(newDateTime);

            Assert.That(DateTimeProvider.Current.UtcNow, Is.EqualTo(newDateTime));
        }

        [Test]
        public void ResetingTheProviderReturnsTheCurrentDate()
        {
            var newDateTime = DateTime.Now.AddDays(-1);

            DateTimeProvider.Current = new FakeTimeProvider(newDateTime);

            DateTimeProvider.ResetToDefault();

            Assert.That(DateTimeProvider.Current.UtcNow, Is.Not.EqualTo(newDateTime));
            Assert.That(DateTimeProvider.Current.UtcNow, Is.EqualTo(DateTime.UtcNow).Within(1).Seconds);
            Assert.That(DateTimeProvider.Current is DefaultTimeProvider);
        }
    }
}