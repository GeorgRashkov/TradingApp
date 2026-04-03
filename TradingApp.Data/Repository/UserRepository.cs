
using Microsoft.EntityFrameworkCore;
using TradingApp.Data.Dtos.User;
using TradingApp.Data.Models;
using TradingApp.Data.Repository.Interfaces;
using TradingApp.GCommon.Enums;

namespace TradingApp.Data.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        //<bool methods
        public async Task<bool> DoesUserExistAsync(string userId)
        {
            return await _context.Users
                    .AsNoTracking()
                    .AnyAsync(u => u.Id == userId);
        }

        public async Task<bool> DidUserBoughtProductAsync(Guid productId, string userId)
        {
            return await _context
                .CompletedOrders
                .AsNoTracking()
                .AnyAsync(co => co.BuyerId == userId && co.ProductId == productId);
        }

        public async Task<bool> DoesCreatorHaveOtherProductsWithTheSameNameAsync(string productName, Guid productId, string creatorId)
        {
            return await _context
                .Products
                .AsNoTracking()
                .AnyAsync(p => p.Name == productName && p.Id != productId && p.CreatorId == creatorId);
        }
        //bool methods>

        //<number methods
        public async Task<int> GetUsersCountAsync()
        {
            int usersCount = await _context
               .Users
              .AsNoTracking()
              .CountAsync();

            return usersCount;
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
        //number methods>

        //<text methods
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
        //text methods>

        //<entity methods
        public async Task<IEnumerable<User>> GetUsersAsync(int skipCount, int takeCount)
        {
            List<User> users = await _context
                .Users
                .AsNoTracking()
                .Skip(skipCount).Take(takeCount)
                .ToListAsync();

            return users;
        }
        //entity methods>


        //<dto methods
        public async Task<User_CreateSellOrderEligibilityDto?> GetUserForCreateSellOrderAsync(string userId)
        {
            User_CreateSellOrderEligibilityDto? user = await _context
                .Users
                .Include(u => u.SellOrders)
                .AsNoTracking()
                .Where(u => u.Id == userId)
                .Select(u => new User_CreateSellOrderEligibilityDto
                {
                    UserId = u.Id,
                    ActiveSellOrdersCount = u.SellOrders.Where(so => so.Status == SellOrderStatus.active).Count()
                }).SingleOrDefaultAsync();

            return user;
        }

        public async Task<User_CancelSellOrderEligibilityDto?> GetUserForCancelSellOrderAsync(string userId)
        {
            User_CancelSellOrderEligibilityDto? user = await _context
               .Users
               .Include(u => u.SellOrders)
               .AsNoTracking()
               .Where(u => u.Id == userId)
               .Select(u => new User_CancelSellOrderEligibilityDto
               {
                   UserId = u.Id,
                   ActiveSellOrdersCount = u.SellOrders.Where(so => so.Status == SellOrderStatus.active).Count()
               }).SingleOrDefaultAsync();

            return user;
        }

        public async Task<User_BuySellOrderEligibilityDto?> GetUserForBuySellOrderAsync(string userId)
        {
            User_BuySellOrderEligibilityDto? user = await _context
                .Users
                .Include(u => u.Balance)
                .AsNoTracking()
                .Where(u => u.Id == userId)
                .Select(u => new User_BuySellOrderEligibilityDto
                {
                    UserId = u.Id,
                    Balance = u.Balance.Amount
                }).SingleOrDefaultAsync();

            return user;
        }
        //dto methods>


        //<operations methods
        public async Task ManageUserAsync(User user, string? lockoutMessage, bool lockoutEnabled, DateTimeOffset lockoutEnd)
        {
            _context.Attach<User>(user);

            user.LockoutMessage = lockoutMessage;
            user.LockoutEnabled = lockoutEnabled;
            user.LockoutEnd = lockoutEnd;
            
            await _context.SaveChangesAsync();            
        }
        //operations methods>
    }
}
