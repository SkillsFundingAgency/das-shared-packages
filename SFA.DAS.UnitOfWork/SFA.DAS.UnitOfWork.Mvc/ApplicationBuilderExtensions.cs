#if NETCOREAPP2_0
using Microsoft.AspNetCore.Builder;

namespace SFA.DAS.UnitOfWork.Mvc
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