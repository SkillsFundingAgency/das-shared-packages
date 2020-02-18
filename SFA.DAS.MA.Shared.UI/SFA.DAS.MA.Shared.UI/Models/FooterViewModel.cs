using System;
using System.Collections.Generic;
using SFA.DAS.MA.Shared.UI.Models.Links;
using SFA.DAS.MA.Shared.UI.Configuration;
using SFA.DAS.MA.Shared.UI.Services;

namespace SFA.DAS.MA.Shared.UI.Models
{
    public class FooterViewModel : IFooterViewModel
    {
        const string BuiltByHRef = "http://gov.uk/esfa";
        const string SurveyHRef = "https://www.smartsurvey.co.uk/s/apprenticeshipservicefeedback/";
        const string OpenGovernmentLicenseHRef = "https://www.nationalarchives.gov.uk/doc/open-government-licence/version/3/";
        const string CrownCopyrightHRef = "https://www.nationalarchives.gov.uk/information-management/re-using-public-sector-information/uk-government-licensing-framework/crown-copyright/";

        public IReadOnlyList<Link> Links => _linkCollection.Links;

        private readonly ILinkCollection _linkCollection;
        private readonly ILinkHelper _linkHelper;
        private readonly IUrlHelper _urlHelper;

        public FooterViewModel(
            IFooterConfiguration configuration, 
            IUserContext userContext,
            ILinkCollection linkCollection = null,
            ILinkHelper linkHelper = null, 
            IUrlHelper urlHelper = null)
        {
            if (configuration == null) throw new ArgumentNullException("configuration");
            if (userContext == null) throw new ArgumentNullException("userContext");

            _linkCollection = new LinkCollection();

            _linkCollection = linkCollection ?? new LinkCollection();
            _linkHelper = linkHelper ?? new LinkHelper(_linkCollection);
            _urlHelper = urlHelper ?? new UrlHelper();

            AddOrUpdateLink(new Help(_urlHelper.GetPath(configuration.ManageApprenticeshipsBaseUrl, "service/help")));
            AddOrUpdateLink(new Feedback(SurveyHRef));
            AddOrUpdateLink(new Privacy(_urlHelper.GetPath(userContext, configuration.ManageApprenticeshipsBaseUrl, "privacy", "service")));
            if (userContext?.HashedAccountId == null)
            {
                AddOrUpdateLink(new Cookies(_urlHelper.GetPath(configuration.ManageApprenticeshipsBaseUrl, "cookieConsent")));
            }
            else
            {
                AddOrUpdateLink(new Cookies(_urlHelper.GetPath(userContext, configuration.ManageApprenticeshipsBaseUrl, "cookieConsent")));
            }
            AddOrUpdateLink(new BuiltBy(BuiltByHRef));
            AddOrUpdateLink(new OpenGovernmentLicense(OpenGovernmentLicenseHRef));
            AddOrUpdateLink(new OpenGovernmentLicenseV3(OpenGovernmentLicenseHRef));
            AddOrUpdateLink(new CrownCopyright(CrownCopyrightHRef));
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
