using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SFA.DAS.Employer.Shared.UI.Configuration;
using SFA.DAS.EmployerUrlHelper;

namespace SFA.DAS.Employer.Shared.UI
{
    public class UrlBuilder
    {
        private readonly ILogger<UrlBuilder> _logger;
        private readonly ILinkGenerator _generator;
        private readonly MaRoutes _routes;

        public UrlBuilder(ILogger<UrlBuilder> logger, IOptionsMonitor<MaPageConfiguration> options, ILinkGenerator generator) 
        {
            _logger = logger;
            _generator = generator;
            _routes = options.CurrentValue.Routes;
        }

        public string AccountsLink()
        {
            return AccountsLink(string.Empty);
        }

        public string AccountsLink(string routeName, params string[] args)
        {
            if (string.IsNullOrWhiteSpace(routeName))
                return _generator.AccountsLink(string.Empty);

            var route = _routes.Accounts[routeName];
            
            if (args != null && args.Length > 0)
                route = string.Format(route, args);

            return _generator.AccountsLink(route);
        }
        public string UsersLink(string routeName, params string[] args)
        {
            var route = _routes.Identity[routeName];
            
            if (args != null && args.Length > 0)
                route = string.Format(route, args);

            return _generator.UsersLink(route);
        }

        public string CommitmentsLink(string routeName, params string[] args)
        {
            var route = _routes.Commitments[routeName];
            
            if (args != null && args.Length > 0)
                route = string.Format(route, args);

            return _generator.CommitmentsLink(route);
        }

        public string RecruitLink(string routeName, params string[] args)
        {
            var route = _routes.Recruit[routeName];
            
            if (args != null && args.Length > 0)
                route = string.Format(route, args);

            return _generator.RecruitLink(route);
        }

        public string GetLink(MaRoutes config, string routeName = null)
        {
            // RouteName -> lookup routename and get action
           // _generator.AccountsLink(null);
            //return string.IsNullOrWhiteSpace(routeName) ? config.RootUrl : config.RootUrl + config.Routes[routeName];
            return string.Empty;
        }

        public static string GetLink(MaRoutes config, string routeName, string accountId)
        {
            //return string.Format(config.RootUrl + config.Routes[routeName], accountId);
            return string.Empty;
        }

        public static string GetLink(MaRoutes config, string routeName, string clientId, string returnUrl)
        {
            //return string.Format(config.RootUrl + config.Routes[routeName], clientId, returnUrl);
            return string.Empty;
        }
    }
}