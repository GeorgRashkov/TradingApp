//Area `Admin`

using Microsoft.AspNetCore.Mvc;
using TradingApp.Services.Core.Interfaces;
using TradingApp.ViewModels.InputUser;
using TradingApp.ViewModels.User;

namespace TradingApp.Areas.Admin.Controllers
{
    public class UserController : ControllerBase
    {
        private IUserService _userService;
        public UserController(IUserService userService) 
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> Users(int pageIndex)
        {
            IEnumerable<UsersViewModel> users = await _userService.GetUsers(pageIndex);

            if (users.Count() == 0)
            { return View(model: null); }

            ViewData["page"] = _userService.UserPageIndex;            

            return View(model: users);
        }


        [HttpGet]
        public async Task<IActionResult> ManageUser(string userId) 
        {
            ManagedUserModel? user = await _userService.GetManagedUserAsync(userId: userId);
            
            if(user == null) 
            { return NotFound(); }            

            return View(model: user);
        }
    }
}
