using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SFA.DAS.Provider.Shared.UI.Attributes
{
    public class HideNavigationBarAttribute : ResultFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (!(context.Controller is Controller controller)) return;
            controller.ViewData[ViewDataKeys.HideNavigationBar] = true;
        }
    }
}
