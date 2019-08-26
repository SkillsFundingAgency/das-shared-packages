#if NETCOREAPP2_0
using Microsoft.AspNetCore.Builder;
using SFA.DAS.UnitOfWork.Mvc.Middleware;

namespace SFA.DAS.UnitOfWork.Mvc.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseUnitOfWork(this IApplicationBuilder app)
        {
            return app.UseMiddleware<UnitOfWorkManagerMiddleware>();
        }
    }
}
#endif