using System;
using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.Validation
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DayAttribute : ValidationAttribute
    {
        protected override System.ComponentModel.DataAnnotations.ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var lowercaseDisplayName = validationContext.DisplayName.ToLower();
            var dayMonthYear = value as DayMonthYear;
            var day = dayMonthYear?.Day;

            if (string.IsNullOrWhiteSpace(day))
            {
                return new System.ComponentModel.DataAnnotations.ValidationResult($"Enter a {lowercaseDisplayName} day", new[] { nameof(dayMonthYear.Day) });
            }

            if (day.Length > 2)
            {
                return new System.ComponentModel.DataAnnotations.ValidationResult($"{validationContext.DisplayName} day: 2 character limit", new[] { nameof(dayMonthYear.Day) });
            }

            return null;
        }
    }
}