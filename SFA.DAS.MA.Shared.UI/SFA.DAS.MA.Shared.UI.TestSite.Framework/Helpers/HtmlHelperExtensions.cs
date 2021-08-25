using System;
using System.Web;
using System.Web.Mvc;
using SFA.DAS.MA.Shared.UI.Configuration;
using SFA.DAS.MA.Shared.UI.Models;
using SFA.DAS.MA.Shared.UI.TestSite.Framework.App_Start;

namespace SFA.DAS.MA.Shared.UI.TestSite.Framework.Helpers
{
    public static class HtmlHelperExtensions
    {
        public static IHeaderViewModel GetHeaderViewModel(this HtmlHelper html, bool useLegacyStyles = false)
        {
            var configuration = DependencyResolver.Current.GetService<IConfiguration>();
            var oidcConfiguration = DependencyResolver.Current.GetService<IOidcConfiguration>();
            var requestRoot = GetRootUrl(html.ViewContext.HttpContext.Request);

            var headerModel = new HeaderViewModel(new HeaderConfiguration
            {
                ManageApprenticeshipsBaseUrl = configuration.EmployerAccountsBaseUrl,
                ApplicationBaseUrl = configuration.EmployerAccountsBaseUrl,
                EmployerCommitmentsBaseUrl = configuration.EmployerCommitmentsBaseUrl,
                EmployerFinanceBaseUrl = configuration.EmployerFinanceBaseUrl,
                AuthenticationAuthorityUrl = oidcConfiguration.BaseAddress,
                ClientId = oidcConfiguration.ClientId,
                EmployerRecruitBaseUrl = configuration.EmployerRecruitBaseUrl,
                SignOutUrl = new Uri($"{requestRoot}/signOut"),
                ChangeEmailReturnUrl = new System.Uri(configuration.EmployerPortalBaseUrl + "/service/email/change"),
                ChangePasswordReturnUrl = new System.Uri(configuration.EmployerPortalBaseUrl + "/service/password/change")
            },
            new UserContext
            {
                User = html.ViewContext.HttpContext.User,
                HashedAccountId = html.ViewContext.RouteData.Values["accountHashedId"]?.ToString()
            }, 
            useLegacyStyles: useLegacyStyles);

            headerModel.SelectMenu("home");

            return headerModel;
        }

        public static IFooterViewModel GetFooterViewModel(this HtmlHelper html, bool useLegacyStyles = false)
        {
            var configuration = DependencyResolver.Current.GetService<IConfiguration>();
            var oidcConfiguration = DependencyResolver.Current.GetService<IOidcConfiguration>();

            return new FooterViewModel(new FooterConfiguration
            {
                ManageApprenticeshipsBaseUrl = configuration.EmployerAccountsBaseUrl,
                AuthenticationAuthorityUrl = oidcConfiguration.BaseAddress,
            },
            new UserContext
            {
                User = html.ViewContext.HttpContext.User,
                HashedAccountId = html.ViewContext.RouteData.Values["accountHashedId"]?.ToString()
            },
            useLegacyStyles: useLegacyStyles
            );
        }

        public static ICookieBannerViewModel GetCookieBannerViewModel(this HtmlHelper html)
        {
            var configuration = DependencyResolver.Current.GetService<IConfiguration>();

            return new CookieBannerViewModel(new CookieBannerConfiguration
            {
                ManageApprenticeshipsBaseUrl = configuration.EmployerAccountsBaseUrl
            },
            new UserContext
            {
                User = html.ViewContext.HttpContext.User,
                HashedAccountId = html.ViewContext.RouteData.Values["accountHashedId"]?.ToString()
            }
            );
        }

        private static string GetRootUrl(HttpRequestBase request)
        {
            var requestUrl = new Uri(request.Url.AbsoluteUri);

            return $"{requestUrl.Scheme}://{requestUrl.Authority}";
        }

        public static MvcHtmlString CdnLink(this HtmlHelper html, string folderName, string fileName)
        {
            var cdnLocation = StructuremapMvc.StructureMapDependencyScope.Container.GetInstance<IConfiguration>().CdnBaseUrl;

            var trimCharacters = new char[] { '/' };
            return new MvcHtmlString($"{cdnLocation.Trim(trimCharacters)}/{folderName.Trim(trimCharacters)}/{fileName.Trim(trimCharacters)}");
        }
    }
}