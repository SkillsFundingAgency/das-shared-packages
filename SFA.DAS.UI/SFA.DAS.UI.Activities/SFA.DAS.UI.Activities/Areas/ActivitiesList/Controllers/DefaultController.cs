using System.Linq;
using System.Web.Mvc;
using SFA.DAS.UI.Activities.Areas.ActivitiesList.Mappers;
using SFA.DAS.UI.Activities.DataAccess.Repositories;
using SFA.DAS.UI.Activities.Domain.Configurations;
using ActivitiesListModel = SFA.DAS.UI.Activities.Areas.ActivitiesList.Models.ActivitiesListModel;

namespace SFA.DAS.UI.Activities.Areas.ActivitiesList.Controllers
{
    public class DefaultController : Controller
    {
        private readonly IActivitiesUiRepository _repository;

        public DefaultController()
        {
            //if (repository == null)
                _repository = new ActivitiesRepository(new ActivitiesConfiguration());
            //else
                //_repository = repo;

        }

        // GET: ActivitiesList/Default
        public ActionResult Index()
        {
            var activities = _repository.GetActivities("OwnerId");
            //var activitiesGroupedByType = messages.GroupBy(a => a.Type);
            var summarisedActivities=new ActivityMapper().SummariseCollections(activities);

            //var activitiesView = messages.Select(activityMapper.SummariseCollections);
            //activitiesView.Add(new ActivityModel("acc1", "activityType1","Description 1", "url",DateTime.Now.ToString("O")));
            //activitiesView.Add(new ActivityModel("acc2", "activityType1", "Description 2", "url", DateTime.Now.ToString("O")));
            var activitiesView=new ActivitiesListModel(summarisedActivities.ToList());
            return PartialView(activitiesView);
        }

        //private List<ActivityModel> GroupByType(ActivitiesModel model)
        //{
        //    return model.Activities.Where(a => a.PostedDateTime.).GroupBy(a => a.ActivityType);
        //}

        //private class ActivitiesRepository : IActivitiesUiRepository
        //{
        //    public async Task<IEnumerable<Activity>> GetActivities(string ownerId)
        //    {

        //            var rtn = new List<Activity>()
        //            {
        //                MakeActivity(Activity.ActivityType.ActivityOne, DateTime.Now.AddHours(-1)),
        //                MakeActivity(Activity.ActivityType.ActivityTwo, DateTime.Now.AddHours(-3)),
        //                MakeActivity(Activity.ActivityType.ActivityTwo, DateTime.Now.AddHours(-4)),
        //                MakeActivity(Activity.ActivityType.ActivityThree, DateTime.Now.AddHours(-9)),
        //                MakeActivity(Activity.ActivityType.ActivityOne, DateTime.Now.AddHours(-8)),
        //                MakeActivity(Activity.ActivityType.ActivityTwo, DateTime.Now.AddHours(-20)),
        //                MakeActivity(Activity.ActivityType.ActivityFour, DateTime.Now.AddHours(-10)),
        //                MakeActivity(Activity.ActivityType.ActivityThree, DateTime.Now.AddHours(-40))
        //            };
                
        //        return rtn;
        //    }

        //    private Activity MakeActivity(Activity.ActivityType type, DateTime timeAdded)
        //    {
        //        return new FluentActivity()
        //            .ActivityType(type)
        //            .DescriptionFull("desc full")
        //            .DescriptionPlural("things added")
        //            .DescriptionSingular("thing added")
        //            .OwnerId("Me")
        //            .PostedDateTime(timeAdded)
        //            .Url("http...")
        //            .Object();
        //    }
        //}
    }
}