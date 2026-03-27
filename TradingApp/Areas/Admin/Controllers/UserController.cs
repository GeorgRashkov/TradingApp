//Area `Admin`

using Microsoft.AspNetCore.Mvc;
using TradingApp.GCommon;
using TradingApp.GCommon.ErrorCodes;
using TradingApp.Services.Core.Interfaces;
using TradingApp.ViewModels.InputUser;
using TradingApp.ViewModels.User;

namespace TradingApp.Areas.Admin.Controllers
{
    public class UserController : ControllerBase
    {
        private IUserService _userService;
        private IUserOperationsService _userOperationsService;

        private ILogger<UserController> _logger;
        public UserController(IUserService userService, IUserOperationsService userOperationsService, ILogger<UserController> logger) 
        {
            _userService = userService;
            _userOperationsService = userOperationsService;

            _logger = logger;
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

        //this method is formed based on the logic of the service method for changing the properties of a user (the change is executed by the admin) 
        //its purpose is to provide a proper error message which will be shown to the admin based on the error code
        private string Get_ManageUser_ErrorMessage(string errorCode) 
        {
            string errorMessage = errorCode switch
            {
                string code when code == UserErrorCodes.UserNotFound =>
                    "The user was not found.",

                string code when code == RoleErrorCodes.RoleNotFound =>
                "The role you are trying to assign to the user was not found.",

                string code when code == UserErrorCodes.LockedUserWithoutLockoutMessage =>
                "If you want to suspend or ban a user you need to provide a proper lockout message",

                string code when code == RoleErrorCodes.ReadOnlyRole_UpdateAttempt =>
                "You are not allowed to update admins nor to give admin roles.",

                _ => "Something went wrong."
            };

            return errorMessage;
        }

        [HttpPost]
        public async Task<IActionResult> ManageUser(ManagedUserModel user) 
        {
            if(ModelState.IsValid == false) 
            {  return View(model: user); }

            Result result;
            try
            {
                result = await _userOperationsService.ManageUserAsync(user: user);
            }
            catch (Exception e) 
            {
                _logger.LogError(e, "An error occured while attempting to change the user properties!");

                TempData["title"] = "Error";
                TempData["message"] = $"An error occured while attempting to change the user properties! Please try again later.";
                return RedirectToAction(nameof(Message));
            }

            if(result.Success == false) 
            {
                string errorMessage = Get_ManageUser_ErrorMessage(result.ErrorCode);                
                TempData["title"] = "Error";
                TempData["message"] = errorMessage;
                return RedirectToAction(nameof(Message));
            }

            TempData["title"] = "Success";
            TempData["message"] = $"The user {user.UserHelper?.UserName} was updated succesfully.";
            return RedirectToAction(nameof(Message));
        }
    }
}
