using System;
using SFA.DAS.Employer.Shared.UI.Configuration;

namespace SFA.DAS.Employer.Shared.UI
{
    public class UrlBuilder
    {
        private readonly LinkGenerator _generator;
        
        public UrlBuilder(string environment) 
        {
            _generator = new LinkGenerator(environment);
        }

        public string AccountsLink()
        {
            return AccountsLink(string.Empty);
        }

        public string AccountsLink(string routeName, params string[] args)
        {
            if (string.IsNullOrWhiteSpace(routeName))
                return _generator.AccountsLink("/");

            var route = MaRoutes.Accounts[routeName];
            
            if (args != null && args.Length > 0)
                route = string.Format(route, args);

            return _generator.AccountsLink(route);
        }

        public string FinanceLink(string routeName, params string[] args)
        {
            var route = MaRoutes.Finance[routeName];
            
            if (args != null && args.Length > 0)
                route = string.Format(route, args);

            return _generator.FinanceLink(route);
        }

        public string UsersLink(string routeName, params string[] args)
        {
            var route = MaRoutes.Identity[routeName];
            
            if (args != null && args.Length > 0)
                route = string.Format(route, args);

            return _generator.UsersLink(route);
        }

        public string CommitmentsV2Link(string routeName, params string[] args)
        {
            var route = MaRoutes.CommitmentsV2[routeName];

            if (args != null && args.Length > 0)
                route = string.Format(route, args);

            return _generator.CommitmentsV2Link(route);
        }


        public string RecruitLink(string routeName, params string[] args)
        {
            var route = MaRoutes.Recruit[routeName];
            
            if (args != null && args.Length > 0)
                route = string.Format(route, args);

            return _generator.RecruitLink(route);
        }
    }
}