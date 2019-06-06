using Microsoft.AspNetCore.Mvc.ViewFeatures;

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
    }
}
