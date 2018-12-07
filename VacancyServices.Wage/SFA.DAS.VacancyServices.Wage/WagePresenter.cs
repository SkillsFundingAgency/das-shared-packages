using System;
using System.Globalization;

namespace SFA.DAS.VacancyServices.Wage
{
    public static class WagePresenter
    {
        private const string CurrencyCulture = "en-GB";
        private const string CurrencyStringFormat = "C";

        public static string GetCustomWageDisplayAmountWithFrequencyPostfix(WageUnit unit, decimal? amount)
        {
            string postfix = unit.GetWagePostfix();

            string displayAmount = GetCustomWageDisplayAmount(amount);
            if (string.IsNullOrWhiteSpace(displayAmount))
                return postfix;

            return $"{displayAmount} {postfix}";
        }

        public static string GetApprenticeshipMinimumDisplayAmountWithFrequencyPostfix(WageUnit unit, decimal? hoursPerWeek, DateTime? possibleDateTime)
        {
            string postfix = unit.GetWagePostfix();

            string displayAmount = GetApprenticeshipMinimumDisplayAmount(unit, hoursPerWeek, possibleDateTime);
            if (string.IsNullOrWhiteSpace(displayAmount))
                return postfix;

            return $"{displayAmount} {postfix}";
        }

        public static string GetNationalMinimumDisplayAmountWithFrequencyPostfix(WageUnit unit, decimal? hoursPerWeek, DateTime? possibleDateTime)
        {
            string postfix = unit.GetWagePostfix();

            string displayAmount = GetNationalMinimumDisplayAmount(unit, hoursPerWeek, possibleDateTime);
            if (string.IsNullOrWhiteSpace(displayAmount))
                return postfix;

            return $"{displayAmount} {postfix}";
        }

        public static string GetCustomRangeDisplayAmountWithFrequencyPostfix(WageUnit unit, decimal? amountLowerBound, decimal? amountUpperBound)
        {
            string postfix = unit.GetWagePostfix();

            string displayAmount = GetCustomRangeDisplayAmount(amountLowerBound, amountUpperBound);
            if (string.IsNullOrWhiteSpace(displayAmount))
                return postfix;

            return $"{displayAmount} {postfix}";
        }

        public static string GetLegacyTextDisplayAmountWithFrequencyPostfix(WageUnit unit, string text)
        {
            string postfix = unit.GetWagePostfix();

            string displayAmount = GetLegacyTextDisplayAmount(text);
            if (string.IsNullOrWhiteSpace(displayAmount))
                return postfix;

            return $"{displayAmount} {postfix}";
        }

        public static string GetCustomWageDisplayAmount(decimal? amount)
        {
            return GetDisplayAmount(WageType.Custom, WageUnit.NotApplicable, amount, null, null, null, null, null);
        }

        public static string GetApprenticeshipMinimumDisplayAmount(WageUnit wageUnit, decimal? hoursPerWeek,
            DateTime? expectedStartDate)
        {
            return GetDisplayAmount(WageType.ApprenticeshipMinimum, wageUnit, null, null, null, null, hoursPerWeek,
                expectedStartDate);
        }

        public static string GetNationalMinimumDisplayAmount(WageUnit wageUnit, decimal? hoursPerWeek, DateTime? expectedStartDate)
        {
            return GetDisplayAmount(WageType.NationalMinimum, wageUnit, null, null, null, null, hoursPerWeek, expectedStartDate);
        }

        public static string GetCustomRangeDisplayAmount(decimal? lowerAmount, decimal? upperAmount)
        {
            return GetDisplayAmount(WageType.CustomRange, WageUnit.NotApplicable, null, lowerAmount, upperAmount, null, null,
                null);
        }

        public static string GetLegacyTextDisplayAmount(string text)
        {
            return GetDisplayAmount(WageType.LegacyText, WageUnit.NotApplicable, null, null, null, text, null, null);
        }

        public static string GetWeeklyNationalMinimumWageMaximum(decimal hoursPerWeek, DateTime? possibleStartDate)
        {
            NationalMinimumWageRates wageRange = NationalMinimumWageService.GetHourlyRates(possibleStartDate);

            string higherRange = FormatWageAmount(wageRange.Over24NationalMinimumWage * hoursPerWeek);

            return $"{higherRange}";
        }

        public static string GetWeeklyApprenticeshipMinimumWage(decimal hoursPerWeek, DateTime? possibleStartDate)
        {
            NationalMinimumWageRates wageRange = NationalMinimumWageService.GetHourlyRates(possibleStartDate);

            return $"{FormatWageAmount(wageRange.ApprenticeMinimumWage * hoursPerWeek)}";
        }

        public static string GetYearlyApprenticeshipMinimumWage(decimal hoursPerWeek, DateTime? possibleStartDate)
        {
            NationalMinimumWageRates wageRange = NationalMinimumWageService.GetHourlyRates(possibleStartDate);

            return
                $"{FormatWageAmount(WageCalculator.GetYearlyRateFromHourlyRate(wageRange.ApprenticeMinimumWage, hoursPerWeek))}";
        }

        private static string GetDisplayAmount(WageType type, WageUnit unit, decimal? amount, decimal? amountLowerBound,
            decimal? amountUpperBound,
            string text, decimal? hoursPerWeek, DateTime? possibleDateTime)
        {
            switch (type)
            {
                case WageType.LegacyWeekly:
                case WageType.Custom:
                    return FormatWageAmount(amount);

                case WageType.ApprenticeshipMinimum:

                    if (!hoursPerWeek.HasValue || hoursPerWeek.Value == 0)
                        return $"£{WageConstants.UnknownText}";

                    if (unit == WageUnit.Annually)
                        return GetYearlyApprenticeshipMinimumWage(hoursPerWeek.Value, possibleDateTime);

                    return GetWeeklyApprenticeshipMinimumWage(hoursPerWeek.Value, possibleDateTime);

                case WageType.NationalMinimum:
                    if (!hoursPerWeek.HasValue || hoursPerWeek.Value == 0)
                        return $"£{WageConstants.UnknownText}";

                    if (unit == WageUnit.Annually)
                        return GetYearlyNationalMinimumWage(hoursPerWeek.Value, possibleDateTime);

                    return GetWeeklyNationalMinimumWage(hoursPerWeek.Value, possibleDateTime);

                case WageType.LegacyText:

                    //if it's unknown, return standard unknown text
                    string displayText = text ?? $"£{WageConstants.UnknownText}";

                    //if it's not unknown, then prepend a '£' sign to its decimal value.
                    decimal wageDecimal;

                    //if it's already got a '£' sign, or is text, fail to parse and all is good => return value.
                    if (decimal.TryParse(displayText, out wageDecimal))
                        displayText = FormatWageAmount(wageDecimal);

                    return displayText;

                case WageType.CustomRange:
                    return
                        $"{FormatWageAmount(amountLowerBound)} - {FormatWageAmount(amountUpperBound)}";

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

        private static string GetWeeklyNationalMinimumWage(decimal hoursPerWeek, DateTime? possibleStartDate)
        {
            NationalMinimumWageRates wageRange = NationalMinimumWageService.GetHourlyRates(possibleStartDate);

            string lowerRange = FormatWageAmount(wageRange.Under18NationalMinimumWage * hoursPerWeek);
            string higherRange = FormatWageAmount(wageRange.Over24NationalMinimumWage * hoursPerWeek);

            return $"{lowerRange} - {higherRange}";
        }

        private static string GetYearlyNationalMinimumWage(decimal hoursPerWeek, DateTime? possibleStartDate)
        {
            NationalMinimumWageRates wageRange = NationalMinimumWageService.GetHourlyRates(possibleStartDate);

            string lowerRange =
                FormatWageAmount(WageCalculator.GetYearlyRateFromHourlyRate(wageRange.Under18NationalMinimumWage, hoursPerWeek));
            string higherRange =
                FormatWageAmount(WageCalculator.GetYearlyRateFromHourlyRate(wageRange.Over24NationalMinimumWage, hoursPerWeek));

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
