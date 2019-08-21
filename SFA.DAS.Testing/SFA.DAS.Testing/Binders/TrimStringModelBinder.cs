using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace SFA.DAS.Testing.Binders
{
    public class TrimStringModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

            if (valueProviderResult.FirstValue == null)
            {
                return null;
            }

            if (valueProviderResult.FirstValue == string.Empty)
            {
                return string.Empty;
            }

            return valueProviderResult.FirstValue.Trim();
        }
    }
}
