using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SFA.DAS.Provider.Shared.UI.Models;
using System;

namespace SFA.DAS.Provider.Shared.UI.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class SetZenDeskValuesAttribute : ResultFilterAttribute
    {
        public ZenDeskConfiguration ZenDeskConfiguration { get; }

        public SetZenDeskValuesAttribute(ZenDeskConfiguration zenDeskConfiguration)
        {
            ZenDeskConfiguration = zenDeskConfiguration;
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (!(context.Controller is Controller controller)) return;
            controller.ViewData[ViewDataKeys.ZenDeskConfiguration] = ZenDeskConfiguration;
        }
    }
}