using System;
using FluentAssertions;
using NUnit.Framework;

namespace SFA.DAS.VacancyServices.Wage.UnitTests
{
    [TestFixture]
    public class NationalMinimumWageServiceTests
    {
        [Test]
        public void ShouldReturnOldRatesUptoMarch2019()
        {
            var result = NationalMinimumWageService.GetHourlyRates(new DateTime(2019, 3, 31));

            result.ApprenticeMinimumWage.Should().Be(3.7m);
            result.Under18NationalMinimumWage.Should().Be(4.2m);
            result.Between18AndUnder21NationalMinimumWage.Should().Be(5.9m);
            result.Between21AndUnder25NationalMinimumWage.Should().Be(7.38m);
            result.Over25NationalMinimumWage.Should().Be(7.83m);
        }

        [Test]
        public void ShouldReturnNewRatesFromApril2019()
        {
            var result = NationalMinimumWageService.GetHourlyRates(new DateTime(2019, 4, 1));

            result.ApprenticeMinimumWage.Should().Be(3.9m);
            result.Under18NationalMinimumWage.Should().Be(4.35m);
            result.Between18AndUnder21NationalMinimumWage.Should().Be(6.15m);
            result.Between21AndUnder25NationalMinimumWage.Should().Be(7.7m);
            result.Over25NationalMinimumWage.Should().Be(8.21m);
        }

        [Test]
        public void ShouldReturnNewRatesFromApril2020()
        {
            var result = NationalMinimumWageService.GetHourlyRates(new DateTime(2020, 4, 1));

            result.ApprenticeMinimumWage.Should().Be(4.15m);
            result.Under18NationalMinimumWage.Should().Be(4.55m);
            result.Between18AndUnder21NationalMinimumWage.Should().Be(6.45m);
            result.Between21AndUnder25NationalMinimumWage.Should().Be(8.2m);
            result.Over25NationalMinimumWage.Should().Be(8.72m);
        }
    }
}
