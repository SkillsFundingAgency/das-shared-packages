using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Validation.Mvc.Extensions;

namespace SFA.DAS.Validation.Mvc.Filters
{
    public class PreserveModelStateFilter : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (!(filterContext.Result is RedirectToActionResult)) return;
            var tempDataFactory = filterContext.HttpContext.RequestServices.GetRequiredService<ITempDataDictionaryFactory>();
            var tempData = tempDataFactory.GetTempData(filterContext.HttpContext);
            var serializableModelState = filterContext.ModelState.ToSerializable();
            tempData.Set(serializableModelState);
        }
    }
}