using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SFA.DAS.HashingService;
using SFA.DAS.UI.Activities.DataAccess.Repositories;
using SFA.DAS.UI.Activities.Domain.Configurations;

namespace SFA.DAS.UI.Activities.Areas.ActivitiesPage.Controllers
{
    public class DogController : Controller
    {
        private IActivitiesUiRepository _repository;
        private readonly IHashingService _hashingService;

        public DogController()
        {
            
        }

        public DogController(IActivitiesUiRepository repository, IHashingService hashingService)
        {
            _repository = repository;
        }

        // GET: ActivitiesPage/Dog
        public ActionResult Index()
        {
            _repository = new ActivitiesRepository(new ActivitiesConfiguration());
            return View();
        }
    }
}