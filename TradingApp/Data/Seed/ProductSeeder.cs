using Microsoft.EntityFrameworkCore;
using TradingApp.Data.Enums;
using TradingApp.Data.Models;

namespace TradingApp.Data.Seed
{
    public class ProductSeeder
    {
        private readonly ApplicationDbContext _context;

        public ProductSeeder(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            if (await _context.Products.AnyAsync())
               { return; }

             List<string> userIds = await _context
                .Users
                .AsNoTracking()
                .OrderBy(u => u.UserName)
                .Select(u => u.Id)
                .Take(5)
                .ToListAsync();

            int approvedProductsCount = 15;
            int productsPerUser = 3;//determines both the user count and products per user
            List<Product> products = new List<Product>();


            for (int i = 1; i <= approvedProductsCount; i++)
            {
                int userIdIndex = i % productsPerUser;

                Product approvedProduct = new Product()
                {
                    Name = $"Product ({i})",
                    Description = $"Some product description ({i}).",
                    Price = i + 10,
                    Status = ProductStatus.approved,
                    CreatorId = userIds[userIdIndex]
                };
                products.Add(approvedProduct);
            }
            await _context.Products.AddRangeAsync(products);

            Product notCheckedProduct = new Product()
            {
                Name = "Not checked product (-1)",
                Description = "Some not checked product description (-1).",
                Price = 1_000,
                Status = ProductStatus.inspection,
                CreatorId = userIds[3]
            };
            await _context.Products.AddAsync(notCheckedProduct);

            Product badProduct = new Product()
            {
                Name = "Bad product (-1)",
                Description = "Some bad product description (-1).",
                Price = 10_000,
                Status = ProductStatus.disapproved,
                CreatorId = userIds[4]
            };
            await _context.Products.AddAsync(badProduct);



            await _context.SaveChangesAsync();
        }
        
    }
}
