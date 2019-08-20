using System;
using System.Globalization;

namespace SFA.DAS.Validation.ModelBinding
{
    public class DayMonthYear
    {
        public virtual string Day { get; set; }
        public virtual string Month { get; set; }
        public virtual string Year { get; set; }

        public static implicit operator DayMonthYear(DateTime dateTime)
        {
            return new DayMonthYear
            {
                Day = dateTime.Day.ToString(),
                Month = dateTime.Month.ToString(),
                Year = dateTime.Year.ToString()
            };
        }

        public static implicit operator DateTime(DayMonthYear dayMonthYear)
        {
            return dayMonthYear.ToDate();
        }

        public bool IsValid()
        {
            return Day != null && Month != null && Year != null && DateTime.TryParseExact($"{Year}-{Month}-{Day}", "yyyy-M-d", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime _);
        }

        public DateTime ToDate()
        {
            return new DateTime(int.Parse(Year), int.Parse(Month), int.Parse(Day), 0, 0, 0);
        }
    }
}