using Microsoft.EntityFrameworkCore;
using TradingApp.GCommon.Enums;
using TradingApp.Data.Models;

namespace TradingApp.Data.Seed
{
    public class OrderRequestSeeder
    {
        private readonly ApplicationDbContext _context;

        public OrderRequestSeeder(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            if (await _context.OrderRequests.AnyAsync())
               { return; }

            List<string> userIds = await _context
               .Users
               .AsNoTracking()
               .OrderByDescending(u => u.UserName)
               .Select(u => u.Id)
               .Take(3)
               .ToListAsync();

            int orderRequestsCount = userIds.Count*3;
            int orderRequestsPerUser = 3;//determines both the user count and the Requests per user
            List<OrderRequest> orderRequests = new List<OrderRequest>();


            for (int i = 1; i <= orderRequestsCount; i++)
            {
                int userIdIndex = i % orderRequestsPerUser;

                OrderRequest orderRequest = new OrderRequest()
                {
                    Title = $"Request ({i})",
                    Description = $"Some request description describing the desired 3D model ({i}).",
                    MaxPrice = i + 50,
                    Status = OrderRequestStatus.active,
                    CreatorId = userIds[userIdIndex]
                };
                orderRequests.Add(orderRequest);
            }
            await _context.OrderRequests.AddRangeAsync(orderRequests);


            OrderRequest cancelledOrderRequest = new OrderRequest()
            {
                Title = $"Cancelled Request (-1)",
                Description = $"Some request description describing the desired 3D model (-1).",
                MaxPrice = 5,
                Status = OrderRequestStatus.cancelled,
                CreatorId = userIds[0]
            };
            await _context.OrderRequests.AddAsync(cancelledOrderRequest);



            await _context.SaveChangesAsync();
        }

    }
}
