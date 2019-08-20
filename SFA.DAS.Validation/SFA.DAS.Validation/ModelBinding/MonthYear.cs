using System;

namespace SFA.DAS.Validation.ModelBinding
{
    public class MonthYear : DayMonthYear
    {
        public override string Day => 1.ToString();

        public static implicit operator MonthYear(DateTime dateTime)
        {
            return new MonthYear
            {
                Month = dateTime.Month.ToString(),
                Year = dateTime.Year.ToString()
            };
        }

        public static implicit operator DateTime(MonthYear monthYear)
        {
            return monthYear.ToDate();
        }
    }
}