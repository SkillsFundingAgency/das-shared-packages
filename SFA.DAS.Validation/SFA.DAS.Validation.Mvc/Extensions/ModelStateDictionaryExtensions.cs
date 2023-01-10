using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using SFA.DAS.Validation.Exceptions;
using SFA.DAS.Validation.Mvc.ModelBinding;

namespace SFA.DAS.Validation.Mvc.Extensions
{
    public static class ModelStateDictionaryExtensions
    {
        public static void AddModelError(this ModelStateDictionary modelState, ValidationException ex)
        {
            if (string.IsNullOrWhiteSpace(ex.Message) && !ex.ValidationErrors.Any())
            {
                modelState.AddModelError("", "");
            }
            else
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
        }

        public static SerializableModelStateDictionary ToSerializable(this ModelStateDictionary modelState)
        {
            var data = modelState
                .Select(kvp => new SerializableModelState
                {
                    AttemptedValue = kvp.Value.AttemptedValue,
                    ErrorMessages = kvp.Value.Errors.Select(e => e.ErrorMessage).ToList(),
                    Key = kvp.Key,
                    RawValue = kvp.Value.RawValue
                })
                .ToList();

            return new SerializableModelStateDictionary
            {
                Data = data
            };
        }
    }
}
