using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TradingApp.GCommon;

namespace TradingApp.Controllers
{
    [Authorize(Roles = ApplicationRoles.User)]
    public class ControllerBase : Controller
    {
        //this is the Id of the currently logged user; if the user is not logged the value will be null 
        public string LoggedUserId
        {
            get { return User.FindFirst(ClaimTypes.NameIdentifier)?.Value; }
        }

        //this is the username of the currently logged user; if the user is not logged the value will be null 
        public string LoggedUserUsername
        {
            get { return User.Identity?.Name; }
        }

        //this is the last page which was accessed by the user
        public string Referer
        {
            get { return Request.Headers["Referer"].ToString(); }
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult Message()
        {
            return View();
        }
    }
}
