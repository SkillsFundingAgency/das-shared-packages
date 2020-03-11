﻿using Microsoft.AspNetCore.Mvc.ViewFeatures;

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

        public static bool HiddenNavigationBar(this ViewDataDictionary viewData)
        {
            return viewData.ContainsKey(ViewDataKeys.HideNavigationBar) && (bool)viewData[ViewDataKeys.HideNavigationBar];
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

        public static string GetZenDeskSectionId(this ViewDataDictionary viewData)
        {
            if (viewData.ContainsKey(ViewDataKeys.ZenDeskSectionId))
            {
                return viewData[ViewDataKeys.ZenDeskSectionId].ToString();
            }

            return string.Empty;
        }

        public static string GetZenDeskSnippetKey(this ViewDataDictionary viewData)
        {
            if (viewData.ContainsKey(ViewDataKeys.ZenDeskSnippetKey))
            {
                return viewData[ViewDataKeys.ZenDeskSnippetKey].ToString();
            }

            return string.Empty;
        }
    }
}
