using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TradingApp.Common;
using TradingApp.Data;
using TradingApp.Data.Models;

namespace TradingApp.Services
{
    public class CrudDb
    {
        private ApplicationDbContext _context;

        public CrudDb(ApplicationDbContext context)
        {
            _context = context;
        }

        

        //this method filters the products based on the parameter values in the filter
        public IQueryable<Product> FilterProducts(ProductFilter productFilter)
        {
            IQueryable<Product> query = _context.Products
               .AsNoTracking();

            if (productFilter.PorductId != null)
            {
                query = query.Where(p => p.Id == productFilter.PorductId);
            }

            if (productFilter.UserId != null)
            {
                query = query.Where(p => p.CreatorId == productFilter.UserId);
            }

            if (productFilter.ProductStatus != null)
            {
                query = query.Where(p => p.Status == productFilter.ProductStatus);
            }

            if (productFilter.SellOrderStatus != null)
            {
                query = query.Where(p => p.SellOrders.Any(so => so.Status == productFilter.SellOrderStatus));//must check if this line works correctly !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            }


            if (productFilter.Username != null)
            {
                if (productFilter.UsernameContains == true)
                { query = query.Where(p => p.Creator.UserName.Contains(productFilter.Username)); }
                else
                { query = query.Where(p => p.Creator.UserName == productFilter.Username); }
            }

            if (productFilter.ProductName != null)
            {
                if (productFilter.ProductNameContains == true)
                { query = query.Where(p => p.Name.Contains(productFilter.ProductName)); }
                else
                { query = query.Where(p => p.Name == productFilter.ProductName); }
            }

            if (productFilter.Skip != null)
            {
                query = query.Skip((int)productFilter.Skip);
            }

            if (productFilter.Take != null)
            {
                query = query.Take((int)productFilter.Take);
            }

            return query;
        }

        //get's the products based on the filter and projects them to the specified view model using the selector expression
        public async Task<List<PVM>> GetProductsAsync<PVM>(ProductFilter productFilter, Expression<Func<Product, PVM>> selector)
        {
            List<PVM> productViewModels = await FilterProducts(productFilter)
                .Select(selector)
                .ToListAsync();

            return productViewModels;
        }

        //get's the product based on the filter and projects it to the specified view model using the selector expression
        public async Task<PVM?> GetProductAsync<PVM>(ProductFilter productFilter, Expression<Func<Product, PVM>> selector)
        {
            PVM? productViewModel = await FilterProducts(productFilter)
                .Select(selector)
                .FirstOrDefaultAsync();

            return productViewModel;
        }

        //get's the product based on the filter
        public async Task<int> GetProductsCountAsync(ProductFilter productFilter)
        {
            int productsCount = await FilterProducts(productFilter).CountAsync();
            return productsCount;
        }

        //get's the sell orders count of the products created by the user with the specified id
        public async Task<int> GetSellOrdersCountAsync(string userId)
        {
            int sellOrdersCount = await _context.SellOrders
                .AsNoTracking()
                .Where(so => so.CreatorId == userId)
                .CountAsync();
            return sellOrdersCount;
        }











        public async Task<bool> DoesCreatorExistAsync(string userId)
        {
            return await _context.Users
                    .AsNoTracking()
                    .AnyAsync(u => u.Id == userId);
        }

        public async Task<bool> DoesProductCreatedByCreatorExistAsync(string userId, string productName)
        {
            return await _context.Products
                    .AsNoTracking()
                    .AnyAsync(p => p.CreatorId == userId && p.Name == productName);
        }



        //saves the product in the database
        public async Task SaveProductAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

    }
}
