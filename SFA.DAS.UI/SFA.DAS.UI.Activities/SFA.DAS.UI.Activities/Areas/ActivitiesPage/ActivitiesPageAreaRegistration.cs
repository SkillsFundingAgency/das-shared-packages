using System.Web.Mvc;

namespace SFA.DAS.UI.Activities.Areas.ActivitiesPage
{
    public class ActivitiesPageAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "ActivitiesPage";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "ActivitiesPage_default",
                "ActivitiesPage/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}