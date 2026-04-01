
using Microsoft.EntityFrameworkCore;
using TradingApp.Data.Models;
using TradingApp.Data.Repository.Interfaces;
using TradingApp.Data.Dtos.CompletedOrder;

namespace TradingApp.Data.Repository
{
    public class CompletedOrderRepository: ICompletedOrderRepository
    {
        private readonly ApplicationDbContext _context;
        public CompletedOrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        

        public async Task<int> GetCompletedOrdersCountAsync(string userId)
        {
            int completedOrdersCount = await _context
                .CompletedOrders
                .AsNoTracking()
                .Where(co => co.SellerId == userId || co.BuyerId == userId)
                .CountAsync();

            return completedOrdersCount;
        }

        public async Task<IEnumerable<CompletedOrderDto>> GetCompletedOrdersAsync(string userId, int skipCount, int takeCount)
        {
            List<CompletedOrderDto> completedOrders = await _context
                .CompletedOrders
                .AsNoTracking()
                .Where(co => co.SellerId == userId || co.BuyerId == userId)
                .OrderByDescending(co => co.CompletedAt)
                .Skip(skipCount).Take(takeCount)
                .Select(co => 
                new CompletedOrderDto
                {
                    Id = co.Id,
                    BuyerId = co.BuyerId,
                    TitleForBuyer = co.TitleForBuyer,
                    TitleForSeller = co.TitleForSeller,
                    CompletedAt = co.CompletedAt
                })
                .ToListAsync();

            return completedOrders;
        }


        public async Task<CompletedOrder?> GetCompletedOrderAsync(Guid completedOrderId) 
        {
            CompletedOrder? completedOrder = await _context
                .CompletedOrders
                .AsNoTracking()
                .Include(co => co.Seller)
                .Include(co => co.Buyer)
                .Include(co => co.Product)
                .Where(co => co.Id == completedOrderId)
                .SingleOrDefaultAsync();

            return completedOrder;
        }
    }
}
