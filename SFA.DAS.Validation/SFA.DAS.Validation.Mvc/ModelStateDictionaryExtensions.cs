﻿#if NET462
using System.Linq;
using System.Web.Mvc;

namespace SFA.DAS.Validation.Mvc
{
    public static class ModelStateDictionaryExtensions
    {
        public static void AddModelError<T>(this ModelStateDictionary modelState, T model, ValidationException ex) where T : class
        {
            if (!string.IsNullOrWhiteSpace(ex.Message))
            {
                modelState.AddModelError("", ex.Message);
            }

            foreach (var validationError in ex.ValidationErrors)
            {
                var key = ExpressionHelper.GetExpressionText(validationError.Property);

                modelState.AddModelError(key, validationError.Message);
            }
        }

        public static SerializableModelStateDictionary ToSerializable(this ModelStateDictionary modelState)
        {
            var data = modelState
                .Select(kvp => new SerializableModelState
                {
                    AttemptedValue = kvp.Value.Value?.AttemptedValue,
                    CultureName = kvp.Value.Value?.Culture.Name,
                    ErrorMessages = kvp.Value.Errors.Select(e => e.ErrorMessage).ToList(),
                    Key = kvp.Key,
                    RawValue = kvp.Value.Value?.RawValue
                })
                .ToList();

            return new SerializableModelStateDictionary
            {
                Data = data
            };
        }
    }
}
#endif