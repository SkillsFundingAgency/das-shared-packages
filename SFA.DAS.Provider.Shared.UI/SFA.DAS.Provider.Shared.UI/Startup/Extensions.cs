using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Provider.Shared.UI.Attributes;
using SFA.DAS.Provider.Shared.UI.Models;

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

        public static IMvcBuilder SuppressNavigationSection(this IMvcBuilder builder, params NavigationSection[] sections)
        {
            builder.Services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new SuppressNavigationSectionAttribute(sections));
            });
            return builder;
        }

        public static IMvcBuilder EnableGoogleAnalytics(this IMvcBuilder builder)
        {
            builder.Services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new EnableGoogleAnalyticsAttribute());
            });
            return builder;
        }

        public static IMvcBuilder EnableCookieBanner(this IMvcBuilder builder)
        {
            builder.Services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new EnableCookieBannerAttribute());
            });
            return builder;
        }

        public static IMvcBuilder SetZenDeskConfiguration(this IMvcBuilder builder, ZenDeskConfiguration zenDeskConfiguration)
        {
            builder.Services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new SetZenDeskValuesAttribute(zenDeskConfiguration));
            });
            return builder;
        }

        public static IMvcBuilder SetDfESignInConfiguration(this IMvcBuilder builder, bool useDfESignIn)
        {
            builder.Services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new SetDfESignStatusValuesAttribute(useDfESignIn));
            });
            return builder;
        }
    }
}