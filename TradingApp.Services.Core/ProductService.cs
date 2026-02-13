using Microsoft.EntityFrameworkCore;
using System.Globalization;
using TradingApp.Data;
using TradingApp.GCommon;
using TradingApp.GCommon.Enums;
using TradingApp.Services.Core.Interfaces;
using TradingApp.ViewModels.Product;

namespace TradingApp.Services.Core
{
    public class ProductService: IProductService
    {
        private const int _productsPerPage = ApplicationConstants.ProductsPerPage;                
        private ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        public int ProductPageIndex { get; private set; }


        public async Task<IEnumerable<ProductsViewModel>> GetApprovedProductsWithActiveSellOrdersAsync(int pageIndex)
        {   
            int productsCount = await _context
                .Products
               .AsNoTracking()
               .Where(p => p.Status == ProductStatus.approved && p.SellOrders.Any(so => so.Status == SellOrderStatus.active))
               .CountAsync();

            if (productsCount == 0)
            { return new List<ProductsViewModel>(); }

            SetProductPage(pageIndex, productsCount);

            IEnumerable<ProductsViewModel> products = await _context
                .Products
              .AsNoTracking()
              .Where(p => p.Status == ProductStatus.approved && p.SellOrders.Any(so => so.Status == SellOrderStatus.active))
              .Skip(ProductPageIndex * _productsPerPage).Take(_productsPerPage)
              .Select(p => new ProductsViewModel
              {
                  Id = p.Id,
                  CreatorName = p.Creator.UserName,
                  Price = p.Price.ToString("f2"),
                  ProductName = p.Name
              }).ToListAsync();                     

            return products;
        }



        public async Task<ProductViewModel?> GetDetailsForApprovedProductWithActiveSellOrdersAsync(Guid productId)
        {

            ProductViewModel? product = await _context
                .Products
                .Include(p => p.Creator)
                .AsNoTracking()
                .Where(p => p.Id == productId && p.Status == ProductStatus.approved && p.SellOrders.Any(so => so.Status == SellOrderStatus.active))
                .Select(p => new ProductViewModel
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



        public async Task<IEnumerable<MyProductsViewModel>> GetProductsCreatedByUserAsync(int pageIndex, string userId)
        {

            int productsCount = await _context
                .Products
              .AsNoTracking()
              .Where(p => p.CreatorId == userId)
              .CountAsync();

            if (productsCount == 0)
            { return new List<MyProductsViewModel>(); }

            SetProductPage(pageIndex, productsCount);

            IEnumerable<MyProductsViewModel> products = await _context
                .Products
              .AsNoTracking()
              .Where(p => p.CreatorId == userId)
              .Skip(ProductPageIndex * _productsPerPage).Take(_productsPerPage)
              .Select(p => new MyProductsViewModel
              {
                  Id = p.Id,
                  Price = p.Price.ToString("f2"),
                  ProductName = p.Name,
                  ProductStatus = p.Status.ToString(),
                  CreatorName = p.Creator.UserName
              }).ToListAsync();

            return products;
        }



        public async Task<MyProductViewModel?> GetDetailsForProductAsync(Guid productId)
        {
            MyProductViewModel? product = await _context
                .Products
                .AsNoTracking()
                .Where(p => p.Id == productId)
                .Select(p => new MyProductViewModel
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


        public async Task<int> GetUserActiveSellOrdersCountAsync(string userId)
        {
            int sellOrdersCount = await _context
                .SellOrders
                 .AsNoTracking()
                 .Where(so => so.CreatorId == userId && so.Status == SellOrderStatus.active)
                 .CountAsync();
            return sellOrdersCount;
        }

        private void SetProductPage(int pageIndex, int productsCount)
        {
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageIndex = pageIndex * _productsPerPage >= productsCount ? (int)Math.Ceiling((decimal)productsCount / (decimal)_productsPerPage) - 1 : pageIndex;
            ProductPageIndex = pageIndex;
        }
    }
}
