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
        public void ShouldReturnOldRatesUptoMarch2020()
        {
            var result = NationalMinimumWageService.GetHourlyRates(new DateTime(2020, 3, 31));

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

        [Test]
        public void ShouldReturnOldRatesUptoMarch2021()
        {
            var result = NationalMinimumWageService.GetHourlyRates(new DateTime(2021, 3, 31));

            result.ApprenticeMinimumWage.Should().Be(4.15m);
            result.Under18NationalMinimumWage.Should().Be(4.55m);
            result.Between18AndUnder21NationalMinimumWage.Should().Be(6.45m);
            result.Between21AndUnder25NationalMinimumWage.Should().Be(8.2m);
            result.Over25NationalMinimumWage.Should().Be(8.72m);
        }

        [Test]
        public void ShouldReturnNewRatesFromApril2021()
        {
            var result = NationalMinimumWageService.GetHourlyRates(new DateTime(2021, 4, 1));

            result.ApprenticeMinimumWage.Should().Be(4.30m);
            result.Under18NationalMinimumWage.Should().Be(4.62m);
            result.Between18AndUnder21NationalMinimumWage.Should().Be(6.56m);
            result.Between21AndUnder25NationalMinimumWage.Should().Be(8.36m);
            result.Over25NationalMinimumWage.Should().Be(8.91m);
        }

        [Test]
        public void ShouldReturnOldRatesUptoMarch2022()
        {
            var result = NationalMinimumWageService.GetHourlyRates(new DateTime(2022, 3, 31));

            result.ApprenticeMinimumWage.Should().Be(4.30m);
            result.Under18NationalMinimumWage.Should().Be(4.62m);
            result.Between18AndUnder21NationalMinimumWage.Should().Be(6.56m);
            result.Between21AndUnder25NationalMinimumWage.Should().Be(8.36m);
            result.Over25NationalMinimumWage.Should().Be(8.91m);
        }

        [Test]
        public void ShouldReturnNewRatesFromApril2022()
        {
            var result = NationalMinimumWageService.GetHourlyRates(new DateTime(2022, 4, 1));

            result.ApprenticeMinimumWage.Should().Be(4.81m);
            result.Under18NationalMinimumWage.Should().Be(4.81m);
            result.Between18AndUnder21NationalMinimumWage.Should().Be(6.83m);
            result.Between21AndUnder25NationalMinimumWage.Should().Be(9.18m);
            result.Over25NationalMinimumWage.Should().Be(9.50m);
        }

        [Test]
        public void ShouldReturnOldRatesUptoMarch2023()
        {
            var result = NationalMinimumWageService.GetHourlyRates(new DateTime(2023, 3, 31));

            result.ApprenticeMinimumWage.Should().Be(4.81m);
            result.Under18NationalMinimumWage.Should().Be(4.81m);
            result.Between18AndUnder21NationalMinimumWage.Should().Be(6.83m);
            result.Between21AndUnder25NationalMinimumWage.Should().Be(9.18m);
            result.Over25NationalMinimumWage.Should().Be(9.50m);
        }

        [Test]
        public void ShouldReturnNewRatesFromApril2023()
        {
            var result = NationalMinimumWageService.GetHourlyRates(new DateTime(2023, 4, 1));

            result.ApprenticeMinimumWage.Should().Be(5.28m);
            result.Under18NationalMinimumWage.Should().Be(5.28m);
            result.Between18AndUnder21NationalMinimumWage.Should().Be(7.49m);
            result.Between21AndUnder25NationalMinimumWage.Should().Be(10.18m);
            result.Over25NationalMinimumWage.Should().Be(10.42m);
        }
        [Test]
        public void ShouldReturnOldRatesUptoMarch2024()
        {
            var result = NationalMinimumWageService.GetHourlyRates(new DateTime(2024, 3, 31));

            result.ApprenticeMinimumWage.Should().Be(5.28m);
            result.Under18NationalMinimumWage.Should().Be(5.28m);
            result.Between18AndUnder21NationalMinimumWage.Should().Be(7.49m);
            result.Between21AndUnder25NationalMinimumWage.Should().Be(10.18m);
            result.Over25NationalMinimumWage.Should().Be(10.42m);
        }

        [Test]
        public void ShouldReturnNewRatesFromApril2024()
        {
            var result = NationalMinimumWageService.GetHourlyRates(new DateTime(2024, 4, 1));

            result.ApprenticeMinimumWage.Should().Be(6.40m);
            result.Under18NationalMinimumWage.Should().Be(6.40m);
            result.Between18AndUnder21NationalMinimumWage.Should().Be(8.60m);
            result.Between21AndUnder25NationalMinimumWage.Should().Be(11.44m);
            result.Over25NationalMinimumWage.Should().Be(11.44m);
        }
    }
}
