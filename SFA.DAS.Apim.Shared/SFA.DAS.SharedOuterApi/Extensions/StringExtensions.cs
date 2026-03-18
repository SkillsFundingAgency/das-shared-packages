using System;

namespace SFA.DAS.SharedOuterApi.Extensions
{
    public static class StringExtensions
    {
        public static string TrimVacancyReference(this string vacancyReference)
        {
            return string.IsNullOrEmpty(vacancyReference) 
                ? string.Empty 
                : vacancyReference.Replace("VAC", string.Empty, StringComparison.CurrentCultureIgnoreCase);
        }
        
        public static long ConvertVacancyReferenceToLong(this string vacancyReference)
        {
            return Convert.ToInt64(vacancyReference?.TrimVacancyReference() ?? "0");
        }
    }
}