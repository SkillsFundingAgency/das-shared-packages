using System;
using System.ComponentModel.DataAnnotations;
using SFA.DAS.Validation.ModelBinding;

namespace SFA.DAS.Validation.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DayAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var lowercaseDisplayName = validationContext.DisplayName.ToLower();
            var dayMonthYear = value as DayMonthYear;
            var day = dayMonthYear?.Day;

            if (string.IsNullOrWhiteSpace(day))
            {
                return new ValidationResult($"Enter a {lowercaseDisplayName} day", new[] { nameof(dayMonthYear.Day) });
            }

            if (day.Length > 2)
            {
                return new ValidationResult($"{validationContext.DisplayName} day: 2 character limit", new[] { nameof(dayMonthYear.Day) });
            }

            return null;
        }
    }
}