using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using SFA.DAS.Validation.Exceptions;
using SFA.DAS.Validation.Mvc.ModelBinding;

namespace SFA.DAS.Validation.Mvc.Extensions;

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
            .Select(ToSerializableModelState)
            .ToList();

        return new SerializableModelStateDictionary {
            Data = data
        };
    }

    /// <summary>
    /// Returns a SerializableModelStateDictionary containing only modelState entries which have an error value.
    /// </summary>
    public static SerializableModelStateDictionary ToSerializableWithOnlyErrors(this ModelStateDictionary modelState)
    {
        var data = modelState
            .Where(x => x.Value.Errors != null && x.Value.Errors.Any(error=>!string.IsNullOrEmpty(error.ErrorMessage)))
            .Select(ToSerializableModelState)
            .ToList();

        return new SerializableModelStateDictionary {
            Data = data
        };
    }

    private static SerializableModelState ToSerializableModelState(KeyValuePair<string, ModelStateEntry> keyValuePair)
    {
        return new SerializableModelState {
            AttemptedValue = keyValuePair.Value.AttemptedValue,
            ErrorMessages = keyValuePair.Value.Errors.Select(e => e.ErrorMessage).ToList(),
            Key = keyValuePair.Key,
            RawValue = keyValuePair.Value.RawValue
        };
    }
}