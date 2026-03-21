
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TradingApp.Data;
using TradingApp.Data.Models;
using TradingApp.GCommon;
using TradingApp.GCommon.ErrorCodes;
using TradingApp.Services.Core.Interfaces;
using TradingApp.ViewModels.InputUser;

namespace TradingApp.Services.Core
{
    public class UserOperationsService : IUserOperationsService
    {
        private ApplicationDbContext _context;
        private RoleManager<IdentityRole> _roleManager;
        private UserManager<User> _userManager;

        public UserOperationsService(ApplicationDbContext context, RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<Result> ManageUserAsync(ManagedUserModel user)
        {
            User? userFromDB = await _context.Users.FindAsync(user.UserId);

            //<validations

            //cheking user existance in the DB
            if (userFromDB == null)
            { return new Result(errorCode: UserErrorCodes.UserNotFound); }

            //make sure the readonly roles (the admin role) cannot be modified nor set
            string userCurrentRole = (await _userManager.GetRolesAsync(user: userFromDB))[0];
            if (user.Role == ApplicationRoles.Admin || userCurrentRole == ApplicationRoles.Admin) 
            { return new Result(errorCode: RoleErrorCodes.ReadOnlyRole_UpdateAttempt); }

            //checking whether the role (which is about to be assigned to the user) exists in the DB
            bool isRoleValid = await _roleManager.Roles.AnyAsync(r => r.Name == user.Role);
            if (isRoleValid == false)
            { return new Result(errorCode: RoleErrorCodes.RoleNotFound); }

            //checking whether there is a lockout message if the user was suspended
            if (user.DaysToSuspend > 0 && string.IsNullOrEmpty(user.LockoutMessage))
            { return new Result(errorCode: UserErrorCodes.LockedUserWithoutLockoutMessage); }

            //validations>

            //<update DB records
            bool isUserRoleChanged = (await _userManager.IsInRoleAsync(user: userFromDB, role: user.Role)) == false;
            if (isUserRoleChanged == true)
            {                
                await _userManager.RemoveFromRoleAsync(user: userFromDB, role: userCurrentRole);
                await _userManager.AddToRoleAsync(user: userFromDB, role: user.Role);
            }

            userFromDB.LockoutMessage = (user.DaysToSuspend > 0) ? user.LockoutMessage: null;
            userFromDB.LockoutEnabled = true;//make sure the user can be locked (`LockoutEnd` makes no effect if `LockoutEnabled` is `false`)
            userFromDB.LockoutEnd = new DateTimeOffset(DateTimeOffset.UtcNow.Date.AddDays(user.DaysToSuspend), TimeSpan.Zero);
            //update DB records>

            await _context.SaveChangesAsync();

            return new Result();
        }
    }
}
