using System;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.UnitOfWork.Sample.Data;
using SFA.DAS.UnitOfWork.Sample.Models;

namespace SFA.DAS.UnitOfWork.Sample.Web.Controllers
{
    [Route("")]
    public class FoobarsController : Controller
    {
        private readonly Lazy<SampleDbContext> _db;

        public FoobarsController(Lazy<SampleDbContext> db)
        {
            _db = db;
        }

        public IActionResult Create()
        {
            _db.Value.Foobars.Add(new Foobar(DateTime.UtcNow));
            
            return Ok($"{nameof(Foobar)} created");
        }
    }
}