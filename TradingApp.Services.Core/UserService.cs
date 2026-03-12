
using Microsoft.EntityFrameworkCore;
using TradingApp.Data;
using TradingApp.GCommon.Enums;
using TradingApp.Services.Core.Interfaces;

namespace TradingApp.Services.Core
{
    public class UserService: IUserService
    {
        private ApplicationDbContext _context;
        public UserService(ApplicationDbContext context)
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

    }
}
