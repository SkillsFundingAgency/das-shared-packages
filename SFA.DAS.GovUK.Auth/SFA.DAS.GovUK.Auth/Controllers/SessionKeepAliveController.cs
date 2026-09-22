using Microsoft.AspNetCore.Mvc;
using SFA.DAS.GovUK.Auth.Controllers.Routes;

namespace SFA.DAS.GovUK.Auth.Controllers;

[Route(ServiceRoutes.Paths.Controller)]
public class SessionKeepAliveController : Controller
{
    [HttpGet(ServiceRoutes.Paths.KeepAlive, Name = ServiceRoutes.Names.KeepAlive)]
    public IActionResult Index()
    {
        return NoContent();
    }
}