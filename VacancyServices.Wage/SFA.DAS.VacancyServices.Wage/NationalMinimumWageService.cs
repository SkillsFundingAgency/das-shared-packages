using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.VacancyServices.Wage
{
    public static class NationalMinimumWageService
    {
        private const int WeeksPerYear = 52;
        private static readonly NationalMinimumWageRates[] nationalMinimumWageRates;

        static NationalMinimumWageService()
        {
            nationalMinimumWageRates = GetNationalMinimumWageRates();
        }

        /// <summary>Gets the hourly rates as at a specified date.</summary>
        /// <param name="dateTime">The specified date for the rates</param>
        /// <returns>The rates as at the specified date</returns>
        public static NationalMinimumWageRates GetHourlyRates(DateTime dateTime)
        {
            return nationalMinimumWageRates.Single(r => dateTime >= r.ValidFrom && dateTime < r.ValidTo);
        }

        /// <summary>Gets all the rates asynchronously</summary>
        /// <returns>An immutable array of rates</returns>
        public static Task<ImmutableArray<NationalMinimumWageRates>> GetRatesAsync()
        {
            return Task.FromResult(nationalMinimumWageRates.ToImmutableArray());
        }

        /// <summary>
        ///     Gets the weekly rates as at a specified date and hours per week.
        /// </summary>
        /// <param name="dateTime">The specified date for the rates</param>
        /// <param name="hoursPerWeek">The specified hours per week</param>
        /// <returns>The rates as at the specified date or the latest rates if no date specified</returns>
        public static NationalMinimumWageRates GetWeeklyRates(DateTime dateTime, decimal hoursPerWeek)
        {
            NationalMinimumWageRates hourlyRates = nationalMinimumWageRates.Single(r => dateTime >= r.ValidFrom && dateTime < r.ValidTo);
            return new NationalMinimumWageRates
            (
                hourlyRates.ValidFrom,
                hourlyRates.ValidFrom,
                hourlyRates.ApprenticeMinimumWage * hoursPerWeek,
                hourlyRates.Under18NationalMinimumWage * hoursPerWeek,
                hourlyRates.Between18AndUnder21NationalMinimumWage * hoursPerWeek,
                hourlyRates.Between21AndUnder25NationalMinimumWage * hoursPerWeek,
                hourlyRates.Over25NationalMinimumWage * hoursPerWeek
            );
        }

        /// <summary>
        ///     Gets the annual rates as at a specified date and hours per week.  If no date specified then will return the
        ///     latest rates. If no hours specified then will use 37.5 hours as a standard week.
        /// </summary>
        /// <param name="dateTime">The specified date for the rates</param>
        /// <param name="hoursPerWeek">The specified hours per week</param>
        /// <returns>The rates as at the specified date or the latest rates if no date specified</returns>
        public static NationalMinimumWageRates GetAnnualRates(DateTime dateTime, decimal hoursPerWeek)
        {
            NationalMinimumWageRates weeklyRates = GetWeeklyRates(dateTime, hoursPerWeek);
            return new NationalMinimumWageRates
            (
                weeklyRates.ValidFrom,
                weeklyRates.ValidFrom,
                weeklyRates.ApprenticeMinimumWage * WeeksPerYear,
                weeklyRates.Under18NationalMinimumWage * WeeksPerYear,
                weeklyRates.Between18AndUnder21NationalMinimumWage * WeeksPerYear,
                weeklyRates.Between21AndUnder25NationalMinimumWage * WeeksPerYear,
                weeklyRates.Over25NationalMinimumWage * WeeksPerYear
            );
        }

        private static NationalMinimumWageRates[] GetNationalMinimumWageRates()
        {
            return new[]
            {
                new NationalMinimumWageRates
                (
                    DateTime.MinValue,
                    new DateTime(2016, 10, 1),
                    3.30m,
                    3.87m,
                    5.30m,
                    6.70m,
                    7.20m
                ),
                //October 1st, 2016
                new NationalMinimumWageRates
                (
                    new DateTime(2016, 10, 1),
                    new DateTime(2017, 04, 1),
                    3.40m,
                    4.00m,
                    5.55m,
                    6.95m,
                    7.20m
                ),
                //April 1st, 2017
                new NationalMinimumWageRates
                (
                    new DateTime(2017, 04, 1),
                    new DateTime(2018, 04, 1),
                    3.50m,
                    4.05m,
                    5.60m,
                    7.05m,
                    7.50m
                ),
                //April 1st, 2018
                new NationalMinimumWageRates
                (
                    new DateTime(2018, 04, 1),
                    DateTime.MaxValue,
                    3.70m,
                    4.20m,
                    5.90m,
                    7.38m,
                    7.83m
                )
            };
        }
    }
}
