using Microsoft.AspNetCore.Mvc.Rendering;
using SFA.DAS.MA.Shared.UI.Configuration;
using SFA.DAS.MA.Shared.UI.Models;

namespace SFA.DAS.GovUk.SampleSite.Legacy.Extensions
{

    public static class HtmlHelperExtensions
    {
        public static IHeaderViewModel GetHeaderViewModel(this IHtmlHelper html, bool hideMenu = false)
        {

            var headerModel = new HeaderViewModel(new HeaderConfiguration
                {
                    EmployerCommitmentsBaseUrl = "#",
                    EmployerFinanceBaseUrl = "#",
                    ManageApprenticeshipsBaseUrl = "#",
                    AuthenticationAuthorityUrl = "#",
                    ClientId = "#",
                    EmployerRecruitBaseUrl = "#",
                    SignOutUrl = null,
                    ChangeEmailReturnUrl = null,
                    ChangePasswordReturnUrl = null
                },
                new UserContext
                {
                    User = html.ViewContext.HttpContext.User,
                    HashedAccountId = html.ViewContext.RouteData.Values["employerAccountId"]?.ToString()
                });

            headerModel.SelectMenu("recruitment");

            if (html.ViewBag.HideNav is bool && html.ViewBag.HideNav)
            {
                headerModel.HideMenu();
            }

            return headerModel;
        }

        public static IFooterViewModel GetFooterViewModel(this IHtmlHelper html)
        {
            return new FooterViewModel(new FooterConfiguration
                {
                    ManageApprenticeshipsBaseUrl = "#"
                },
                new UserContext
                {
                    User = html.ViewContext.HttpContext.User,
                    HashedAccountId = html.ViewContext.RouteData.Values["employerAccountId"]?.ToString()
                });
        }

        public static ICookieBannerViewModel GetCookieBannerViewModel(this IHtmlHelper html)
        {
            return new CookieBannerViewModel(new CookieBannerConfiguration
                {
                    ManageApprenticeshipsBaseUrl = "#"
                },
                new UserContext
                {
                    User = html.ViewContext.HttpContext.User,
                    HashedAccountId = html.ViewContext.RouteData.Values["employerAccountId"]?.ToString()
                });
        }
    }
}