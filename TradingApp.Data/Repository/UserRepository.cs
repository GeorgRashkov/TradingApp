
using Microsoft.EntityFrameworkCore;
using TradingApp.Data.Repository.Interfaces;
using TradingApp.GCommon.Enums;

namespace TradingApp.Data.Repository
{
    public class UserRepository: IUserRepository
    {
        private readonly ApplicationDbContext _context;
        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

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
    }
}
