using System.Web;
using System.Web.Mvc;

namespace SFA.DAS.DfESignIn.SampleSite.Classic
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
