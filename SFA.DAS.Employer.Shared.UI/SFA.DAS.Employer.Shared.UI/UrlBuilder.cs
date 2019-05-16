using SFA.DAS.Employer.Shared.UI.Configuration;

namespace SFA.DAS.Employer.Shared.UI
{
    public static class UrlBuilder
    {
        public static string GetLink(SiteConfiguration config, string routeName = null)
        {
            return string.IsNullOrWhiteSpace(routeName) ? config.RootUrl : config.RootUrl + config.Routes[routeName];
        }

        public static string GetLink(SiteConfiguration config, string routeName, string accountId)
        {
            return string.Format(config.RootUrl + config.Routes[routeName], accountId);
        }

        public static string GetLink(SiteConfiguration config, string routeName, string clientId, string returnUrl)
        {
            return string.Format(config.RootUrl + config.Routes[routeName], clientId, returnUrl);
        }
    }
}