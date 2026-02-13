using Microsoft.EntityFrameworkCore;
using TradingApp.Data.Models;
using TradingApp.GCommon.Enums;

namespace TradingApp.Data.Seed
{
    public class SellOrderSeeder
    {
        private readonly ApplicationDbContext _context;

        public SellOrderSeeder(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            if (await _context.SellOrders.AnyAsync())
               { return; }
                     

            var productsAndCreatorsIds = await _context
               .Products
               .AsNoTracking()
               .OrderByDescending(p => p.Name)
               .Select(p => new { p.Id, p.CreatorId })               
               .Take(9)
               .ToListAsync();

            int activeSellOrdersCount = productsAndCreatorsIds.Count-1;           
            List<SellOrder> sellOrders = new List<SellOrder>();

            for (int i = 0; i < activeSellOrdersCount; i++)
            {              
                SellOrder activeSellOrder = new SellOrder()
                {
                    Status = SellOrderStatus.active,
                    CreatedAt = DateTime.UtcNow,
                    CreatorId = productsAndCreatorsIds[i].CreatorId,
                    ProductId = productsAndCreatorsIds[i].Id,
                };
                sellOrders.Add(activeSellOrder);
            }
            await _context.SellOrders.AddRangeAsync(sellOrders);
                      

            SellOrder cancelledOrder = new SellOrder()
            {
                Status = SellOrderStatus.cancelled,
                CreatedAt = DateTime.UtcNow,
                CreatorId = productsAndCreatorsIds[productsAndCreatorsIds.Count - 1].CreatorId,
                ProductId = productsAndCreatorsIds[productsAndCreatorsIds.Count - 1].Id,
            };
            await _context.SellOrders.AddAsync(cancelledOrder);


            await _context.SaveChangesAsync();
        }
    }
}
