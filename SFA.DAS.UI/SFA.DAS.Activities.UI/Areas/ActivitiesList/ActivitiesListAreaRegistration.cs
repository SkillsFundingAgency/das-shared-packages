using System.Web.Mvc;

namespace SFA.DAS.Activities.Areas.ActivitiesList
{
    public class ActivitiesListAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "ActivitiesList";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "ActivitiesList_default",
                "ActivitiesList/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}