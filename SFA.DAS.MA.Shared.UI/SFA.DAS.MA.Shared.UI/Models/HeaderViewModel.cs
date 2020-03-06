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

        public bool UseLegacyStyles { get; private set; }

        private readonly ILinkCollection _linkCollection;
        private readonly ILinkHelper _linkHelper;
        private readonly IUrlHelper _urlHelper;
        
        public HeaderViewModel(
            IHeaderConfiguration configuration, 
            IUserContext userContext,
            ILinkCollection linkCollection = null,
            ILinkHelper linkHelper = null, 
            IUrlHelper urlHelper = null,
            bool useLegacyStyles = false)
        {
            if (configuration == null) throw new ArgumentNullException("configuration");
            UserContext = userContext ?? throw new ArgumentNullException("userContext");

            _linkCollection = linkCollection ?? new LinkCollection();
            _linkHelper = linkHelper ?? new LinkHelper(_linkCollection);
            _urlHelper = urlHelper ?? new UrlHelper();
            UseLegacyStyles = useLegacyStyles;

            MenuIsHidden = false;
            SelectedMenu = "home";
            
            // floating header links
            AddOrUpdateLink(new GovUk(GovUkHref, isLegacy: UseLegacyStyles));
            AddOrUpdateLink(new ManageApprenticeships(configuration.ManageApprenticeshipsBaseUrl, UseLegacyStyles ? "" : "govuk-header__link govuk-header__link--service-name"));
            AddOrUpdateLink(new Help(_urlHelper.GetPath(configuration.ManageApprenticeshipsBaseUrl, "service/help"), UseLegacyStyles ? "" : "menu-main"));
            AddOrUpdateLink(new YourAccounts(_urlHelper.GetPath(configuration.ManageApprenticeshipsBaseUrl, "service/accounts"), UseLegacyStyles ? "" : "sub-menu-item"));
            AddOrUpdateLink(new RenameAccount(_urlHelper.GetPath(userContext, configuration.ManageApprenticeshipsBaseUrl, "rename"), UseLegacyStyles ? "" : "sub-menu-item"));

            var returnUrl = configuration.ChangePasswordReturnUrl?.AbsoluteUri ?? _urlHelper.GetPath(configuration.ManageApprenticeshipsBaseUrl, "service/password/change");
            AddOrUpdateLink(new ChangePassword(_urlHelper.GetPath(configuration.AuthenticationAuthorityUrl?.Replace("/identity", ""), $"account/changepassword?clientId={configuration.ClientId}&returnurl={System.Net.WebUtility.UrlEncode(returnUrl)}"), UseLegacyStyles ? "" : "sub-menu-item"));

            returnUrl = configuration.ChangeEmailReturnUrl?.AbsoluteUri ?? _urlHelper.GetPath(configuration.ManageApprenticeshipsBaseUrl, "service/email/change");
            AddOrUpdateLink(new ChangeEmail(_urlHelper.GetPath(configuration.AuthenticationAuthorityUrl?.Replace("/identity", ""), $"account/changeemail?clientId={configuration.ClientId}&returnurl={System.Net.WebUtility.UrlEncode(returnUrl)}"), UseLegacyStyles ? "" : "sub-menu-item"));

            AddOrUpdateLink(new NotificationSettings(_urlHelper.GetPath(configuration.ManageApprenticeshipsBaseUrl, "settings/notifications"), UseLegacyStyles ? "" : "sub-menu-item"));
            if (configuration.SignOutUrl != null)
            {
                AddOrUpdateLink(new SignOut(configuration.SignOutUrl.IsAbsoluteUri ? configuration.SignOutUrl?.AbsoluteUri : configuration.SignOutUrl.OriginalString, UseLegacyStyles ? "" : "menu-main"));
            }
            AddOrUpdateLink(new SignIn(_urlHelper.GetPath(configuration.ManageApprenticeshipsBaseUrl, "service/signIn")));
            // global nav links
            AddOrUpdateLink(new Home(_urlHelper.GetPath(userContext, configuration.ManageApprenticeshipsBaseUrl, "teams")));
            AddOrUpdateLink(new Finance(_urlHelper.GetPath(userContext, configuration.EmployerFinanceBaseUrl, "finance")));
            AddOrUpdateLink(new Recruitment(_urlHelper.GetPath(userContext, configuration.EmployerRecruitBaseUrl)));
            AddOrUpdateLink(new Apprentices(_urlHelper.GetPath(userContext, configuration.EmployerCommitmentsBaseUrl, "apprentices/home")));
            AddOrUpdateLink(new YourTeam(_urlHelper.GetPath(userContext, configuration.ManageApprenticeshipsBaseUrl, "teams/view")));

            AddOrUpdateLink(new YourOrganisations(_urlHelper.GetPath(userContext, configuration.ManageApprenticeshipsBaseUrl, "agreements")));
            AddOrUpdateLink(new PayeSchemes(_urlHelper.GetPath(userContext, configuration.ManageApprenticeshipsBaseUrl, "schemes")));
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

        public string RenderListItemLink<T>(bool isSelected = false, string @class = "") where T : Link
        {
            return _linkHelper.RenderListItemLink<T>(isSelected, @class);
        }

        public string RenderLink<T>(Func<string> before = null, Func<string> after = null, bool isSelected = false) where T : Link
        {
            return _linkHelper.RenderLink<T>(before, after, isSelected);
        }
    }
}
