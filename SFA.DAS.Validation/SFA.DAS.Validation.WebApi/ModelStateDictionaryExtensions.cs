using System.Linq;
using System.Web.Mvc;
using ModelStateDictionary = System.Web.Http.ModelBinding.ModelStateDictionary;

namespace SFA.DAS.Validation.WebApi
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
    }
}