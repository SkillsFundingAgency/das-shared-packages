using SFA.DAS.MA.Shared.UI.Configuration;
using SFA.DAS.MA.Shared.UI.Models.Links;
using SFA.DAS.MA.Shared.UI.Services;
using System;
using System.Collections.Generic;

namespace SFA.DAS.MA.Shared.UI.Models
{
    public class HeaderViewModel : IHeaderViewModel
    {
        const string GovUkHref = "https://www.gov.uk/";
        public IUserContext UserContext { get; private set; }
        public bool MenuIsHidden { get; private set; }
        public string SelectedMenu { get; private set; }

        public IReadOnlyList<Link> Links => _linkCollection.Links;

        private readonly ILinkCollection _linkCollection;
        private readonly ILinkHelper _linkHelper;
        private readonly IUrlHelper _urlHelper;

        public HeaderViewModel(
            IHeaderConfiguration configuration, 
            IUserContext userContext,
            ILinkCollection linkCollection = null,
            ILinkHelper linkHelper = null, 
            IUrlHelper urlHelper = null)
        {
            if (configuration == null) throw new ArgumentNullException("configuration");
            UserContext = userContext ?? throw new ArgumentNullException("userContext");

            _linkCollection = linkCollection ?? new LinkCollection();
            _linkHelper = linkHelper ?? new LinkHelper(_linkCollection);
            _urlHelper = urlHelper ?? new UrlHelper();

            MenuIsHidden = false;
            SelectedMenu = "home";
            
            // floating header links
            AddOrUpdateLink(new GovUk(GovUkHref));
            AddOrUpdateLink(new Help(_urlHelper.GetPath(configuration.EmployerAccountsBaseUrl, "service/help")));
            AddOrUpdateLink(new YourAccounts(_urlHelper.GetPath(configuration.EmployerAccountsBaseUrl, "service/accounts")));
            AddOrUpdateLink(new RenameAccount(_urlHelper.GetPath(userContext, configuration.EmployerAccountsBaseUrl, "rename")));

            var returnUrl = System.Net.WebUtility.UrlEncode(_urlHelper.GetPath(configuration.EmployerAccountsBaseUrl, "service/password/change"));
            AddOrUpdateLink(new ChangePasssword(_urlHelper.GetPath(configuration.IdentityServerBaseUrl?.Replace("/identity", ""), $"account/changepassword?clientId={configuration.ClientId}&returnurl={returnUrl}")));

            returnUrl = System.Net.WebUtility.UrlEncode(_urlHelper.GetPath(configuration.EmployerAccountsBaseUrl, "service/email/change"));
            AddOrUpdateLink(new ChangeEmail(_urlHelper.GetPath(configuration.IdentityServerBaseUrl?.Replace("/identity", ""), $"account/changeemail?clientId={configuration.ClientId}&returnurl={returnUrl}")));

            AddOrUpdateLink(new NotificationSettings(_urlHelper.GetPath(configuration.EmployerAccountsBaseUrl, "settings/notifications")));
            AddOrUpdateLink(new SignOut(configuration.SignOutUrl?.AbsoluteUri));
            AddOrUpdateLink(new SignIn(_urlHelper.GetPath(configuration.EmployerAccountsBaseUrl, "service/signIn")));
            // global nav links
            AddOrUpdateLink(new Home(_urlHelper.GetPath(userContext, configuration.EmployerAccountsBaseUrl, "teams")));
            AddOrUpdateLink(new Finance(_urlHelper.GetPath(userContext, configuration.EmployerFinanceBaseUrl, "finance")));
            AddOrUpdateLink(new Recruitment(configuration.AuthorizationService, _urlHelper.GetPath(userContext, configuration.EmployerRecruitBaseUrl)));
            AddOrUpdateLink(new Apprentices(_urlHelper.GetPath(userContext, configuration.EmployerCommitmentsBaseUrl, "apprentices/home")));
            AddOrUpdateLink(new YourTeam(_urlHelper.GetPath(userContext, configuration.EmployerAccountsBaseUrl, "teams/view")));

            AddOrUpdateLink(new YourOrganisations(_urlHelper.GetPath(userContext, configuration.EmployerAccountsBaseUrl, "agreements")));
            AddOrUpdateLink(new PayeSchemes(_urlHelper.GetPath(userContext, configuration.EmployerAccountsBaseUrl, "schemes")));
        }

        public void HideMenu()
        {
            MenuIsHidden = true;
        }

        public void SelectMenu(string menu)
        {
            SelectedMenu = menu;
        }

        public void AddOrUpdateLink<T>(T link) where T : Link
        {
            _linkCollection.AddOrUpdateLink(link);
        }

        public void RemoveLink<T>() where T : Link
        {
            _linkCollection.RemoveLink<T>();
        }

        public string RenderListItemLink<T>(bool isSelected = false) where T : Link
        {
            return _linkHelper.RenderListItemLink<T>(isSelected);
        }

        public string RenderLink<T>(Func<string> before = null, Func<string> after = null, bool isSelected = false) where T : Link
        {
            return _linkHelper.RenderLink<T>(before, after, isSelected);
        }
    }
}
