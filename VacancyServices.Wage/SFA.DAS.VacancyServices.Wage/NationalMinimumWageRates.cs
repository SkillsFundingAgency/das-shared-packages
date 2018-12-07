using System;

namespace SFA.DAS.VacancyServices.Wage
{
    public struct NationalMinimumWageRates
    {
        internal NationalMinimumWageRates(DateTime validFrom, DateTime validTo, decimal apprenticeMinimumWage, decimal under18NationalMinimumWage, decimal between18And20NationalMinimumWage, decimal between21And24NationalMinimumWage, decimal over24NationalMinimumWage)
        {
            ValidFrom = validFrom;
            ValidTo = validTo;
            ApprenticeMinimumWage = apprenticeMinimumWage;
            Under18NationalMinimumWage = under18NationalMinimumWage;
            Between18And20NationalMinimumWage = between18And20NationalMinimumWage;
            Between21And24NationalMinimumWage = between21And24NationalMinimumWage;
            Over24NationalMinimumWage = over24NationalMinimumWage;
        }
        public DateTime ValidFrom { get;  }
        public DateTime ValidTo { get;  }
        public decimal ApprenticeMinimumWage { get; }
        public decimal Under18NationalMinimumWage { get; }
        public decimal Between18And20NationalMinimumWage { get; }
        public decimal Between21And24NationalMinimumWage { get; }
        public decimal Over24NationalMinimumWage { get; }
    }
}
