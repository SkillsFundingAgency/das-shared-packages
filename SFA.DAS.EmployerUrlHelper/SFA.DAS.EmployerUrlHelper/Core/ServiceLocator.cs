#if NETCOREAPP
using Microsoft.AspNetCore.Http;

namespace SFA.DAS.EmployerUrlHelper.Core
{
    public static class ServiceLocator
    {
        public static T Get<T>(HttpContext httpContext) where T : class
        {
            return httpContext.RequestServices.GetService(typeof(T)) as T;
        }
    }
}
#endif