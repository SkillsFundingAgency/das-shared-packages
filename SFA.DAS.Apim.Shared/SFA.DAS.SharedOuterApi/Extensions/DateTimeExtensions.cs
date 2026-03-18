using System;

namespace SFA.DAS.SharedOuterApi.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime StartOfAprilOfFinancialYear(this DateTime dateTime)
        {
            return dateTime.Month >= 4 ? new DateTime(dateTime.Year, 4, 1) : new DateTime(dateTime.AddYears(-1).Year, 4, 1);
        }

        public static string ToDayMonthYearString(this DateTime? date)
        {
            return date?.ToString("d MMMM yyyy") ?? string.Empty;
        }
    }
}
