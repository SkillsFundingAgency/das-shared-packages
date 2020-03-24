using System.Web;
using System.Web.Mvc;

namespace SFA.DAS.MA.Shared.UI.TestSite.Framework
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
