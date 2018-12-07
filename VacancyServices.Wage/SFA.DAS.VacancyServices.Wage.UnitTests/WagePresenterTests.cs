using System;
using FluentAssertions;
using NUnit.Framework;

namespace SFA.DAS.VacancyServices.Wage.UnitTests
{
    [TestFixture]
    [Parallelizable]
    public class WagePresenterTests
    {
        private const string Space = " ";

        [TestCase(WageUnit.Weekly, WageConstants.WeeklyWageText)]
        [TestCase(WageUnit.Annually, WageConstants.AnnualWageText)]
        [TestCase(WageUnit.Monthly, WageConstants.MonthlyWageText)]
        [TestCase(WageUnit.NotApplicable, WageConstants.WageText)]
        public void ShouldGetHeaderDisplayTextForCustomWageType(WageUnit wageUnit, string expected)
        {
            // Arrange.

            // Act.
            string actual = wageUnit.GetHeaderDisplayText();

            // Assert.
            actual.Should().Be(expected);
        }

        [TestCase(WageUnit.Weekly, WageConstants.WeeklyWageText)]
        [TestCase(WageUnit.Monthly, WageConstants.MonthlyWageText)]
        [TestCase(WageUnit.Annually, WageConstants.AnnualWageText)]
        [TestCase(WageUnit.NotApplicable, WageConstants.WageText)]
        public void ShouldGetHeaderDisplayTextForWageUnit(WageUnit wageUnit, string expected)
        {
            // Act.
            string actual = wageUnit.GetHeaderDisplayText();

            // Assert.
            actual.Should().Be(expected);
        }

        [TestCase(WageUnit.Weekly, 5, "£5.00" + Space + WageConstants.PerWeekText)]
        [TestCase(WageUnit.Monthly, 5, "£5.00" + Space + WageConstants.PerMonthText)]
        [TestCase(WageUnit.Annually, 5, "£5.00" + Space + WageConstants.PerYearText)]
        [TestCase(WageUnit.NotApplicable, 5, "£5.00" + Space)]
        public void ShouldGetDisplayAmountWithFrequencyPostfix(WageUnit wageUnit, decimal displayAmount, string expected)
        {
            // Act.
            string actual =
                WagePresenter.GetCustomWageDisplayAmountWithFrequencyPostfix(wageUnit, displayAmount);

            // Assert.
            actual.Should().Be(expected);
        }

        [TestCase(WageUnit.Weekly, "£127.50" + Space + WageConstants.PerWeekText)]
        [TestCase(WageUnit.Annually, "£6,630.00" + Space + WageConstants.PerYearText)]
        public void ShouldGetDisplayAmountWithFrequencyPostfixNationalMinimums(WageUnit wageUnit, string expected)
        {
            // Act.
            string actual = WagePresenter.GetApprenticeshipMinimumDisplayAmountWithFrequencyPostfix(wageUnit, 37.5m, new DateTime(2016, 10, 30));

            // Assert.
            actual.Should().Be(expected);
        }

        [TestCase(WageUnit.Weekly, "£150.00 - £270.00" + Space + WageConstants.PerWeekText)]
        [TestCase(WageUnit.Annually, "£7,800.00 - £14,040.00" + Space + WageConstants.PerYearText)]
        public void ShouldGetDisplayAmountWithFrequencyPostfixNationalMinimums_After1stOct2016(WageUnit unit, string expected)
        {
            // Act.
            string actual = WagePresenter.GetNationalMinimumDisplayAmountWithFrequencyPostfix(unit, 37.5m, new DateTime(2016, 10, 1));

            // Assert.
            actual.Should().Be(expected);
        }

        [TestCase("Competitive salary", "Competitive salary")]
        [TestCase(null, "£Unknown")]
        public void ShouldGetLegacyTextDisplayText(string wageText, string expected)
        {
            // Act.
            string actual = WagePresenter.GetLegacyTextDisplayAmount( wageText);

            // Assert.
            actual.Should().Be(expected);
        }
        [TestCase("1", "2",  @"£1.00 - £2.00")]
        [TestCase( null, "2",  @"£Unknown - £2.00")]
        [TestCase( "1", null,  @"£1.00 - £Unknown")]
        public void ShouldGetCustomRangeDisplayText(string wageLowerString, string wageUpperString, string expected)
        {
            // Arrange.  This is frustrating. You cant use decimal parameters as decimals aren't primitives.
            decimal tempDecimal;
            decimal? wageAmountLower = null;
            decimal? wageAmountUpper = null;
            if (decimal.TryParse(wageLowerString, out tempDecimal))
            {
                wageAmountLower = tempDecimal;
            }

            if (decimal.TryParse(wageUpperString, out tempDecimal))
            {
                wageAmountUpper = tempDecimal;
            }

            // Act.
            string actual = WagePresenter.GetCustomRangeDisplayAmount(wageAmountLower, wageAmountUpper);

            // Assert.
            actual.Should().Be(expected);
        }

        [TestCase("0", "£Unknown")]
        public void ShouldGetApprenticeshipMinimumDisplayText(string hoursPerWeekString, string expected)
        {
            // Arrange.  This is frustrating. You cant use decimal parameters as decimals aren't primitives.
            decimal tempDecimal;
            decimal? hoursPerWeek = null;

            if (decimal.TryParse(hoursPerWeekString, out tempDecimal))
            {
                hoursPerWeek = tempDecimal;
            }

            // Act.
            string actual = WagePresenter.GetApprenticeshipMinimumDisplayAmount(WageUnit.Annually, Convert.ToDecimal(hoursPerWeek), null);

            // Assert.
            actual.Should().Be(expected);
        }

        [TestCase("0", "£Unknown")]
        public void ShouldGetNationalMinimumDisplayText(string hoursPerWeekString, string expected)
        {
            // Arrange.  This is frustrating. You cant use decimal parameters as decimals aren't primitives.
            decimal tempDecimal;
            decimal? hoursPerWeek = null;

            if (decimal.TryParse(hoursPerWeekString, out tempDecimal))
            {
                hoursPerWeek = tempDecimal;
            }

            // Act.
            string actual = WagePresenter.GetNationalMinimumDisplayAmount(WageUnit.Annually, Convert.ToDecimal(hoursPerWeek), null);

            // Assert.
            actual.Should().Be(expected);
        }
    }
}
