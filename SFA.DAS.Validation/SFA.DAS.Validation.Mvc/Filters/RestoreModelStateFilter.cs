using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Validation.Mvc.Extensions;
using SFA.DAS.Validation.Mvc.ModelBinding;

namespace SFA.DAS.Validation.Mvc.Filters
{
    public class RestoreModelStateFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var tempDataFactory = filterContext.HttpContext.RequestServices.GetRequiredService<ITempDataDictionaryFactory>();
            var tempData = tempDataFactory.GetTempData(filterContext.HttpContext);
            var serializableModelState = tempData.Get<SerializableModelStateDictionary>();
            var modelState = serializableModelState?.ToModelState();

            filterContext.ModelState.Merge(modelState);
        }
    }
}