using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SFA.DAS.Provider.Shared.UI.Extensions;

namespace SFA.DAS.Provider.Shared.UI.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class SuppressNavigationSectionAttribute : ResultFilterAttribute
    {
        public NavigationSection Section { get; }

        public SuppressNavigationSectionAttribute(NavigationSection section)
        {
            Section = section;
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (!(context.Controller is Controller controller)) return;

            var suppressedNavigationSections = GetSuppressedNavigationSections(controller);

            if (!suppressedNavigationSections.Contains(Section))
            {
                suppressedNavigationSections.Add(Section);
                controller.ViewData[ViewDataKeys.SuppressNavigationSection] = suppressedNavigationSections;
            }
        }

        private List<NavigationSection> GetSuppressedNavigationSections(Controller controller)
        {
            if (controller.ViewData[ViewDataKeys.SuppressNavigationSection] is List<NavigationSection> existing)
            {
                return existing;
            }

            return new List<NavigationSection>();
        }
    }
}