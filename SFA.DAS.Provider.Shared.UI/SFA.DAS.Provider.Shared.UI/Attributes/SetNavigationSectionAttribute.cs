using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SFA.DAS.Provider.Shared.UI.Attributes
{
    public class SetNavigationSectionAttribute : ResultFilterAttribute
    {
        public NavigationSection Section { get; set; }

        public SetNavigationSectionAttribute(NavigationSection section)
        {
            Section = section;
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (!(context.Controller is Controller controller)) return;
            controller.ViewData[ViewDataKeys.SelectedNavigationSection] = Section;
        }
    }
}
