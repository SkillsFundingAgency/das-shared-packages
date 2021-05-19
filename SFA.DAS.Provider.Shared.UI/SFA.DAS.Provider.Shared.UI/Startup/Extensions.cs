using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SFA.DAS.Provider.Shared.UI.Attributes;
using SFA.DAS.Provider.Shared.UI.Extensions;
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
    }

    public static class AddServiceExtensions
    {
        public static void AddProviderUiServiceRegistration(this IServiceCollection services, IConfiguration configuration)
        {
            if(configuration.GetSection(nameof(ProviderSharedUIConfiguration)) == null)
            {
                throw new Exception("Cannot find ProviderSharedUIConfiguration in configuration");
            }
            
            services.Configure<ProviderSharedUIConfiguration>(configuration.GetSection(nameof(ProviderSharedUIConfiguration)));
            services.AddSingleton(cfg => cfg.GetService<IOptions<ProviderSharedUIConfiguration>>().Value);
            services.AddSingleton<IExternalUrlHelper, ExternalUrlHelper>();
        }
    }
}