

using Microsoft.EntityFrameworkCore;
using TradingApp.Data;
using TradingApp.Data.Models;
using TradingApp.GCommon;
using TradingApp.GCommon.Enums;
using TradingApp.Services.Core.Interfaces;

namespace TradingApp.Services.Core
{
    public class OrderService : IOrderService
    {
        private ApplicationDbContext _context;
        private IProductService _productService;
        public OrderService(ApplicationDbContext context, IProductService productService)
        {
            _context = context;
            _productService = productService;
        }

        public async Task CreateSellOrders(string creatorId, Guid productId, int ordersCount)
        {
            for (int i = 0; i < ordersCount; i++)
            {
                DateTime sellOrderCreatedAt = DateTime.UtcNow;

                SellOrder sellOrder = new SellOrder()
                {
                    Status = ApplicationConstants.CreatedSellOrderDefaultStatus,
                    CreatedAt = sellOrderCreatedAt,
                    CreatorId = creatorId,
                    ProductId = productId,
                };

                await _context.SellOrders.AddAsync(sellOrder);
            }

            await _context.SaveChangesAsync();
        }


        public async Task CancelSellOrdersAsync(Guid productId, int ordersCount)
        {
            List<SellOrder> sellOrders = await _context
                .SellOrders
                .Where(so => so.ProductId == productId && so.Status == GCommon.Enums.SellOrderStatus.active)
                .OrderBy(so => so.CreatedAt)
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


        public async Task<bool> DidUserBoughtProductAsync(Guid productId, string userId)
        {
            return await _context
                .CompletedOrders
                .AsNoTracking()
                .AnyAsync(co => co.BuyerId == userId && co.ProductId == productId);
        }



















        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



        public async Task<int> FitOrdersCreationCountInBoundariesAsync(int ordersCount, Guid productId, string userId)
        {
            int productActiveSellOrdersCount = await _productService.GetProductActiveSellOrdersCountAsync(productId: productId);
            int userActiveSellOrdersCount = await _productService.GetUserActiveSellOrdersCountAsync(userId: userId);

            if (ordersCount < 1)
            { ordersCount = 1; }

            //make sure the count of the created orders are not above the max number of total sale orders per product nor above the max number of total sell orders per user
            if (ordersCount + productActiveSellOrdersCount > ApplicationConstants.ProductMaxActiveSellOrders)
            { ordersCount = ApplicationConstants.ProductMaxActiveSellOrders - productActiveSellOrdersCount; }
            if (ordersCount + userActiveSellOrdersCount > ApplicationConstants.UserMaxActiveSellOrders)
            { ordersCount = ApplicationConstants.UserMaxActiveSellOrders - userActiveSellOrdersCount; }

            return ordersCount;
        }

        public async Task<int> FitOrdersCancelationCountInBoundariesAsync(int ordersCount, Guid productId)
        {
            int productActiveSellOrdersCount = await _productService.GetProductActiveSellOrdersCountAsync(productId: productId);

            ordersCount = Math.Max(1, ordersCount);
            ordersCount = Math.Min(ordersCount, productActiveSellOrdersCount);

            return ordersCount;
        }



        //this method returns an error message; if the output is '' than there are no errors (the user can create a sell order of the product)
        public async Task<string> CanUserCreateSellOrderOfSpecificProductAsync(Guid productId, string userId)
        {
            var product = await _context
                .Products
                .Include(p => p.SellOrders)
                .AsNoTracking()
                .Where(p => p.Id == productId && p.Status == ProductStatus.approved)
                .Select(p => new
                {
                    Name = p.Name,
                    Status = p.Status,
                    CreatorId = p.CreatorId,
                    ActiveSellOrdersCount = p.SellOrders.Where(so => so.Status == SellOrderStatus.active).Count()
                }).SingleOrDefaultAsync();

            var user = await _context
                .Users
                .Include(u => u.SellOrders)
                .AsNoTracking()
                .Where(u => u.Id == userId)
                .Select(u => new
                {
                    Id = u.Id,
                    ActiveSellOrdersCount = u.SellOrders.Where(so => so.Status == SellOrderStatus.active).Count()
                }).SingleOrDefaultAsync();


            if (product == null)
            {
                return "The product was not found!";
            }

            if (user == null)
            {
                return "You cannot create sell orders if you are not logged in!";
            }

            if (user.Id != product.CreatorId)
            {
                return $"You are not allowed to create sell orders of products created by other users!";
            }

            if (product.Status != ProductStatus.approved)
            {
                return $"You cannot create a sell order of the product {product.Name} because it's status is not 'approved'!";
            }

            if (product.ActiveSellOrdersCount >= ApplicationConstants.ProductMaxActiveSellOrders)
            {
                return $"You cannot create sell orders for product {product.Name} because the product has reached the maximum number of active sale orders!";
            }

            if (user.ActiveSellOrdersCount >= ApplicationConstants.UserMaxActiveSellOrders)
            {
                return $"You cannot create sell orders because you reached the maximum number of active sale orders.";
            }

            return "";

        }



        //this method returns an error message; if the output is '' than there are no errors (the user can create a sell order of the product)
        public async Task<string> CanUserCancelSellOrderOfSpecificProductAsync(Guid productId, string userId)
        {
            var product = await _context
                .Products
                .Include(p => p.SellOrders)
                .AsNoTracking()
                .Where(p => p.Id == productId && p.Status == ProductStatus.approved)
                .Select(p => new
                {
                    Name = p.Name,
                    Status = p.Status,
                    CreatorId = p.CreatorId,
                    ActiveSellOrdersCount = p.SellOrders.Where(so => so.Status == SellOrderStatus.active).Count()
                }).SingleOrDefaultAsync();

            var user = await _context
                .Users
                .Include(u => u.SellOrders)
                .AsNoTracking()
                .Where(u => u.Id == userId)
                .Select(u => new
                {
                    Id = u.Id,
                    ActiveSellOrdersCount = u.SellOrders.Where(so => so.Status == SellOrderStatus.active).Count()
                }).SingleOrDefaultAsync();


            if (product == null)
            {
                return "The product was not found!";
            }

            if (user == null)
            {
                return "You cannot cancel sell orders if you are not logged in!";
            }

            if (user.Id != product.CreatorId)
            {
                return $"You are not allowed to cancel sell orders of products created by other users!";
            }

            if (product.Status != ProductStatus.approved)
            {
                return $"You cannot create a sell order of the product {product.Name} because it's status is not 'approved'!";
            }

            if (product.ActiveSellOrdersCount < 1)
            {
                return $"The product {product.Name} has no active sell orders to cancel!";
            }

            return "";

        }




        //this method returns an error message; if the output is '' than there are no errors (the user can create a sell order of the product)
        public async Task<string> CanUserBuySellOrderOfSpecificProductAsync(Guid productId, string userId)
        {
            var product = await _context
                .Products
                .Include(p => p.SellOrders)
                .AsNoTracking()
                .Where(p => p.Id == productId && p.Status == ProductStatus.approved)
                .Select(p => new
                {
                    Name = p.Name,
                    Price = p.Price,
                    Status = p.Status,
                    CreatorId = p.CreatorId,
                    ActiveSellOrdersCount = p.SellOrders.Where(so => so.Status == SellOrderStatus.active).Count()
                }).SingleOrDefaultAsync();

            var user = await _context
                .Users
                .Include(u => u.Balance)
                .AsNoTracking()
                .Where(u => u.Id == userId)
                .Select(u => new
                {
                    Id = u.Id,
                    Balance = u.Balance.Amount
                }).SingleOrDefaultAsync();


            if (product == null)
            {
                return "The product was not found!";
            }

            if (user == null)
            {
                return "You cannot buy products if you are not logged in!";
            }

            if (user.Id == product.CreatorId)
            {
                return $"You are not allowed to buy your own products!";
            }

            if (product.Status != ProductStatus.approved)
            {
                return $"You cannot buy the product {product.Name} because it's status is not 'approved'!";
            }


            if (product.ActiveSellOrdersCount < 1)
            {
               return $"You cannot buy the product {product.Name} because it has no active sell orders!";
            }

            if(user.Balance < product.Price)
            {
                return $"You do not have enough money in your balance to buy the product {product.Name}!";
            }

            bool didUserBoughtProduct = await DidUserBoughtProductAsync(productId: productId, userId: userId);
            if(didUserBoughtProduct == true)
            {
                return $"You are not allowed to buy products you previosly purchased! You can find and dowload the product {product.Name} in the ivoices page.";
            }

            return "";

        }













        public async Task BuySellOrderAsync(Guid productId, string buyerId)
        {
            SellOrder? sellOrder = await _context
                .SellOrders
                .Include(so => so.Product)
                .Where(so => so.ProductId == productId && so.Status == SellOrderStatus.active)
                .OrderBy(so => so.CreatedAt)
                .FirstOrDefaultAsync();

            if (sellOrder == null)
            {
                throw new InvalidOperationException("Cannot buy a non existing sell order!");
            }

            User? buyer = await _context
                .Users
                .Include(u => u.Balance)
                .Where(u => u.Id == buyerId)
                .SingleOrDefaultAsync();

            User? seller = await _context
                .Users
                .Include(u => u.Balance)
                .Where(u => u.Id == sellOrder.CreatorId)
                .SingleOrDefaultAsync();


            if (buyer == null || seller == null)
            {
                throw new InvalidOperationException("A non existing user or user without a balance is not allowed to buy or sell orders!");
            }

            if (buyer.Id == seller.Id)
            {
                throw new InvalidOperationException("Users are not allowed to buy their own sell orders!");
            }

            decimal productPrice = sellOrder.Product.Price;
            decimal platformFee = 0.1m * productPrice;

            if (buyer.Balance.Amount < productPrice)
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
                BuyerId = buyer.Id,
                SellerId = seller.Id,
            };

            buyer.Balance.Amount -= completedOrder.PricePaid;
            seller.Balance.Amount += completedOrder.SellerRevenue;
            sellOrder.Status = SellOrderStatus.completed;
            _context.CompletedOrders.Add(completedOrder);

            await _context.SaveChangesAsync();

        }
    }
}
