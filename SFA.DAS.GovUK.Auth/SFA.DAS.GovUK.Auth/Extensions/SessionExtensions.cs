using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace SFA.DAS.GovUK.Auth.Extensions;

public static class SessionExtensions
{
    public static void MapSessionKeepAliveEndpoint(this IEndpointRouteBuilder endpoints, string route = "/service/keepalive")
    {
        endpoints.MapGet(route, async context =>
        {
            context.Response.StatusCode = context.User.Identity?.IsAuthenticated == true 
                ? StatusCodes.Status204NoContent 
                : StatusCodes.Status401Unauthorized;
        }).RequireAuthorization();
    }
}