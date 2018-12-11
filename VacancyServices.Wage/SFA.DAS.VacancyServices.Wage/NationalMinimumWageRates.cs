using System;

namespace SFA.DAS.VacancyServices.Wage
{
    public struct NationalMinimumWageRates
    {
        internal NationalMinimumWageRates(DateTime validFrom, DateTime validTo, decimal apprenticeMinimumWage, decimal under18NationalMinimumWage, decimal between18AndUnder21NationalMinimumWage, decimal between21AndUnder25NationalMinimumWage, decimal over25NationalMinimumWage)
        {
            ValidFrom = validFrom;
            ValidTo = validTo;
            ApprenticeMinimumWage = apprenticeMinimumWage;
            Under18NationalMinimumWage = under18NationalMinimumWage;
            Between18AndUnder21NationalMinimumWage = between18AndUnder21NationalMinimumWage;
            Between21AndUnder25NationalMinimumWage = between21AndUnder25NationalMinimumWage;
            Over25NationalMinimumWage = over25NationalMinimumWage;
        }
        public DateTime ValidFrom { get;  }
        public DateTime ValidTo { get;  }
        public decimal ApprenticeMinimumWage { get; }
        public decimal Under18NationalMinimumWage { get; }
        public decimal Between18AndUnder21NationalMinimumWage { get; }
        public decimal Between21AndUnder25NationalMinimumWage { get; }
        public decimal Over25NationalMinimumWage { get; }
    }
}
