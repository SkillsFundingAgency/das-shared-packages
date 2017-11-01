using NUnit.Framework;
using SFA.DAS.Commitments.Infrastructure.Services;
using System;
using FluentAssertions;

namespace SFA.DAS.CurrentTimeService.UnitTests.CurrentTimeTest
{
    [TestFixture]
    public class WhenGettingCurrentTime
    {

        [Test]
        public void ShouldReturnSpecifiedTime()
        {
            var specifiedTime = new DateTime(2017, 10, 1, 22, 0,0);

            var sut = new CurrentDateTime(specifiedTime);

            var currentTime = sut.Now;

            currentTime.Should().Be(specifiedTime);
        }

    }
}
