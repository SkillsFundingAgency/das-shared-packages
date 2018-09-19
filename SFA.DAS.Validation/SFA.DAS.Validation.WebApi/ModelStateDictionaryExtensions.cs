using System.Web.Http.ModelBinding;

namespace SFA.DAS.Validation.WebApi
{
    public static class ModelStateDictionaryExtensions
    {
        public static void AddModelError(this ModelStateDictionary modelState, ValidationException ex)
        {
            var key = ex.Expression == null ? "" : $"{ex.Expression.Type.GenericTypeArguments[0].Name}.{ExpressionHelper.GetExpressionText(ex.Expression)}";

            modelState.AddModelError(key, ex.Message);
        }
    }
}