using Microsoft.EntityFrameworkCore;
using System.Globalization;
using TradingApp.Data;
using TradingApp.Data.Models;
using TradingApp.GCommon;
using TradingApp.GCommon.Enums;
using TradingApp.GCommon.Filters;
using TradingApp.Services.Core.Interfaces;
using TradingApp.ViewModels.InputProduct;
using TradingApp.ViewModels.Product;

namespace TradingApp.Services.Core
{
    public class ProductService : IProductService
    {
        private const int _productsPerPage = ApplicationConstants.ProductsPerPage;
        private ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        public int ProductPageIndex { get; private set; }


        private IQueryable<Product> GetQueryForFilteringProducts(ProductFilter productFilter, IQueryable<Product> productsQuery)
        {
            if (string.IsNullOrEmpty(productFilter.ProductName) == false)
            { productsQuery = productsQuery.Where(p => p.Name.Contains(productFilter.ProductName)); }

            if (string.IsNullOrEmpty(productFilter.CreatorName) == false)
            { productsQuery = productsQuery.Where(p => p.Creator.UserName.Contains(productFilter.CreatorName)); }

            if (productFilter.MinPrice > EntityValidation.Product.PriceMinValue)
            { productsQuery = productsQuery.Where(p => p.Price >= (decimal)productFilter.MinPrice); }

            if (productFilter.MaxPrice < EntityValidation.Product.PriceMaxValue)
            { productsQuery = productsQuery.Where(p => p.Price <= (decimal)productFilter.MaxPrice); }

            if(productFilter.OrderRequestId != default(Guid)) 
            { 
                productsQuery = productsQuery
                    .Include(p => p.SellOrderSuggestions)
                    .Where(p => p.SellOrderSuggestions.Any(sos => sos.OrderRequestId == productFilter.OrderRequestId)); 
            }

            return productsQuery;
        }

        public async Task<IEnumerable<ProductViewModel>> GetApprovedProductsWithActiveSellOrdersAsync(int pageIndex, ProductFilter? productFilter)
        {
            IQueryable<Product> productsQuery = _context
                .Products
                .Include(p => p.Creator)
               .AsNoTracking()
               .Where(p => p.Status == ProductStatus.approved && p.SellOrders.Any(so => so.Status == SellOrderStatus.active));

            if (productFilter is not null) 
            { productsQuery = GetQueryForFilteringProducts(productFilter, productsQuery); }
            int productsCount = await productsQuery.CountAsync();

            if (productsCount == 0)
            { return new List<ProductViewModel>(); }

            SetProductPage(pageIndex, productsCount);
                        
            IEnumerable<ProductViewModel> products = await productsQuery
            .Skip(ProductPageIndex * _productsPerPage).Take(_productsPerPage)
          .Select(p => new ProductViewModel
          {
              Id = p.Id,
              CreatorName = p.Creator.UserName,
              Price = p.Price.ToString("f2"),
              ProductName = p.Name
          }).ToListAsync();

            return products;
        }

        public async Task<IEnumerable<ProductViewModel>> GetProductsAsync(int pageIndex)
        {
            int productsCount = await _context
                .Products
               .AsNoTracking()
               .CountAsync();

            if (productsCount == 0)
            { return new List<ProductViewModel>(); }

            SetProductPage(pageIndex, productsCount);

            IEnumerable<ProductViewModel> products = await _context
                .Products
              .AsNoTracking()
              .Skip(ProductPageIndex * _productsPerPage).Take(_productsPerPage)
              .Select(p => new ProductViewModel
              {
                  Id = p.Id,
                  CreatorName = p.Creator.UserName,
                  Price = p.Price.ToString("f2"),
                  ProductName = p.Name,
                  Status = p.Status.ToString()
              }).ToListAsync();

            return products;
        }

        public async Task<Dictionary<string, string>> GetIdsAndNamesOfApprovedProductsWithActiveSaleOrdersCreatedByUserAsync(string userId)
        {
            Dictionary<string, string> productIdsAndNamesDict = await _context
                .Products
                .Include(p => p.SellOrders)
                .AsNoTracking()
                .Where(p => p.Status == ProductStatus.approved && p.SellOrders.Any(so => so.Status == SellOrderStatus.active) && p.CreatorId == userId)
                .Select(p => new { Id = p.Id.ToString(), Name = p.Name })
                .ToDictionaryAsync(p => p.Id, p => p.Name);

            return productIdsAndNamesDict;
        }

        public async Task<ProductDetailsViewModel?> GetDetailsForApprovedProductWithActiveSellOrdersAsync(Guid productId)
        {

            ProductDetailsViewModel? product = await _context
                .Products
                .Include(p => p.Creator)
                .AsNoTracking()
                .Where(p => p.Id == productId && p.Status == ProductStatus.approved && p.SellOrders.Any(so => so.Status == SellOrderStatus.active))
                .Select(p => new ProductDetailsViewModel
                {
                    Id = p.Id,
                    ProductName = p.Name,
                    Price = p.Price.ToString("f2"),
                    CreatorName = p.Creator.UserName,
                    Description = p.Description,
                    FirstSellOrderCreationDate = p.SellOrders.Where(so => so.Status == SellOrderStatus.active).Select(so => so.CreatedAt).OrderBy(createdAt => createdAt).SingleOrDefault().ToString(ApplicationConstants.DateFormat, CultureInfo.InvariantCulture),
                    LastSellOrderCreationDate = p.SellOrders.Where(so => so.Status == SellOrderStatus.active).Select(so => so.CreatedAt).OrderByDescending(createdAt => createdAt).SingleOrDefault().ToString(ApplicationConstants.DateFormat, CultureInfo.InvariantCulture),
                    SellOrdersCount = p.SellOrders.Where(so => so.Status == SellOrderStatus.active).Count()
                }).SingleOrDefaultAsync();

            return product;
        }



        public async Task<IEnumerable<MyProductViewModel>> GetProductsCreatedByUserAsync(int pageIndex, string userId)
        {

            int productsCount = await _context
                .Products
              .AsNoTracking()
              .Where(p => p.CreatorId == userId)
              .CountAsync();

            if (productsCount == 0)
            { return new List<MyProductViewModel>(); }

            SetProductPage(pageIndex, productsCount);

            IEnumerable<MyProductViewModel> products = await _context
                .Products
              .AsNoTracking()
              .Where(p => p.CreatorId == userId)
              .Skip(ProductPageIndex * _productsPerPage).Take(_productsPerPage)
              .Select(p => new MyProductViewModel
              {
                  Id = p.Id,
                  Price = p.Price.ToString("f2"),
                  ProductName = p.Name,
                  ProductStatus = p.Status.ToString(),
                  CreatorName = p.Creator.UserName
              }).ToListAsync();

            return products;
        }



        public async Task<MyProductDetailsViewModel?> GetDetailsForProductAsync(Guid productId)
        {
            MyProductDetailsViewModel? product = await _context
                .Products
                .AsNoTracking()
                .Where(p => p.Id == productId)
                .Select(p => new MyProductDetailsViewModel
                {
                    Id = p.Id,
                    ProductName = p.Name,
                    Description = p.Description,
                    CreatorName = p.Creator.UserName,
                    Price = p.Price.ToString("f2"),
                    ProductStatus = p.Status.ToString(),
                    ActiveSellOrdersCount = p.SellOrders.Where(so => so.Status == SellOrderStatus.active).Count()
                }).SingleOrDefaultAsync();

            return product;
        }

        private void SetProductPage(int pageIndex, int productsCount)
        {
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageIndex = pageIndex * _productsPerPage >= productsCount ? (int)Math.Ceiling((decimal)productsCount / (decimal)_productsPerPage) - 1 : pageIndex;
            ProductPageIndex = pageIndex;
        }

        public async Task<int> GetProductActiveSellOrdersCountAsync(Guid productId)
        {
            int sellOrdersCount = await _context.SellOrders
                .AsNoTracking()
                .Where(so => so.ProductId == productId && so.Status == SellOrderStatus.active)
                .CountAsync();
            return sellOrdersCount;
        }

        public async Task<UpdatedProductModel?> GetUpdatedProductModelAsync(Guid productId)
        {
            UpdatedProductModel? product = await _context
               .Products
               .AsNoTracking()
               .Where(p => p.Id == productId)
               .Select(p => new UpdatedProductModel()
               {
                   Id = productId,
                   ProductName = p.Name,
                   Description = p.Description,
                   Price = decimal.Parse(p.Price.ToString("f2"))
               }).SingleOrDefaultAsync();

            return product;
        }

        public async Task<DeletedProductModel?> GetDeletedProductModelAsync(Guid productId)
        {
            DeletedProductModel? product = await _context
               .Products
               .AsNoTracking()
               .Where(p => p.Id == productId)
               .Select(p => new DeletedProductModel()
               {
                   ProductId = productId,
                   ProductName = p.Name,
               }).SingleOrDefaultAsync();

            return product;
        }

        public async Task<ManagedProductModel?> GetManagedProductModelAsync(Guid productId)
        {
            ManagedProductModel? product = await _context
                .Products
                .AsNoTracking()
                .Where(p => p.Id == productId)
                .Select(p => new ManagedProductModel()
                {
                    Id = productId,
                    Name = p.Name,
                    Status = p.Status
                }).SingleOrDefaultAsync();

            return product;
        }


        public async Task<string?> GetProductNameAsync(Guid productId)
        {
            string? productName = await _context
                .Products
                .AsNoTracking()
                .Where(p => p.Id == productId)
                .Select(p => p.Name).SingleOrDefaultAsync();

            return productName;
        }
    }
}
