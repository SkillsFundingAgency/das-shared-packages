using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SFA.DAS.Provider.Shared.UI.Models;

namespace SFA.DAS.Provider.Shared.UI.Extensions
{
    public static class ViewDataExtensions
    {
        public static NavigationSection SelectedNavigationSection(this ViewDataDictionary viewData)
        {
            return viewData.ContainsKey(ViewDataKeys.SelectedNavigationSection)
                ? (NavigationSection)viewData[ViewDataKeys.SelectedNavigationSection]
                : NavigationSection.Home;
        }

        public static bool HiddenAccountHeader(this ViewDataDictionary viewData)
        {
            return viewData.ContainsKey(ViewDataKeys.HideAccountHeader) && (bool)viewData[ViewDataKeys.HideAccountHeader];
        }

        public static bool HiddenNavigationLinks(this ViewDataDictionary viewData)
        {   
            return viewData.ContainsKey(ViewDataKeys.HideNavigationLinks) && (bool)viewData[ViewDataKeys.HideNavigationLinks];
        }

        public static bool ShowBetaPhaseBanner(this ViewDataDictionary viewData)
        {
            return viewData.ContainsKey(ViewDataKeys.ShowBetaPhaseBanner) && (bool)viewData[ViewDataKeys.ShowBetaPhaseBanner];
        }

        public static NavigationSection[] SuppressedNavigationSections(this ViewDataDictionary viewData)
        {
            return viewData.ContainsKey(ViewDataKeys.SuppressNavigationSection)
                ? (NavigationSection[]) viewData[ViewDataKeys.SuppressNavigationSection]
                : new NavigationSection[0];
        }

        public static bool EnableGoogleAnalytics(this ViewDataDictionary viewData)
        {
            return viewData.ContainsKey(ViewDataKeys.EnableGoogleAnalytics) && (bool)viewData[ViewDataKeys.EnableGoogleAnalytics];
        }

        public static bool EnableCookieBanner(this ViewDataDictionary viewData)
        {
            return viewData.ContainsKey(ViewDataKeys.EnableCookieBanner) && (bool) viewData[ViewDataKeys.EnableCookieBanner];
        }

        public static string GetZenDeskSectionId(this ViewDataDictionary viewData)
        {
            if (viewData.ContainsKey(ViewDataKeys.ZenDeskConfiguration))
            {
                var zenDeskConfig = viewData[ViewDataKeys.ZenDeskConfiguration] as ZenDeskConfiguration;
                return zenDeskConfig?.SectionId;
            }

            return null;
        }

        public static string GetZenDeskSnippetKey(this ViewDataDictionary viewData)
        {
            if (viewData.ContainsKey(ViewDataKeys.ZenDeskConfiguration))
            {
                var zenDeskConfig = viewData[ViewDataKeys.ZenDeskConfiguration] as ZenDeskConfiguration;
                return zenDeskConfig?.SnippetKey;
            }

            return null;
        }

        public static string GetCobrowsingSnippetKey(this ViewDataDictionary viewData)
        {
            if (viewData.ContainsKey(ViewDataKeys.ZenDeskConfiguration))
            {
                var zenDeskConfig = viewData[ViewDataKeys.ZenDeskConfiguration] as ZenDeskConfiguration;
                return zenDeskConfig?.CobrowsingSnippetKey;
            }

            return null;
        }

        /// <summary>
        /// Method to get the DfESignIn Status from viewData.
        /// </summary>
        /// <param name="viewData"></param>
        /// <returns>boolean.</returns>
        public static bool GetDfESignInStatus(this ViewDataDictionary viewData)
        {
            return viewData.ContainsKey(ViewDataKeys.UseDfESignIn) && (bool)viewData[ViewDataKeys.UseDfESignIn];
        }
    }
}
