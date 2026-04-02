
using Microsoft.EntityFrameworkCore;
using TradingApp.Data.Dtos.Product;
using TradingApp.Data.Models;
using TradingApp.Data.Repository.Interfaces;
using TradingApp.GCommon;
using TradingApp.GCommon.Enums;

namespace TradingApp.Data.Repository
{
    public class ProductRepository: IProductRepository
    {
        private readonly ApplicationDbContext _context;
        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        //<bool methods

        public async Task<bool> DoesProductExistAsync(Guid productId)
        {
            return await _context.Products
                    .AsNoTracking()
                    .AnyAsync(p => p.Id == productId);
        }
        public async Task<bool> DoesProductCreatedByUserExistAsync(string userId, string productName)
        {
            return await _context.Products
                    .AsNoTracking()
                    .AnyAsync(p => p.CreatorId == userId && p.Name == productName);
        }

        public async Task<bool> DoesProductCreatedByUserExistAsync(string userId, Guid productId)
        {
            return await _context.Products
                    .AsNoTracking()
                    .AnyAsync(p => p.CreatorId == userId && p.Id == productId);
        }

        public async Task<bool> DoesProductHaveActiveSaleOrdersAsync(Guid productId)
        {
            return await _context.SellOrders
                .AsNoTracking()
                .AnyAsync(so => so.ProductId == productId && so.Status == GCommon.Enums.SellOrderStatus.active);
        }

        public async Task<bool> IsProductApprovedAsync(Guid productId)
        {
            return await _context.Products
                    .AsNoTracking()
                    .AnyAsync(p => p.Id == productId && p.Status == GCommon.Enums.ProductStatus.approved);
        }

        public async Task<bool> IsProductSuggestedToOrderRequestAsync(Guid productId, Guid orderRequestId)
        {
            return await _context
                .SellOrderSuggestions
                .AsNoTracking()
                .Where(sos => sos.ProductId == productId && sos.OrderRequestId == orderRequestId)
                .AnyAsync();
        }

        public async Task<bool> DoesProductHaveNonResolvedReports(Guid productId)
        {
            return await _context
                .ProductReports
                .AsNoTracking()
                .AnyAsync(pr => pr.ReportedProductId == productId && pr.Status != ProductReportStatus.resolved);
        }
        //bool methods>

        //<number methods
        public async Task<int> GetProductActiveSellOrdersCountAsync(Guid productId)
        {
            int sellOrdersCount = await _context.SellOrders
                .AsNoTracking()
                .Where(so => so.ProductId == productId && so.Status == SellOrderStatus.active)
                .CountAsync();
            return sellOrdersCount;
        }
        //number methods>


        //<entity methods
        public async Task<Product?> GetProductByIdAsync(Guid productId)
        {
            Product? product = await _context.Products.FindAsync(productId);
            return product;
        }

        //entity methods>

        //<dto methods
        public async Task<Product_CreateSellOrderEligibilityDto?> GetProductForCreateSellOrderAsync(Guid productId)
        {
            Product_CreateSellOrderEligibilityDto? product = await _context
                .Products
                .Include(p => p.SellOrders)
                .AsNoTracking()
                .Where(p => p.Id == productId)
                .Select(p => new Product_CreateSellOrderEligibilityDto
                {
                    Name = p.Name,
                    Status = p.Status,
                    CreatorId = p.CreatorId,
                    ActiveSellOrdersCount = p.SellOrders.Where(so => so.Status == SellOrderStatus.active).Count()
                }).SingleOrDefaultAsync();

            return product;
        }


        public async Task<Product_CancelSellOrderEligibilityDto?> GetProductForCancelSellOrderAsync(Guid productId)
        {
            Product_CancelSellOrderEligibilityDto? product = await _context
               .Products
               .Include(p => p.SellOrders)
               .AsNoTracking()
               .Where(p => p.Id == productId)
               .Select(p => new Product_CancelSellOrderEligibilityDto
               {
                   Name = p.Name,
                   Status = p.Status,
                   CreatorId = p.CreatorId,
                   ActiveSellOrdersCount = p.SellOrders.Where(so => so.Status == SellOrderStatus.active).Count()
               }).SingleOrDefaultAsync();

            return product;
        }

        public async Task<Product_BuySellOrderEligibilityDto?> GetProductForBuySellOrderAsync(Guid productId)
        {
            Product_BuySellOrderEligibilityDto? product = await _context
                .Products
                .Include(p => p.SellOrders)
                .AsNoTracking()
                .Where(p => p.Id == productId)
                .Select(p => new Product_BuySellOrderEligibilityDto
                {
                    Name = p.Name,
                    Price = p.Price,
                    Status = p.Status,
                    CreatorId = p.CreatorId,
                    ActiveSellOrdersCount = p.SellOrders.Where(so => so.Status == SellOrderStatus.active).Count()
                }).SingleOrDefaultAsync();

            return product;
        }
        //dto methods>

        //<operation methods
        public async Task CreateProductAsync(Product product)
        {
            await _context.Products.AddAsync(product);

            int affectedEntities = await _context.SaveChangesAsync();
            if (affectedEntities != 1)
            {
                throw new Exception("Failed to create product.");
            }
        }

        public async Task UpdateProductAsync(Product product, string newName, string newDescription, decimal newPrice)
        {
            _context.Attach<Product>(product);

            product.Name = newName;
            product.Description = newDescription;
            product.Price = newPrice;
            product.Status = ApplicationConstants.CreatedProductDefaultStatus;

            int affectedEntities = await _context.SaveChangesAsync();
            if (affectedEntities != 1)
            {
                throw new Exception("Failed to update product.");
            }
        }

        public async Task ChangeProductStatusAsync(Product product, ProductStatus newStatus)
        {
            _context.Attach<Product>(product);
            product.Status = newStatus;

            int affectedEntities = await _context.SaveChangesAsync();
            if (affectedEntities != 1)
            {
                throw new Exception("Failed to change the status of a product.");
            }
        }

        public async Task DeleteProductAsync(Product product)
        {
            int affectedEntitiesBeforeSaveChanges = 0;

            affectedEntitiesBeforeSaveChanges += await DeleteSellOrdersOfProductAsync(product.Id);
            affectedEntitiesBeforeSaveChanges += await DeleteSellOrderSuggestionsOfProductAsync(product.Id);
            affectedEntitiesBeforeSaveChanges += await DeleteReportsOfProductAsync(product.Id);
            affectedEntitiesBeforeSaveChanges += await SetFkToNullForCompletedOrdersOfProductAsync(product.Id);
                        
            _context.Products.Remove(product);
            affectedEntitiesBeforeSaveChanges++;

            int affectedEntities = await _context.SaveChangesAsync();
            if (affectedEntities != affectedEntitiesBeforeSaveChanges)
            {
                throw new Exception("Failed to delete product.");
            }
        }

        private async Task<int> DeleteSellOrdersOfProductAsync(Guid productId)
        {
            IEnumerable<SellOrder> sellOrders = await _context
                .SellOrders
                .Include(so => so.Product)
                .Where(so => so.Product.Id == productId)
                .ToListAsync();

            _context.SellOrders.RemoveRange(sellOrders);

            return sellOrders.Count();
        }

        private async Task<int> DeleteSellOrderSuggestionsOfProductAsync(Guid productId)
        {
            IEnumerable<SellOrderSuggestion> sellOrderSuggestions = await _context.
                SellOrderSuggestions
                .Where(sos => sos.ProductId == productId)
                .ToListAsync();

            _context.SellOrderSuggestions.RemoveRange(sellOrderSuggestions);

            return sellOrderSuggestions.Count();
        }

        private async Task<int> DeleteReportsOfProductAsync(Guid productId)
        {
            IEnumerable<ProductReport> productReports = await _context
                .ProductReports
                .Where(pr => pr.ReportedProductId == productId)
                .ToListAsync();

            _context.ProductReports.RemoveRange(productReports);

            return productReports.Count();
        }

        private async Task<int> SetFkToNullForCompletedOrdersOfProductAsync(Guid productId)
        {
            IEnumerable<CompletedOrder> completedOrders = await _context
                .CompletedOrders
                .Include(so => so.Product)
                .Where(so => so.Product.Id == productId)
                .ToListAsync();

            foreach (CompletedOrder completedOrder in completedOrders)
            {
                completedOrder.Product = null;
            }

            return completedOrders.Count();
        }
        //<operation methods>
    }
}
