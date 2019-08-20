using System;
using System.ComponentModel.DataAnnotations;
using SFA.DAS.Validation.ModelBinding;

namespace SFA.DAS.Validation.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var beginningOfTwentiethCentury = new DateTime(1900, 1, 1);
            var lowercaseDisplayName = validationContext.DisplayName.ToLower();

            if (value is DayMonthYear dayMonthYear)
            {
                if (dayMonthYear.Day?.Length <= 2 && dayMonthYear.Month?.Length <= 2 && dayMonthYear.Year?.Length <= 4)
                {
                    if (!dayMonthYear.IsValid())
                    {
                        return new ValidationResult($"Enter a different {lowercaseDisplayName}", new[] { nameof(dayMonthYear.Year) });
                    }

                    if (dayMonthYear < beginningOfTwentiethCentury)
                    {
                        return new ValidationResult($"The earliest {lowercaseDisplayName} you can enter is {beginningOfTwentiethCentury:MM yyyy}", new[] { nameof(dayMonthYear.Year) });
                    }
                }
            }

            return null;
        }
    }
}