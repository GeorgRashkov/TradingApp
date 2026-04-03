
using Microsoft.EntityFrameworkCore;
using TradingApp.Data.Models;
using TradingApp.Data.Repository.Interfaces;
using TradingApp.GCommon.Enums;

namespace TradingApp.Data.Repository
{
    public class SellOrderRepository: ISellOrderRepository
    {
        private readonly ApplicationDbContext _context;
        public SellOrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        //<entity methods
        public async Task<IEnumerable<SellOrder>> GetActiveSellOrdersOfProductAsync(Guid productId, int ordersCount)
        {
            List<SellOrder> sellOrders = await _context
                .SellOrders
                .Where(so => so.ProductId == productId && so.Status == SellOrderStatus.active)
                .OrderBy(so => so.CreatedAt)
                .Take(ordersCount)
                .ToListAsync();

            return sellOrders;
        }

        //entity methods>

        //<operation methods
        public async Task CreateSellOrdersAsync(IEnumerable<SellOrder> sellOrders)
        {
            await _context.SellOrders.AddRangeAsync(sellOrders);
            
            int affectedEntities = await _context.SaveChangesAsync();
            if (affectedEntities != sellOrders.Count())
            {
                throw new Exception("Failed to create sell orders.");
            }
        }


        public async Task CancelSellOrdersAsync(IEnumerable<SellOrder> sellOrders)
        {
            _context.AttachRange(sellOrders);

            foreach (SellOrder sellOrder in sellOrders)
            {                
                sellOrder.Status = SellOrderStatus.cancelled;                
            }

            await _context.SaveChangesAsync();            
        }

        public async Task BuySellOrderAsync(Guid productId, string buyerId)
        {
            SellOrder sellOrder = (await _context
               .SellOrders
               .Include(so => so.Product)
               .Where(so => so.ProductId == productId && so.Status == SellOrderStatus.active)
               .OrderBy(so => so.CreatedAt)
               .FirstOrDefaultAsync())!;

            OrderRequest? orderRequest = await _context
                .OrderRequests
                .Include(or => or.SellOrderSuggestions)
                .Where(or => or.CreatorId == buyerId && or.SellOrderSuggestions.Any(sos => sos.ProductId == productId))
                .OrderBy(or => or.CreatedAt)
                .FirstOrDefaultAsync();

            User buyer = (await _context
                .Users
                .Include(u => u.Balance)
                .Where(u => u.Id == buyerId)
                .SingleOrDefaultAsync())!;

            User seller = (await _context
                .Users
                .Include(u => u.Balance)
                .Where(u => u.Id == sellOrder.CreatorId)
                .SingleOrDefaultAsync())!;


            decimal productPrice = sellOrder.Product.Price;
            decimal platformFee = 0.1m * productPrice;


            CompletedOrder completedOrder = new CompletedOrder()
            {
                TitleForBuyer = $"Product {sellOrder.Product.Name} purchased successfully",
                TitleForSeller = $"Product {sellOrder.Product.Name}  sold successfully",
                PricePaid = productPrice,
                PlatformFee = platformFee,
                SellerRevenue = productPrice - platformFee,
                CompletedAt = DateTime.UtcNow,

                ProductId = sellOrder.Product.Id,
                BuyerId = buyer.Id,
                SellerId = seller.Id,
            };

            buyer.Balance.Amount -= completedOrder.PricePaid;
            seller.Balance.Amount += completedOrder.SellerRevenue;
            sellOrder.Status = SellOrderStatus.completed;
            if (orderRequest != null)
            { orderRequest.Status = OrderRequestStatus.completed; }
            await _context.CompletedOrders.AddAsync(completedOrder);

            await _context.SaveChangesAsync();            
        }

        //operation methods>
    }
}
