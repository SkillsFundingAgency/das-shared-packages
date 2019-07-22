#if NETCOREAPP
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;

namespace SFA.DAS.EmployerUrlHelper.Core
{
    public static class EmployerUrlHelperExtensions
    {
        public static string AccountsLink(this UrlHelper urlHelper, string path)
        {
            return GetLinkGenerator(urlHelper.ActionContext.HttpContext).AccountsLink(path);
        }
        
        public static string CommitmentsLink(this UrlHelper urlHelper, string path)
        {
            return GetLinkGenerator(urlHelper.ActionContext.HttpContext).CommitmentsLink(path);
        }
        
        public static string CommitmentsV2Link(this UrlHelper urlHelper, string path)
        {
            return GetLinkGenerator(urlHelper.ActionContext.HttpContext).CommitmentsV2Link(path);
        }
        
        public static string PortalLink(this UrlHelper urlHelper, string path)
        {
            return GetLinkGenerator(urlHelper.ActionContext.HttpContext).PortalLink(path);
        }
        
        public static string ProjectionsLink(this UrlHelper urlHelper, string path)
        {
            return GetLinkGenerator(urlHelper.ActionContext.HttpContext).ProjectionsLink(path);
        }
        
        public static string RecruitLink(this UrlHelper urlHelper, string path)
        {
            return GetLinkGenerator(urlHelper.ActionContext.HttpContext).RecruitLink(path);
        }
        
        public static string UsersLink(this UrlHelper urlHelper, string path)
        {
            return GetLinkGenerator(urlHelper.ActionContext.HttpContext).UsersLink(path);
        }
        
        private static ILinkGenerator GetLinkGenerator(HttpContext httpContext)
        {
            return ServiceLocator.Get<ILinkGenerator>(httpContext);
        }
    }
}
#endif