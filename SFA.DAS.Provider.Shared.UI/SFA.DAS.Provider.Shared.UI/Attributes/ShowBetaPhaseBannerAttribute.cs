using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SFA.DAS.Provider.Shared.UI.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ShowBetaPhaseBannerAttribute : ResultFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (!(context.Controller is Controller controller)) return;
            controller.ViewData[ViewDataKeys.ShowBetaPhaseBanner] = true;
        }
    }
}
