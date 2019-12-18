using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace SFA.DAS.Provider.Shared.UI.Startup
{
    public static class ContentSecurityPolicyStartup
    {
        public static IApplicationBuilder UseDasContentSecurityPolicy(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                var hostingEnvironment = app.ApplicationServices.GetService<IHostingEnvironment>();
                if (!hostingEnvironment.IsDevelopment())
                {
                    context.Response.Headers["Content-Security-Policy"] =
                        "default-src 'self' 'unsafe-inline' https://*.zdassets.com https://*.zendesk.com wss://*.zendesk.com wss://*.zopim.com; " +
                        "img-src 'self' *.google-analytics.com https://*.zdassets.com https://*.zendesk.com wss://*.zendesk.com wss://*.zopim.com; " +
                        "script-src 'self' 'unsafe-inline' " +
                        "*.googletagmanager.com *.postcodeanywhere.co.uk *.google-analytics.com *.googleapis.com https://*.zdassets.com https://*.zendesk.com wss://*.zendesk.com wss://*.zopim.com; " +
                        "font-src 'self' data:;";
                }

                await next();
            });

            return app;
        }
    }
}