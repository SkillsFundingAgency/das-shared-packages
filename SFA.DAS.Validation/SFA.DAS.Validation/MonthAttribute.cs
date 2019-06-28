using System;
using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.Validation
{
    [AttributeUsage(AttributeTargets.Property)]
    public class MonthAttribute : ValidationAttribute
    {
        protected override System.ComponentModel.DataAnnotations.ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var lowercaseDisplayName = validationContext.DisplayName.ToLower();
            var dayMonthYear = value as DayMonthYear;
            var month = dayMonthYear?.Month;

            if (string.IsNullOrWhiteSpace(month))
            {
                return new System.ComponentModel.DataAnnotations.ValidationResult($"Enter a {lowercaseDisplayName} month", new[] { nameof(dayMonthYear.Month) });
            }

            if (month.Length > 2)
            {
                return new System.ComponentModel.DataAnnotations.ValidationResult($"{validationContext.DisplayName} month: 2 character limit", new[] { nameof(dayMonthYear.Month) });
            }

            return null;
        }
    }
}