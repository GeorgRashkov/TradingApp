
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TradingApp.Data.Models;
using TradingApp.Data.Repository.Interfaces;
using TradingApp.GCommon;
using TradingApp.Services.Core.Interfaces;
using TradingApp.ViewModels.InputUser;
using TradingApp.ViewModels.User;

namespace TradingApp.Services.Core
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private const int _usersPerPage = ApplicationConstants.UsersPerPage;
        private UserManager<User> _userManager;
        private RoleManager<IdentityRole> _roleManager;

        public UserService(IUserRepository userRepository, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public int UserPageIndex { get; private set; }
       
        public async Task<string?> GetCreatorNameOfProductAsync(Guid productId)
        {
            string? creatorName = await _userRepository.GetCreatorNameOfProductAsync(productId: productId);
            return creatorName;
        }

        public async Task<string?> GetCreatorIdOfRequestAsync(Guid orderRequestId)
        {
            string? creatorId = await _userRepository.GetCreatorIdOfRequestAsync(orderRequestId: orderRequestId);
            return creatorId;
        }

        public async Task<int> GetUserActiveSellOrdersCountAsync(string userId)
        {
            int sellOrdersCount = await _userRepository.GetUserActiveSellOrdersCountAsync(userId: userId);
            return sellOrdersCount;
        }


        public async Task<string?> GetUserIdAsync(string userName)
        {
            string? userId = await _userRepository.GetUserIdAsync(userName: userName);
            return userName;
        }

        

        public async Task<ManagedUserModel?> GetManagedUserAsync(string userId) 
        {
            User? user = await _userManager.Users.Where(u => u.Id == userId).SingleOrDefaultAsync();
            
            if(user == null) 
            { return null; }

            Helper_ManagedUserModel userHelper = new Helper_ManagedUserModel()
            {
                UserName = user.UserName,
                Roles = await _roleManager.Roles.Where(r => r.Name != ApplicationRoles.Admin).Select(r => r.Name).ToListAsync()
            };

            int daysToSuspend = GetDaysToSuspendUser(user);
            
            ManagedUserModel managedUser = new ManagedUserModel()
            {
                UserId = userId,
                DaysToSuspend = daysToSuspend,                
                Role = (await _userManager.GetRolesAsync(user: user))[0],
                LockoutMessage = user.LockoutMessage,
                UserHelper = userHelper
            };

            return managedUser;
        }


        public async Task<IEnumerable<UsersViewModel>> GetUsersAsync(int pageIndex)
        {
            int usersCount = await _userRepository.GetUsersCountAsync();

            if (usersCount == 0)
            { return new List<UsersViewModel>(); }

            SetUserPage(pageIndex, usersCount);


            IEnumerable<User> usersFromDB = await _userRepository.GetUsersAsync(skipCount: UserPageIndex * _usersPerPage, takeCount: _usersPerPage);

            List<UsersViewModel> users = new List<UsersViewModel>();

            foreach (User user in usersFromDB)
            {
                string currentUserRole = (await _userManager.GetRolesAsync(user: user))[0];                

                UsersViewModel usersViewModel = new UsersViewModel()
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Role = currentUserRole,
                    DaysToSuspend = GetDaysToSuspendUser(user),                    
                    IsAdmin = currentUserRole == ApplicationRoles.Admin
                };
                users.Add(usersViewModel);
            }

            return users;
        }

        private int GetDaysToSuspendUser(User user) 
        {
            DateTimeOffset? userLockoutTime = user.LockoutEnd;
            int daysToSuspend = 0;
            if (userLockoutTime != null)
            {
                daysToSuspend = (userLockoutTime.Value - DateTimeOffset.UtcNow.Date).Days;
                daysToSuspend = Math.Max(0, daysToSuspend);
            }

            return daysToSuspend;
        }

        private void SetUserPage(int pageIndex, int usersCount)
        {
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageIndex = pageIndex * _usersPerPage >= usersCount ? (int)Math.Ceiling((decimal)usersCount / (decimal)_usersPerPage) - 1 : pageIndex;
            UserPageIndex = pageIndex;
        }
    }
}
