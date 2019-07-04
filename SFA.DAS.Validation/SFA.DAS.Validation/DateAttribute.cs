﻿using System;
using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.Validation
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DateAttribute : ValidationAttribute
    {
        protected override System.ComponentModel.DataAnnotations.ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var now = DateTime.Now;
            var today = now.Date;
            var beginningOfTwentiethCentury = new DateTime(1900, 1, 1);
            var lowercaseDisplayName = validationContext.DisplayName.ToLower();

            if (value is DayMonthYear dayMonthYear)
            {
                if (dayMonthYear.Day?.Length <= 2 && dayMonthYear.Month?.Length <= 2 && dayMonthYear.Year?.Length <= 4)
                {
                    if (!dayMonthYear.IsValid())
                    {
                        return new System.ComponentModel.DataAnnotations.ValidationResult($"Enter a different {lowercaseDisplayName}", new[] { nameof(dayMonthYear.Year) });
                    }

                    if (dayMonthYear > today)
                    {
                        return new System.ComponentModel.DataAnnotations.ValidationResult($"The latest {lowercaseDisplayName} you can enter is {today:MM yyyy}", new[] { nameof(dayMonthYear.Year) });
                    }

                    if (dayMonthYear < beginningOfTwentiethCentury)
                    {
                        return new System.ComponentModel.DataAnnotations.ValidationResult($"The earliest {lowercaseDisplayName} you can enter is {beginningOfTwentiethCentury:MM yyyy}", new[] { nameof(dayMonthYear.Year) });
                    }
                }
            }

            return null;
        }
    }
}