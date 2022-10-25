using Microsoft.AspNetCore.Builder;

namespace SFA.DAS.Provider.Shared.UI.Startup
{
    public static class ContentSecurityPolicyStartup
    {
        public static IApplicationBuilder UseDasContentSecurityPolicy(this IApplicationBuilder app, bool isDevelopment)
        {
            app.Use(async (context, next) =>
            {
                if (!isDevelopment)
                {
                    context.Response.Headers["Content-Security-Policy"] =
                        "default-src 'self' 'unsafe-inline' https://*.zdassets.com https://*.zendesk.com wss://*.zendesk.com wss://*.zopim.com; " +
                        "style-src 'self' 'unsafe-inline' https://*.azureedge.net; " +
                        "img-src 'self' https://*.azureedge.net *.google-analytics.com https://*.zdassets.com https://*.zendesk.com wss://*.zendesk.com wss://*.zopim.com; " +
                        "script-src 'self' 'unsafe-inline' " +
                        "*.googletagmanager.com *.postcodeanywhere.co.uk *.google-analytics.com *.googleapis.com https://*.zdassets.com https://*.zendesk.com wss://*.zendesk.com wss://*.zopim.com; " +
                        "font-src 'self' data:;";
                    await next();    
                }
                
            });

            return app;
        }
    }
}