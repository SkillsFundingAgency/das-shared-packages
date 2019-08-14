using System;

namespace SFA.DAS.Validation.ModelBinding
{
    public class Year : DayMonthYear
    {
        public override string Day => 1.ToString();
        public override string Month => 1.ToString();

        public static implicit operator Year(DateTime dateTime)
        {
            return new Year
            {
                Year = dateTime.Year.ToString()
            };
        }

        public static implicit operator DateTime(Year year)
        {
            return year.ToDate();
        }
    }
}