using System;
using System.ComponentModel.DataAnnotations;
using SFA.DAS.Validation.ModelBinding;

namespace SFA.DAS.Validation.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class YearAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var lowercaseDisplayName = validationContext.DisplayName.ToLower();
            var dayMonthYear = value as DayMonthYear;
            var year = dayMonthYear?.Year;

            if (string.IsNullOrWhiteSpace(year))
            {
                return new ValidationResult($"Enter a {lowercaseDisplayName} year", new[] { nameof(dayMonthYear.Year) });
            }

            if (year.Length > 4)
            {
                return new ValidationResult($"{validationContext.DisplayName} year: 4 character limit", new[] { nameof(dayMonthYear.Year) });
            }

            return null;
        }
    }
}