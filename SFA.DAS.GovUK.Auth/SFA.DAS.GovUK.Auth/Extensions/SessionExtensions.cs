using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace SFA.DAS.GovUK.Auth.Extensions;

public static class SessionExtensions
{
    public static void MapSessionKeepAliveEndpoint(this IEndpointRouteBuilder endpoints, string route = "/session/keepalive")
    {
        endpoints.MapGet(route, async context =>
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                context.Response.StatusCode = StatusCodes.Status204NoContent;
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            }
        });
    }
}