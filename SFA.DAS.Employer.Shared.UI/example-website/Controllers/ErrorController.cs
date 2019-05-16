using System.Diagnostics;
using System.Net;
using DfE.Example.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DfE.Example.Web.Controllers
{
    [AllowAnonymous]
    public class ErrorController : Controller
    {        
        [Route("error/{id?}")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int id)
        {
            Response.StatusCode = id;

            return View(GetViewNameForStatus(id), new ErrorViewModel { StatusCode = id, RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private string GetViewNameForStatus(int statusCode)
        {
            switch(statusCode)
            {
                case (int)HttpStatusCode.NotFound:
                    return "PageNotFound";
                case (int)HttpStatusCode.Forbidden:
                case (int)HttpStatusCode.Unauthorized:
                    return "AccessDenied";
                default:
                    return "Error";
            }
        }
    }
}