using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.DfESignIn.Auth.Controllers
{
    [Route("service/keepalive")]
    public class SessionKeepAliveController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index()
        {
           return NoContent();
        }
    }
} 