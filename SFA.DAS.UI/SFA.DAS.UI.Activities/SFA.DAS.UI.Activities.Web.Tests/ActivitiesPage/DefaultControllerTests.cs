using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Moq;
using NuGet;
using NUnit.Framework;
using SFA.DAS.UI.Activities.DataAccess.Repositories;
using SFA.DAS.UI.Activities.Areas.ActivitiesPage.Controllers;
using SFA.DAS.UI.Activities.Areas.ActivitiesPage.Models;

namespace SFA.DAS.UI.Activities.Web.Tests.ActivitiesPage
{
    public class DefaultControllerTests
    {
        private Mock<IActivitiesUiRepository> _mockRepository;
        private List<Activity> _activities;
        private DefaultController _controller;

        private const long AccountId = 123456;

        [SetUp]
        public void Init()
        {
            
            _activities = MakeActivities();

            _mockRepository =new Mock<IActivitiesUiRepository>();
            _mockRepository.Setup(a => a.GetActivities(AccountId)).Returns(_activities);

            _controller = new DefaultController(_mockRepository.Object);
        }

        [Test]
        public void AcivitiesCollectionIsReturnedInViewModelForm()
        {
            var result = (ViewResult)_controller.Index();
            Assert.AreEqual(typeof(ActivitiesListModel), result.Model.GetType());
            var activitiesModels = ((ActivitiesListModel)result.Model).Activities;
            Assert.AreEqual(10, activitiesModels.Count);

            for (int f = 0; f < 10; f++)
            {
                Assert.AreEqual($"description1_{f}", activitiesModels[f].LineOne);
                Assert.AreEqual($"description2_{f}", activitiesModels[f].LineTwo);
                Assert.AreEqual("Today", activitiesModels[f].DayText);
            }
        }

        private List<Activity> MakeActivities()
        {
            var rtn = new List<Activity>();

            for (int f = 0; f < 10; f++)
            {
                rtn.Add(MakeActivity(f));
            }       

            return rtn;
        }

        private Activity MakeActivity(int numberOf)
        {
            return new FluentActivity()
                .AccountId(AccountId)
                .DescriptionOne($"description1_{numberOf}")
                .DescriptionTwo($"description2_{numberOf}")
                .PostedDateTime(DateTime.Now)
                .Object();
        }
    }
}
