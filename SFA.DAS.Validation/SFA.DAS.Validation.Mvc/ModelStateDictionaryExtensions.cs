﻿#if NET462
using System.Linq;
using System.Web.Mvc;

namespace SFA.DAS.Validation.Mvc
{
    public static class ModelStateDictionaryExtensions
    {
        public static void AddModelError(this ModelStateDictionary modelState, object model, ValidationException ex)
        {
            if (!string.IsNullOrWhiteSpace(ex.Message))
            {
                modelState.AddModelError("", ex.Message);
            }

            foreach (var validationError in ex.ValidationErrors)
            {
                var parentKey = model == validationError.Model ? "" : model.GetType().GetProperties().Where(p => p.GetValue(model) == validationError.Model).Select(p => p.Name).Single();
                var childKey = ExpressionHelper.GetExpressionText(validationError.Property);
                var key = $"{parentKey}.{childKey}".Trim('.');

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