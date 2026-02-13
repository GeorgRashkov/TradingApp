using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TradingApp.Data.Helpers;
using TradingApp.Data;
using TradingApp.Data.Models;

namespace TradingApp.Data.Helpers
{
    public class CrudDb
    {
        private ApplicationDbContext _context;

        public CrudDb(ApplicationDbContext context)
        {
            _context = context;
        }

        

        //this method filters the products based on the parameter values in the filter
        private IQueryable<Product> FilterProducts(ProductFilter productFilter)
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

        //get's the sell orders count of all products created by the user with the specified id
        public async Task<int> GetSellOrdersCountAsync(string userId)
        {
            int sellOrdersCount = await _context.SellOrders
                .AsNoTracking()
                .Where(so => so.CreatorId == userId)
                .CountAsync();
            return sellOrdersCount;
        }

        //get's the sell orders count of one specific product created by the user with the specified id
        public async Task<int> GetSellOrdersCountAsync(string userId, Guid productId)
        {
            int sellOrdersCount = await _context.SellOrders
                .AsNoTracking()
                .Where(so => so.ProductId == productId && so.CreatorId == userId)
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


        public async Task<bool> DoesProductCreatedByCreatorExistAsync(string userId, Guid productId)
        {
            return await _context.Products
                    .AsNoTracking()
                    .AnyAsync(p => p.CreatorId == userId && p.Id == productId);
        }



        //saves the product in the database
        public async Task SaveProductAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }


        public async Task UpdateProductAsync(Product dtoProduct)
        {
            Product? dbProduct = await _context.Products
                .FindAsync(dtoProduct.Id);
            
            if(dbProduct == null)
            {
                throw new InvalidOperationException("Product could not be updated in the DB because it's ID was not in the DB!");
            }

            if(dbProduct.CreatorId != dtoProduct.CreatorId)
            {
                throw new UnauthorizedAccessException("The product cannot be updated by anyone except it's creator.");
            }

            //update the product
            dbProduct.Name = dtoProduct.Name;
            dbProduct.Description = dtoProduct.Description;
            dbProduct.Price = dtoProduct.Price;            
            dbProduct.Status = dtoProduct.Status;

            //apply change to DB
            await _context.SaveChangesAsync();
        }



        public async Task DeleteProductAsync(Guid productId)
        {
            Product product = (await _context.Products.FindAsync(productId))!;

            if(product == null)
            {
                throw new InvalidOperationException("Product could not be deleted from the DB because it's ID was not in the DB!");
            }

            _context.Products.Attach(product);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }




        public async Task CreateSellOrders(SellOrder sellOrder, int ordersCount)
        {
            for (int i = 0; i < ordersCount; i++)
            {
                sellOrder.CreatedAt = DateTime.UtcNow;
                await _context.SellOrders.AddAsync(sellOrder);
            }

            await _context.SaveChangesAsync();
        }

        public async Task CancelSellOrdersAsync(Guid productId, int ordersCount)
        {
            List<SellOrder> sellOrders = await _context
                .SellOrders
                .Where(so => so.ProductId == productId && so.Status == GCommon.Enums.SellOrderStatus.active)
                .ToListAsync();

            if (sellOrders.Count == 0)
            {
                throw new InvalidOperationException("Product has no active sale orders to cancel!");
            }

            foreach (SellOrder sellOrder in sellOrders)
            {
                if (ordersCount < 1) { break; }
                sellOrder.Status = GCommon.Enums.SellOrderStatus.cancelled;
                ordersCount--;
            }

            await _context.SaveChangesAsync();
        }


        public async Task<decimal> GetUserBalanceAsync(string userId)
        {
            var balance = await _context
                .Users
                .AsNoTracking()
                .Where( u => u.Id == userId)
                .Select(u => new { Amount = u.Balance.Amount })
                .FirstOrDefaultAsync();

            if (balance == null) 
            {
                throw new Exception("Balance not found!");
            }

            return balance.Amount;
        }

        public async Task BuySellOrderAsync(Guid productId, string buyerId)
        {
            SellOrder? sellOrder = await _context
                .SellOrders               
                .Include(so => so.Product)
                .Where(so => so.ProductId == productId && so.Status == GCommon.Enums.SellOrderStatus.active)
                .OrderBy(so => so.CreatedAt)                
                .FirstOrDefaultAsync();

            if(sellOrder == null)
            {
                throw new InvalidOperationException("Cannot buy a non existing sell order!");
            }


            Balance? buyerBalance = await _context
                .Balances
                .Include(b => b.User)
                .Where(b => b.User.Id == buyerId)
                .FirstOrDefaultAsync();          

            Balance? sellerBalance = await _context
                .Balances
                .Include(b => b.User)
                .Where(b => b.User.Id == sellOrder.CreatorId)
                .FirstOrDefaultAsync();


            if (buyerBalance == null || sellerBalance == null)
            {
                throw new InvalidOperationException("A non existing user or user without a balance is not allowed to buy or sell orders!");
            }

            if (sellerBalance.User.Id == buyerBalance.User.Id) 
            {
                throw new InvalidOperationException("Users are not allowed to buy their own sell orders!");
            }

            decimal productPrice = sellOrder.Product.Price;
            decimal platformFee = 0.1m * productPrice;

            if (buyerBalance.Amount < productPrice)
            {
                throw new InvalidOperationException("Users are not allowed to buy sell orders when the sell order price is above the user balance!");
            }
                        

            CompletedOrder completedOrder = new CompletedOrder()
            {
                TitleForBuyer = $"Product {sellOrder.Product.Name} purchased successfully",
                TitleForSeller = $"Product {sellOrder.Product.Name}  sold successfully",
                PricePaid = productPrice,
                PlatformFee = platformFee,
                SellerRevenue = productPrice - platformFee,
                CompletedAt = DateTime.UtcNow,

                ProductId = sellOrder.Product.Id,
                BuyerId = buyerBalance.User.Id,
                SellerId = sellerBalance.User.Id,
            };

            buyerBalance.Amount -= completedOrder.PricePaid;
            sellerBalance.Amount += completedOrder.SellerRevenue;
            sellOrder.Status = GCommon.Enums.SellOrderStatus.completed;
            _context.CompletedOrders.Add(completedOrder);

            await _context.SaveChangesAsync();

        }




        public async Task<List<COVM>> GetCompletedOrdersAsync<COVM>(string userId, Expression<Func<CompletedOrder, COVM>> buyerSelector, Expression<Func<CompletedOrder, COVM>> sellerSelector)
        {
            List<COVM> buyerCompletedOrders = await _context
                .CompletedOrders
                .AsNoTracking()
                .Where(co => co.BuyerId == userId)
                .Select(buyerSelector)
                .ToListAsync();

            List<COVM> sellerCompletedOrders = await _context
                .CompletedOrders
                .AsNoTracking()
                .Where(co => co.SellerId == userId)
                .Select(sellerSelector)
                .ToListAsync();

            List<COVM> userCompletedOrders = [.. buyerCompletedOrders, .. sellerCompletedOrders];

            return userCompletedOrders;
        }

        public async Task<CompletedOrder?> GetCompletedOrderAsync(Guid orderId)
        {
            CompletedOrder? completedOrder = await _context
                .CompletedOrders
                .AsNoTracking()
                .Include(co => co.Seller)
                .Include(co => co.Buyer)
                .Include(co => co.Product)                
                .Where(co => co.Id == orderId)
                .FirstOrDefaultAsync();

            return completedOrder;
        }

        public async Task<User?> GetUserAsync(string userId)
        {
            User? user = await _context.Users.FindAsync(userId);
            return user;
        }
    }
}
