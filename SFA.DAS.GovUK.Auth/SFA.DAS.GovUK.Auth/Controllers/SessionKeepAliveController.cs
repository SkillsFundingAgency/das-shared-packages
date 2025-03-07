using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.GovUK.Auth.Controllers;

[Route("service/keepalive")]
public class SessionKeepAliveController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return NoContent();
    }
}