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
            //if (string.IsNullOrWhiteSpace(routeName) || args == null || args.Length == 0 || string.IsNullOrWhiteSpace(args[0]) ) 
            //return _generator.AccountsLink(string.Empty);

            if (string.IsNullOrWhiteSpace(routeName))
                return _generator.AccountsLink(string.Empty);

            var route = _routes.Accounts[routeName];
            
            if (args != null && args.Length > 0)
                route = string.Format(route, args);

            return _generator.AccountsLink(route);
        }

        public string FinanceLink(string routeName, params string[] args)
        {
            var route = _routes.Finance[routeName];
            
            if (args != null && args.Length > 0)
                route = string.Format(route, args);

            return _generator.FinanceLink(route);
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

        public string CommitmentsV2Link(string routeName, params string[] args)
        {
            var route = _routes.CommitmentsV2[routeName];

            if (args != null && args.Length > 0)
                route = string.Format(route, args);

            return _generator.CommitmentsV2Link(route);
        }


        public string RecruitLink(string routeName, params string[] args)
        {
            var route = _routes.Recruit[routeName];
            
            if (args != null && args.Length > 0)
                route = string.Format(route, args);

            return _generator.RecruitLink(route);
        }
    }
}