using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SFA.DAS.Provider.Shared.UI.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class SuppressNavigationSectionAttribute : ResultFilterAttribute
    {
        public NavigationSection[] Sections { get; }

        public SuppressNavigationSectionAttribute(NavigationSection[] sections)
        {
            Sections = sections;
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (!(context.Controller is Controller controller)) return;
            controller.ViewData[ViewDataKeys.SuppressNavigationSection] = Sections;
        }
    }
}