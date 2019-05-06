using System;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.UnitOfWork.Sample.Data;
using SFA.DAS.UnitOfWork.Sample.Models;

namespace SFA.DAS.UnitOfWork.Sample.Web.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        private readonly Lazy<SampleDbContext> _db;

        public HomeController(Lazy<SampleDbContext> db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            _db.Value.Foobars.Add(new Foobar(DateTime.UtcNow));
            
            return Ok();
        }
    }
}