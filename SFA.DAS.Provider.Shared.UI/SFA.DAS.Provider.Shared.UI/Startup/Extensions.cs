using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Provider.Shared.UI.Attributes;

namespace SFA.DAS.Provider.Shared.UI.Startup
{
    public static class Extensions
    {
        public static IMvcBuilder SetDefaultNavigationSection(this IMvcBuilder builder, NavigationSection defaultSection)
        {
            builder.Services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new SetNavigationSectionAttribute(defaultSection));
            });
            return builder;
        }
    }
}
