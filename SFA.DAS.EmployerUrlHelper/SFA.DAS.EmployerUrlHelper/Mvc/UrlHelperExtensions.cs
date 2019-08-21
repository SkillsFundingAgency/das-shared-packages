#if NETSTANDARD2_0
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace SFA.DAS.EmployerUrlHelper.Mvc
{
    public static class UrlHelperExtensions
    {
        public static string AccountsLink(this UrlHelper urlHelper, string path)
        {
            return GetLinkGenerator(urlHelper).AccountsLink(path);
        }
        
        public static string CommitmentsLink(this UrlHelper urlHelper, string path)
        {
            return GetLinkGenerator(urlHelper).CommitmentsLink(path);
        }
        
        public static string CommitmentsV2Link(this UrlHelper urlHelper, string path)
        {
            return GetLinkGenerator(urlHelper).CommitmentsV2Link(path);
        }
        
        public static string FinanceLink(this UrlHelper urlHelper, string path)
        {
            return GetLinkGenerator(urlHelper).FinanceLink(path);
        }
        
        public static string PortalLink(this UrlHelper urlHelper, string path)
        {
            return GetLinkGenerator(urlHelper).PortalLink(path);
        }
        
        public static string ProjectionsLink(this UrlHelper urlHelper, string path)
        {
            return GetLinkGenerator(urlHelper).ProjectionsLink(path);
        }
        
        public static string RecruitLink(this UrlHelper urlHelper, string path)
        {
            return GetLinkGenerator(urlHelper).RecruitLink(path);
        }
        
        public static string ReservationsLink(this UrlHelper urlHelper, string path)
        {
            return GetLinkGenerator(urlHelper).ReservationsLink(path);
        }
        
        public static string UsersLink(this UrlHelper urlHelper, string path)
        {
            return GetLinkGenerator(urlHelper).UsersLink(path);
        }
        
        private static ILinkGenerator GetLinkGenerator(UrlHelper urlHelper)
        {
            return urlHelper.ActionContext.HttpContext.RequestServices.GetRequiredService<ILinkGenerator>();
        }
    }
}
#elif NETFRAMEWORK
using System.Web.Mvc;

namespace SFA.DAS.EmployerUrlHelper.Mvc
{
    public static class UrlHelperExtensions
    {
        public static string AccountsLink(this UrlHelper urlHelper, string path)
        {
            return GetLinkGenerator().AccountsLink(path);
        }
        
        public static string CommitmentsLink(this UrlHelper urlHelper, string path)
        {
            return GetLinkGenerator().CommitmentsLink(path);
        }
        
        public static string CommitmentsV2Link(this UrlHelper urlHelper, string path)
        {
            return GetLinkGenerator().CommitmentsV2Link(path);
        }
        
        public static string FinanceLink(this UrlHelper urlHelper, string path)
        {
            return GetLinkGenerator().FinanceLink(path);
        }
        
        public static string PortalLink(this UrlHelper urlHelper, string path)
        {
            return GetLinkGenerator().PortalLink(path);
        }
        
        public static string ProjectionsLink(this UrlHelper urlHelper, string path)
        {
            return GetLinkGenerator().ProjectionsLink(path);
        }
        
        public static string RecruitLink(this UrlHelper urlHelper, string path)
        {
            return GetLinkGenerator().RecruitLink(path);
        }
        
        public static string UsersLink(this UrlHelper urlHelper, string path)
        {
            return GetLinkGenerator().UsersLink(path);
        }
        
        public static string ReservationsLink(this UrlHelper urlHelper, string path)
        {
            return GetLinkGenerator().ReservationsLink(path);
        }
        
        private static ILinkGenerator GetLinkGenerator()
        {
            return DependencyResolver.Current.GetService<ILinkGenerator>();
        }
    }
}
#endif