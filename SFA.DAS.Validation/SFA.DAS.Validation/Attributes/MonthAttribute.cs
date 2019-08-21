using System;
using System.ComponentModel.DataAnnotations;
using SFA.DAS.Validation.ModelBinding;

namespace SFA.DAS.Validation.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class MonthAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var lowercaseDisplayName = validationContext.DisplayName.ToLower();
            var dayMonthYear = value as DayMonthYear;
            var month = dayMonthYear?.Month;

            if (string.IsNullOrWhiteSpace(month))
            {
                return new ValidationResult($"Enter a {lowercaseDisplayName} month", new[] { nameof(dayMonthYear.Month) });
            }

            if (month.Length > 2)
            {
                return new ValidationResult($"{validationContext.DisplayName} month: 2 character limit", new[] { nameof(dayMonthYear.Month) });
            }

            return null;
        }
    }
}