using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace SFA.DAS.Api.Common.Middleware;

public class SecurityHeadersMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        context.Response.Headers.TryAdd("x-frame-options", new StringValues("DENY"));
        context.Response.Headers.TryAdd("x-content-type-options", new StringValues("nosniff"));
        context.Response.Headers.TryAdd("X-Permitted-Cross-Domain-Policies", new StringValues("none"));
        context.Response.Headers.TryAdd("Content-Security-Policy", new StringValues("default-src *; script-src *; connect-src *; img-src *; style-src *; object-src *;"));
        
        await next(context);
    }
}