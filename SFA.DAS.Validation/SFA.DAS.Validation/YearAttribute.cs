using System;
using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.Validation
{
    [AttributeUsage(AttributeTargets.Property)]
    public class YearAttribute : ValidationAttribute
    {
        protected override System.ComponentModel.DataAnnotations.ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var lowercaseDisplayName = validationContext.DisplayName.ToLower();
            var dayMonthYear = value as DayMonthYear;
            var year = dayMonthYear?.Year;

            if (string.IsNullOrWhiteSpace(year))
            {
                return new System.ComponentModel.DataAnnotations.ValidationResult($"Enter a {lowercaseDisplayName} year", new[] { nameof(dayMonthYear.Year) });
            }

            if (year.Length > 4)
            {
                return new System.ComponentModel.DataAnnotations.ValidationResult($"{validationContext.DisplayName} year: 4 character limit", new[] { nameof(dayMonthYear.Year) });
            }

            return null;
        }
    }
}