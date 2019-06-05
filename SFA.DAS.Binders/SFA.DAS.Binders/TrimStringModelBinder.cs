using System.Web.Mvc;

namespace SFA.DAS.Binders
{
    public sealed class TrimStringModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

            if (valueProviderResult?.AttemptedValue == null)
            {
                return null;
            }

            if (valueProviderResult.AttemptedValue == string.Empty)
            {
                return string.Empty;
            }

            return valueProviderResult.AttemptedValue.Trim();
        }
    }
}
