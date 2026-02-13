using Microsoft.EntityFrameworkCore;
using TradingApp.GCommon.Enums;
using TradingApp.Data.Models;

namespace TradingApp.Data.Seed
{
    public class PurchaseOrderSeeder
    {
        private readonly ApplicationDbContext _context;

        public PurchaseOrderSeeder(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            if (await _context.PurchaseOrders.AnyAsync())
               { return; }

            List<string> userIds = await _context
               .Users
               .AsNoTracking()
               .OrderByDescending(u => u.UserName)
               .Select(u => u.Id)
               .Take(3)
               .ToListAsync();

            int purchaseOrdersCount = userIds.Count*3;
            int purchaseOrdersPerUser = 3;//determines both the user count and purchase orders per user
            List<PurchaseOrder> purchaseOrders = new List<PurchaseOrder>();


            for (int i = 1; i <= purchaseOrdersCount; i++)
            {
                int userIdIndex = i % purchaseOrdersPerUser;

                PurchaseOrder purchaseOrder = new PurchaseOrder()
                {
                    Title = $"Purchase ({i})",
                    Description = $"Some purchase description describing the desired 3D model ({i}).",
                    MaxPrice = i + 50,
                    Status = PurchaseOrderStatus.active,
                    CreatorId = userIds[userIdIndex]
                };
                purchaseOrders.Add(purchaseOrder);
            }
            await _context.PurchaseOrders.AddRangeAsync(purchaseOrders);


            PurchaseOrder cancelledPurchaseOrder = new PurchaseOrder()
            {
                Title = $"Cancelled Purchase (-1)",
                Description = $"Some purchase description describing the desired 3D model (-1).",
                MaxPrice = 5,
                Status = PurchaseOrderStatus.cancelled,
                CreatorId = userIds[0]
            };
            await _context.PurchaseOrders.AddAsync(cancelledPurchaseOrder);



            await _context.SaveChangesAsync();
        }

    }
}
