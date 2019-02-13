using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.Http.TestHarness.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public IActionResult Index()
        {
            return Ok("WooHoo! Test works...");
        }
    }
}