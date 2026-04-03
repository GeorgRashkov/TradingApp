
using Microsoft.EntityFrameworkCore;
using TradingApp.Data.Dtos.Product;
using TradingApp.Data.Models;
using TradingApp.Data.Repository.Interfaces;
using TradingApp.GCommon;
using TradingApp.GCommon.Enums;
using TradingApp.GCommon.Filters;

namespace TradingApp.Data.Repository
{
    public class ProductRepository : IProductRepository
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
        public async Task<int> GetProductsCountAsync()
        {
            int productsCount = await _context
                .Products
               .AsNoTracking()
               .CountAsync();

            return productsCount;
        }

        public async Task<int> GetProductsCountCreatedByUserAsync(string userId)
        {
            int productsCount = await _context
                .Products
              .AsNoTracking()
              .Where(p => p.CreatorId == userId)
              .CountAsync();

            return productsCount;
        }

        public async Task<int> GetProductActiveSellOrdersCountAsync(Guid productId)
        {
            int sellOrdersCount = await _context.SellOrders
                .AsNoTracking()
                .Where(so => so.ProductId == productId && so.Status == SellOrderStatus.active)
                .CountAsync();
            return sellOrdersCount;
        }

        public async Task<int> GetCountOf_ApprovedProductsWithActiveSellOrdersAsync(ProductFilter? productFilter)
        {
            IQueryable<Product> productsQuery = _context
                .Products
               .AsNoTracking()
               .Where(p => p.Status == ProductStatus.approved && p.SellOrders.Any(so => so.Status == SellOrderStatus.active));

            if (productFilter is not null)
            { productsQuery = GetQueryForFilteringProducts(productFilter, productsQuery); }
            int productsCount = await productsQuery.CountAsync();

            return productsCount;
        }
        //number methods>

        //<text methods
        public async Task<string?> GetProductNameAsync(Guid productId)
        {
            string? productName = await _context
                .Products
                .AsNoTracking()
                .Where(p => p.Id == productId)
                .Select(p => p.Name).SingleOrDefaultAsync();

            return productName;
        }
        //text methods>

        //<entity methods
        public async Task<Product?> GetProductByIdAsync(Guid productId)
        {
            Product? product = await _context.Products.FindAsync(productId);
            return product;
        }

        //entity methods>

        //<dto methods
        public async Task<IEnumerable<ProductDto>> GetDtosOf_ProductsAsync(int skipCount, int takeCount)
        {
            IEnumerable<ProductDto> products = await _context
                .Products
                .Include(p => p.Creator)
              .AsNoTracking()
              .Skip(skipCount).Take(takeCount)
              .Select(p => new ProductDto
              {
                  Id = p.Id,
                  CreatorName = p.Creator.UserName,
                  Price = p.Price,
                  ProductName = p.Name,
                  Status = p.Status
              }).ToListAsync();

            return products;
        }

        public async Task<ProductDetailsDto?> GetProductDetailsDtoAsync(Guid productId)
        {
            ProductDetailsDto? product = await _context
               .Products
               .AsNoTracking()
               .Where(p => p.Id == productId)
               .Select(p => new ProductDetailsDto
               {
                   Id = p.Id,
                   ProductName = p.Name,
                   Description = p.Description,
                   CreatorName = p.Creator.UserName,
                   Price = p.Price,
                   Status = p.Status,
                   ActiveSellOrdersCount = p.SellOrders.Where(so => so.Status == SellOrderStatus.active).Count()
               }).SingleOrDefaultAsync();

            return product;
        }


        public async Task<ProductDetailsDto?> GetProductDetailsDtoOf_ApprovedProductWithActiveSellOrdersAsync(Guid productId)
        {
            ProductDetailsDto? product = await _context
                .Products
                .Include(p => p.Creator)
                .AsNoTracking()
                .Where(p => p.Id == productId && p.Status == ProductStatus.approved && p.SellOrders.Any(so => so.Status == SellOrderStatus.active))
                .Select(p => new ProductDetailsDto
                {
                    Id = p.Id,
                    ProductName = p.Name,
                    Price = p.Price,
                    CreatorName = p.Creator.UserName,
                    Description = p.Description,
                    FirstSellOrderCreationDate = p.SellOrders.Where(so => so.Status == SellOrderStatus.active).Select(so => so.CreatedAt).OrderBy(createdAt => createdAt).SingleOrDefault(),
                    LastSellOrderCreationDate = p.SellOrders.Where(so => so.Status == SellOrderStatus.active).Select(so => so.CreatedAt).OrderByDescending(createdAt => createdAt).SingleOrDefault(),
                    ActiveSellOrdersCount = p.SellOrders.Where(so => so.Status == SellOrderStatus.active).Count()
                }).SingleOrDefaultAsync();

            return product;
        }

        public async Task<IEnumerable<ProductDto>> GetDtosOf_ProductsCreatedByUserAsync(string userId, int skipCount, int takeCount)
        {
            IEnumerable<ProductDto> products = await _context
               .Products
               .Include(p => p.Creator)
             .AsNoTracking()
             .Where(p => p.CreatorId == userId)
             .Skip(skipCount).Take(takeCount)
             .Select(p => new ProductDto
             {
                 Id = p.Id,
                 Price = p.Price,
                 ProductName = p.Name,
                 Status = p.Status,
                 CreatorName = p.Creator.UserName
             }).ToListAsync();

            return products;
        }

        public async Task<IEnumerable<ProductListItemDto>> GetProductListItemDtosOf_ApprovedProductsWithActiveSaleOrdersCreatedByUserAsync(string userId)
        {
            IEnumerable<ProductListItemDto> productListItemDtos = await _context
                .Products
                .Include(p => p.SellOrders)
                .AsNoTracking()
                .Where(p => p.Status == ProductStatus.approved && p.SellOrders.Any(so => so.Status == SellOrderStatus.active) && p.CreatorId == userId)
                .Select(p => new ProductListItemDto { Id = p.Id, ProductName = p.Name })
                .ToListAsync();

            return productListItemDtos;
        }

        public async Task<IEnumerable<ProductDto>> GetDtosOf_ApprovedProductsWithActiveSellOrdersAsync(ProductFilter? productFilter, int skipCount, int takeCount)
        {
            IQueryable<Product> productsQuery = _context
                .Products
               .AsNoTracking()
               .Where(p => p.Status == ProductStatus.approved && p.SellOrders.Any(so => so.Status == SellOrderStatus.active));

            if (productFilter is not null)
            { productsQuery = GetQueryForFilteringProducts(productFilter, productsQuery); }

            IEnumerable<ProductDto> products = await productsQuery
            .Include(p => p.Creator)
            .Skip(skipCount).Take(takeCount)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                ProductName = p.Name,
                Price = p.Price,
                CreatorName = p.Creator.UserName,
                Status = p.Status

            }).ToListAsync();

            return products;
        }

        public async Task<Product_UpdateProductDto?> GetProductToUpdateAsync(Guid productId)
        {
            Product_UpdateProductDto? product = await _context
              .Products
              .AsNoTracking()
              .Where(p => p.Id == productId)
              .Select(p => new Product_UpdateProductDto()
              {
                  Id = productId,
                  Name = p.Name,
                  Description = p.Description,
                  Price = p.Price
              }).SingleOrDefaultAsync();

            return product;
        }

        public async Task<Product_DeleteProductDto?> GetProductToDeleteAsync(Guid productId)
        {
            Product_DeleteProductDto? product = await _context
               .Products
               .AsNoTracking()
               .Where(p => p.Id == productId)
               .Select(p => new Product_DeleteProductDto()
               {
                   Id = productId,
                   Name = p.Name,
               }).SingleOrDefaultAsync();

            return product;
        }
        public async Task<Product_ManageProductDto?> GetProductToManageAsync(Guid productId)
        {
            Product_ManageProductDto? product = await _context
               .Products
               .AsNoTracking()
               .Where(p => p.Id == productId)
               .Select(p => new Product_ManageProductDto()
               {
                   Id = productId,
                   Name = p.Name,
                   Status = p.Status
               }).SingleOrDefaultAsync();

            return product;
        }

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

            await _context.SaveChangesAsync();
        }

        public async Task ChangeProductStatusAsync(Product product, ProductStatus newStatus)
        {
            _context.Attach<Product>(product);
            product.Status = newStatus;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(Product product)
        {
            await DeleteSellOrdersOfProductAsync(product.Id);
            await DeleteSellOrderSuggestionsOfProductAsync(product.Id);
            await DeleteReportsOfProductAsync(product.Id);
            await SetFkToNullForCompletedOrdersOfProductAsync(product.Id);

            _context.Products.Remove(product);

            await _context.SaveChangesAsync();

        }

        private async Task DeleteSellOrdersOfProductAsync(Guid productId)
        {
            IEnumerable<SellOrder> sellOrders = await _context
                .SellOrders
                .Include(so => so.Product)
                .Where(so => so.Product.Id == productId)
                .ToListAsync();

            _context.SellOrders.RemoveRange(sellOrders);
        }

        private async Task DeleteSellOrderSuggestionsOfProductAsync(Guid productId)
        {
            IEnumerable<SellOrderSuggestion> sellOrderSuggestions = await _context.
                SellOrderSuggestions
                .Where(sos => sos.ProductId == productId)
                .ToListAsync();

            _context.SellOrderSuggestions.RemoveRange(sellOrderSuggestions);
        }

        private async Task DeleteReportsOfProductAsync(Guid productId)
        {
            IEnumerable<ProductReport> productReports = await _context
                .ProductReports
                .Where(pr => pr.ReportedProductId == productId)
                .ToListAsync();

            _context.ProductReports.RemoveRange(productReports);
        }

        private async Task SetFkToNullForCompletedOrdersOfProductAsync(Guid productId)
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
        }
        //<operation methods>


        //<filtering methods
        private IQueryable<Product> GetQueryForFilteringProducts(ProductFilter productFilter, IQueryable<Product> productsQuery)
        {
            if (string.IsNullOrEmpty(productFilter.ProductName) == false)
            { productsQuery = productsQuery.Where(p => p.Name.Contains(productFilter.ProductName)); }

            if (string.IsNullOrEmpty(productFilter.CreatorName) == false)
            {
                productsQuery = productsQuery
                    .Include(p => p.Creator)
                    .Where(p => p.Creator.UserName.Contains(productFilter.CreatorName));
            }

            if (productFilter.MinPrice > EntityValidation.Product.PriceMinValue)
            { productsQuery = productsQuery.Where(p => p.Price >= (decimal)productFilter.MinPrice); }

            if (productFilter.MaxPrice < EntityValidation.Product.PriceMaxValue)
            { productsQuery = productsQuery.Where(p => p.Price <= (decimal)productFilter.MaxPrice); }

            if (productFilter.OrderRequestId != default(Guid))
            {
                productsQuery = productsQuery
                    .Include(p => p.SellOrderSuggestions)
                    .Where(p => p.SellOrderSuggestions.Any(sos => sos.OrderRequestId == productFilter.OrderRequestId));
            }

            return productsQuery;
        }
        //filtering methods>
    }
}
