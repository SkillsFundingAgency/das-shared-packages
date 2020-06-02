using System;
using SFA.DAS.MA.Shared.UI.Configuration;
using SFA.DAS.MA.Shared.UI.Services;

namespace SFA.DAS.MA.Shared.UI.Models
{
    public class CookieBannerViewModel : ICookieBannerViewModel
    {
        private const string CookieConsentPath = "cookieConsent";
        private const string CookieDetailsPath = CookieConsentPath + "/details";

        private readonly IFooterConfiguration _configuration;
        private readonly IUserContext _userContext;
        private readonly IUrlHelper _urlHelper;

        public CookieBannerViewModel(
            IFooterConfiguration configuration, 
            IUserContext userContext,
            IUrlHelper urlHelper = null)
        {
            if (configuration == null) throw new ArgumentNullException("configuration");
            if (userContext == null) throw new ArgumentNullException("userContext");

            _urlHelper = urlHelper ?? new UrlHelper();

            _configuration = configuration;
            _userContext = userContext;
        }

        public string CookieConsentUrl => _userContext?.HashedAccountId == null 
            ? _urlHelper.GetPath(_configuration.ManageApprenticeshipsBaseUrl, CookieConsentPath)
            : _urlHelper.GetPath(_userContext, _configuration.ManageApprenticeshipsBaseUrl, CookieConsentPath);
        
        public string CookieDetailsUrl => _userContext?.HashedAccountId == null
            ? _urlHelper.GetPath(_configuration.ManageApprenticeshipsBaseUrl, CookieDetailsPath)
            : _urlHelper.GetPath(_userContext, _configuration.ManageApprenticeshipsBaseUrl, CookieDetailsPath);
    }
}
