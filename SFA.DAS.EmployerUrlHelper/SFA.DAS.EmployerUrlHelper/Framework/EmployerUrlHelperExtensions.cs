using System;
#if NETFRAMEWORK
using UrlHelper =System.Web.Mvc.UrlHelper;

namespace SFA.DAS.EmployerUrlHelper.Framework
{
    public static class EmployerUrlHelperExtensions
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
        
        private static ILinkGenerator GetLinkGenerator()
        {
            return ServiceLocator.Get<ILinkGenerator>();
        }
    }
}
#endif