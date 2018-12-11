using System;

namespace SFA.DAS.VacancyServices.Wage
{
    public class WageDetails
    {
        public decimal? Amount { get; set; }

        public decimal? LowerBound { get; set; }

        public decimal? UpperBound { get; set; }

        public string Text { get; set; }

        public decimal? HoursPerWeek { get; set; }

        public DateTime StartDate { get; set; }
    }
}
