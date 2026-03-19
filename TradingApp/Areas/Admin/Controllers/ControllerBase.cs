//Area `Admin`

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TradingApp.GCommon;

namespace TradingApp.Areas.Admin.Controllers
{
    [Area(ApplicationRoles.Admin)]
    [Authorize(Roles = ApplicationRoles.Admin)]
    public class ControllerBase : Controller
    {
        
    }
}
