using System;

namespace SFA.DAS.VacancyServices.Wage
{
    public static class WageCalculator
    {
        public static decimal GetHourRate(decimal wage, WageUnit wageUnit, decimal hoursPerWeek)
        {
            switch (wageUnit)
            {
                case WageUnit.Weekly:
                    return RoundAsMoney(wage / hoursPerWeek);
                case WageUnit.Annually:
                    return RoundAsMoney(wage / WageConstants.WeeksPerYear / hoursPerWeek);
                case WageUnit.Monthly:
                    return RoundAsMoney(wage / WageConstants.WeeksPerYear * WageConstants.MonthPerYear / hoursPerWeek);

                case WageUnit.NotApplicable:
                    return 0;
                default:
                    throw new ArgumentOutOfRangeException(nameof(wageUnit), wageUnit, null);
            }
        }

        public static decimal GetYearlyRateFromHourlyRate(decimal hourlyRate, decimal weeklyHours)
        {
            decimal yearlyRate = hourlyRate * weeklyHours * WageConstants.WeeksPerYear;
            return RoundAsMoney(yearlyRate);
        }

        private static decimal RoundAsMoney(decimal valueToRound)
        {
            return Math.Round(valueToRound, 2, MidpointRounding.AwayFromZero);
        }
    }
}
