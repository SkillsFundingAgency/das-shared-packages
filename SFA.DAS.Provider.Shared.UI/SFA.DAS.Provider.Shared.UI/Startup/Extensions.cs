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

        public static IMvcBuilder ShowBetaPhaseBanner(this IMvcBuilder builder)
        {
            builder.Services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new ShowBetaPhaseBannerAttribute());
            });
            return builder;
        }

        public static IMvcBuilder SuppressNavigationSection(this IMvcBuilder builder, NavigationSection section)
        {
            builder.Services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new SuppressNavigationSectionAttribute(section));
            });
            return builder;
        }
    }
}
