using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Employer.Shared.UI.Attributes;

namespace SFA.DAS.Employer.Shared.UI
{
    public static class IMvcBuilderExtensions
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
