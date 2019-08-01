using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace SFA.DAS.Employer.Shared.UI
{
    public static class ViewDataExtensions
    {
        public static NavigationSection SelectedNavigationSection(this ViewDataDictionary viewData)
        {
            return viewData.ContainsKey(ViewDataKeys.SelectedNavigationSection)
                ? (NavigationSection)viewData[ViewDataKeys.SelectedNavigationSection]
                : NavigationSection.None;
        }
    }
}
