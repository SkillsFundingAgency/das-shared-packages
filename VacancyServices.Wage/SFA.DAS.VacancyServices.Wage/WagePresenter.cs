using System;
using System.Globalization;

namespace SFA.DAS.VacancyServices.Wage
{
    public static class WagePresenter
    {
        private const string CurrencyCulture = "en-GB";
        private const string CurrencyStringFormat = "C";

        public static string GetDisplayTextFrequencyPostfix(WageType type, WageUnit unit, WageDetails wageDetails)
        {
            string postfix = unit.GetWagePostfix();

            string displayAmount = GetDisplayText(type, unit, wageDetails);
            if (string.IsNullOrWhiteSpace(displayAmount))
                return postfix;

            return $"{displayAmount} {postfix}";
        }

        public static string GetDisplayText(WageType type, WageUnit unit, WageDetails wageDetails)
        {
            switch (type)
            {
                case WageType.Custom:
                    return FormatWageAmount(wageDetails.Amount);

                case WageType.ApprenticeshipMinimum:

                    if (!wageDetails.HoursPerWeek.HasValue || wageDetails.HoursPerWeek.Value == 0)
                        return $"£{WageConstants.UnknownText}";

                    if (unit == WageUnit.Annually)
                        return GetYearlyApprenticeshipMinimumWage(wageDetails.HoursPerWeek.Value, wageDetails.StartDate);

                    return GetWeeklyApprenticeshipMinimumWage(wageDetails.HoursPerWeek.Value, wageDetails.StartDate);

                case WageType.NationalMinimum:
                    if (!wageDetails.HoursPerWeek.HasValue || wageDetails.HoursPerWeek.Value == 0)
                        return $"£{WageConstants.UnknownText}";

                    if (unit == WageUnit.Annually)
                        return GetYearlyNationalMinimumWage(wageDetails.HoursPerWeek.Value, wageDetails.StartDate);

                    return GetWeeklyNationalMinimumWage(wageDetails.HoursPerWeek.Value, wageDetails.StartDate);

                case WageType.Text:

                    //if it's unknown, return standard unknown text
                    string displayText = wageDetails.Text ?? $"£{WageConstants.UnknownText}";

                    //if it's not unknown, then prepend a '£' sign to its decimal value.
                    decimal wageDecimal;

                    //if it's already got a '£' sign, or is text, fail to parse and all is good => return value.
                    if (decimal.TryParse(displayText, out wageDecimal))
                        displayText = FormatWageAmount(wageDecimal);

                    return displayText;

                case WageType.CustomRange:
                    return
                        $"{FormatWageAmount(wageDetails.LowerBound)} - {FormatWageAmount(wageDetails.UpperBound)}";

                case WageType.CompetitiveSalary:
                    return WageConstants.CompetitiveSalaryText;

                case WageType.ToBeAgreedUponAppointment:
                    return WageConstants.ToBeAGreedUponAppointmentText;

                case WageType.Unwaged:
                    return WageConstants.UnwagedText;

                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type,
                        $"Invalid Wage Type: {type}");
            }
        }

        public static string GetWeeklyNationalMinimumWageMaximum(decimal hoursPerWeek, DateTime possibleStartDate)
        {
            NationalMinimumWageRates wageRange = NationalMinimumWageService.GetHourlyRates(possibleStartDate);

            string higherRange = FormatWageAmount(wageRange.Over25NationalMinimumWage * hoursPerWeek);

            return $"{higherRange}";
        }

        public static string GetWeeklyApprenticeshipMinimumWage(decimal hoursPerWeek, DateTime possibleStartDate)
        {
            NationalMinimumWageRates wageRange = NationalMinimumWageService.GetHourlyRates(possibleStartDate);

            return $"{FormatWageAmount(wageRange.ApprenticeMinimumWage * hoursPerWeek)}";
        }

        public static string GetYearlyApprenticeshipMinimumWage(decimal hoursPerWeek, DateTime possibleStartDate)
        {
            NationalMinimumWageRates wageRange = NationalMinimumWageService.GetHourlyRates(possibleStartDate);

            return
                $"{FormatWageAmount(WageCalculator.GetYearlyRateFromHourlyRate(wageRange.ApprenticeMinimumWage, hoursPerWeek))}";
        }

        private static string GetWeeklyNationalMinimumWage(decimal hoursPerWeek, DateTime possibleStartDate)
        {
            NationalMinimumWageRates wageRange = NationalMinimumWageService.GetHourlyRates(possibleStartDate);

            string lowerRange = FormatWageAmount(wageRange.Under18NationalMinimumWage * hoursPerWeek);
            string higherRange = FormatWageAmount(wageRange.Over25NationalMinimumWage * hoursPerWeek);

            return $"{lowerRange} - {higherRange}";
        }

        private static string GetYearlyNationalMinimumWage(decimal hoursPerWeek, DateTime possibleStartDate)
        {
            NationalMinimumWageRates wageRange = NationalMinimumWageService.GetHourlyRates(possibleStartDate);

            string lowerRange =
                FormatWageAmount(WageCalculator.GetYearlyRateFromHourlyRate(wageRange.Under18NationalMinimumWage, hoursPerWeek));
            string higherRange =
                FormatWageAmount(WageCalculator.GetYearlyRateFromHourlyRate(wageRange.Over25NationalMinimumWage, hoursPerWeek));

            return $"{lowerRange} - {higherRange}";
        }

        private static string FormatWageAmount(decimal? wageAmount)
        {
            return wageAmount?.ToString(CurrencyStringFormat, new CultureInfo(CurrencyCulture)) ?? $"£{WageConstants.UnknownText}";
        }

        private static string GetWagePostfix(this WageUnit wageUnit)
        {
            switch (wageUnit)
            {
                case WageUnit.Annually:
                    return WageConstants.PerYearText;

                case WageUnit.Monthly:
                    return WageConstants.PerMonthText;

                case WageUnit.Weekly:
                    return WageConstants.PerWeekText;

                case WageUnit.NotApplicable:
                    return string.Empty;

                default:
                    throw new ArgumentOutOfRangeException(nameof(wageUnit), $"Invalid Wage Unit: {wageUnit}");
            }
        }
    }
}