using System.Web.Mvc;

namespace SFA.DAS.Authorization.Mvc
{
    public static class HtmlHelperExtensions
    {
        public static AuthorizationResult GetAuthorizationResult(this HtmlHelper htmlHelper, FeatureType featureType)
        {
            var authorisationService = DependencyResolver.Current.GetService<IAuthorizationService>();
            var authorizationResult = authorisationService.GetAuthorizationResult(featureType);

            return authorizationResult;
        }

        public static bool IsAuthorized(this HtmlHelper htmlHelper, FeatureType featureType)
        {
            var authorisationService = DependencyResolver.Current.GetService<IAuthorizationService>();
            var isAuthorized = authorisationService.IsAuthorized(featureType);

            return isAuthorized;
        }
    }
}