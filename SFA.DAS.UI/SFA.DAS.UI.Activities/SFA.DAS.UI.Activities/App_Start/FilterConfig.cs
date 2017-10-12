using System.Web;
using System.Web.Mvc;

namespace SFA.DAS.UI.Activities
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
