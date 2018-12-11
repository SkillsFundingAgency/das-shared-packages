using System;

namespace SFA.DAS.VacancyServices.Wage
{
    public static class WageUnitExtensions
    {
        public static string GetHeaderDisplayText(this WageUnit wageUnit)
        {
            switch (wageUnit)
            {
                case WageUnit.Annually:
                    return WageConstants.AnnualWageText;

                case WageUnit.Monthly:
                    return WageConstants.MonthlyWageText;

                case WageUnit.Weekly:
                    return WageConstants.WeeklyWageText;

                case WageUnit.NotApplicable:
                    return WageConstants.WageText;

                default:
                    throw new ArgumentOutOfRangeException(nameof(wageUnit), $"Invalid Wage Unit: {wageUnit}");
            }
        }
    }
}
