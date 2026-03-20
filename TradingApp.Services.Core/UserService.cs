
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TradingApp.Data;
using TradingApp.Data.Models;
using TradingApp.GCommon;
using TradingApp.GCommon.Enums;
using TradingApp.Services.Core.Interfaces;
using TradingApp.ViewModels.User;

namespace TradingApp.Services.Core
{
    public class UserService : IUserService
    {
        private ApplicationDbContext _context;
        private const int _usersPerPage = ApplicationConstants.UsersPerPage;
        private UserManager<User> _userManager;

        public UserService(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public int UserPageIndex { get; private set; }
        public async Task<bool> DoesUserExistAsync(string userId)
        {
            return await _context.Users
                    .AsNoTracking()
                    .AnyAsync(u => u.Id == userId);
        }

        public async Task<string?> GetCreatorNameOfProductAsync(Guid productId)
        {
            string? creatorName = await _context
                .Products
                .Include(p => p.Creator)
                .AsNoTracking()
                .Where(p => p.Id == productId)
                .Select(p => p.Creator.UserName)
                .SingleOrDefaultAsync();

            return creatorName;
        }

        public async Task<string?> GetCreatorIdOfRequestAsync(Guid orderRequestId)
        {
            string? creatorId = await _context
                .OrderRequests
                .AsNoTracking()
                .Where(or => or.Id == orderRequestId)
                .Select(or => or.CreatorId)
                .SingleOrDefaultAsync();

            return creatorId;
        }

        public async Task<int> GetUserActiveSellOrdersCountAsync(string userId)
        {
            int sellOrdersCount = await _context
                .SellOrders
                 .AsNoTracking()
                 .Where(so => so.CreatorId == userId && so.Status == SellOrderStatus.active)
                 .CountAsync();
            return sellOrdersCount;
        }


        public async Task<string?> GetUserIdAsync(string userName)
        {
            string? userId = await _context
                .Users
                .AsNoTracking()
                .Where(u => u.UserName == userName)
                .Select(u => u.Id)
                .SingleOrDefaultAsync();

            return userName;
        }

        public async Task<IEnumerable<UsersViewModel>> GetUsers(int pageIndex)
        {
            int usersCount = await _context
                .Users
               .AsNoTracking()
               .CountAsync();

            if (usersCount == 0)
            { return new List<UsersViewModel>(); }

            SetUserPage(pageIndex, usersCount);


            List<User> usersFromDB = await _context
                .Users
                .AsNoTracking()
                .Skip(UserPageIndex * _usersPerPage).Take(_usersPerPage)
                .ToListAsync();

            List<UsersViewModel> users = new List<UsersViewModel>();

            foreach (User user in usersFromDB)
            {
                List<string> currentUserRoles = (List<string>)(await _userManager.GetRolesAsync(user: user));

                UsersViewModel usersViewModel = new UsersViewModel()
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Roles = string.Join(", ", currentUserRoles),
                    IsAdmin = currentUserRoles.Contains(ApplicationRoles.Admin)
                };
                users.Add(usersViewModel);
            }

            return users;
        }


        private void SetUserPage(int pageIndex, int usersCount)
        {
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageIndex = pageIndex * _usersPerPage >= usersCount ? (int)Math.Ceiling((decimal)usersCount / (decimal)_usersPerPage) - 1 : pageIndex;
            UserPageIndex = pageIndex;
        }
    }
}
