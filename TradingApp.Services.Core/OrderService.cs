
using Microsoft.EntityFrameworkCore;
using TradingApp.Data;
using TradingApp.Data.Models;
using TradingApp.GCommon;
using TradingApp.GCommon.Enums;
using TradingApp.GCommon.ErrorCodes;
using TradingApp.Services.Core.Interfaces;

namespace TradingApp.Services.Core
{
    public class OrderService : IOrderService
    {
        private ApplicationDbContext _context;
     
        public OrderService(ApplicationDbContext context)
        {
            _context = context;            
        }

        public async Task<Result> CreateSellOrdersAsync(string creatorId, Guid productId, int ordersCount)
        {
            Result result = await CanUserCreateSellOrdersOfSpecificProductAsync(productId: productId, userId: creatorId, ordersCount: ordersCount);

            if(result.Success == false)
            {
                return result;
            }

            ordersCount = int.Parse(result.SuccessMessage!);
            List<SellOrder> sellOrders = new List<SellOrder>();
            DateTime sellOrderCreatedAt = DateTime.UtcNow;

            for (int i = 0; i < ordersCount; i++)
            {  
                SellOrder sellOrder = new SellOrder()
                {
                    Status = ApplicationConstants.CreatedSellOrderDefaultStatus,
                    CreatedAt = sellOrderCreatedAt,
                    CreatorId = creatorId,
                    ProductId = productId,
                };

                sellOrders.Add(sellOrder);                
            }

            await _context.SellOrders.AddRangeAsync(sellOrders);
            await _context.SaveChangesAsync();
            return result;
        }


        public async Task<Result> CancelSellOrdersAsync(string creatorId, Guid productId, int ordersCount)
        {
            Result result = await CanUserCancelSellOrdersOfSpecificProductAsync(productId: productId, userId: creatorId, ordersCount:ordersCount);

            if (result.Success == false)
            {
                return result;
            }

            ordersCount = int.Parse(result.SuccessMessage!);

            List<SellOrder> sellOrders = await _context
                .SellOrders
                .Where(so => so.ProductId == productId && so.Status == GCommon.Enums.SellOrderStatus.active)
                .OrderBy(so => so.CreatedAt)
                .ToListAsync();

           
            foreach (SellOrder sellOrder in sellOrders)
            {
                if (ordersCount < 1) { break; }
                sellOrder.Status = GCommon.Enums.SellOrderStatus.cancelled;
                ordersCount--;
            }

            await _context.SaveChangesAsync();

            return result;
        }


        public async Task<Result> BuySellOrderAsync(Guid productId, string buyerId)
        {

            Result result = await CanUserBuySellOrderOfSpecificProductAsync(productId: productId, userId: buyerId);

            if (result.Success == false)
            {
                return result;
            }
           
            SellOrder sellOrder = (await _context
                .SellOrders
                .Include(so => so.Product)
                .Where(so => so.ProductId == productId && so.Status == SellOrderStatus.active)
                .OrderBy(so => so.CreatedAt)
                .FirstOrDefaultAsync())!;

            OrderRequest? orderRequest = await _context
                .OrderRequests
                .Include(or => or.SellOrderSuggestions)
                .Where(or => or.CreatorId == buyerId && or.SellOrderSuggestions.Any(sos => sos.ProductId == productId))
                .OrderBy(or => or.CreatedAt)
                .FirstOrDefaultAsync();
           
            User buyer = (await _context
                .Users
                .Include(u => u.Balance)
                .Where(u => u.Id == buyerId)
                .SingleOrDefaultAsync())!;

            User seller = (await _context
                .Users
                .Include(u => u.Balance)
                .Where(u => u.Id == sellOrder.CreatorId)
                .SingleOrDefaultAsync())!;

            
            decimal productPrice = sellOrder.Product.Price;
            decimal platformFee = 0.1m * productPrice;
            

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
            if (orderRequest != null) 
            { orderRequest.Status = OrderRequestStatus.completed; }
            _context.CompletedOrders.Add(completedOrder);

            await _context.SaveChangesAsync();

            return result;
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private int FitOrdersCreationCountInBoundaries(int newOrdersCount, Guid productId, string userId, int productActiveSellOrdersCount, int userActiveSellOrdersCount)
        {
            if (newOrdersCount < 1)
            { newOrdersCount = 1; }

            //make sure the count of the created orders are not above the max number of total sale orders per product nor above the max number of total sell orders per user
            if (newOrdersCount + productActiveSellOrdersCount > ApplicationConstants.ProductMaxActiveSellOrders)
            { newOrdersCount = ApplicationConstants.ProductMaxActiveSellOrders - productActiveSellOrdersCount; }
            if (newOrdersCount + userActiveSellOrdersCount > ApplicationConstants.UserMaxActiveSellOrders)
            { newOrdersCount = ApplicationConstants.UserMaxActiveSellOrders - userActiveSellOrdersCount; }

            return newOrdersCount;
        }

        private int FitOrdersCancelationCountInBoundaries(int newOrdersCount, Guid productId, int productActiveSellOrdersCount)
        {
            newOrdersCount = Math.Max(1, newOrdersCount);
            newOrdersCount = Math.Min(newOrdersCount, productActiveSellOrdersCount);

            return newOrdersCount;
        }

        private async Task<bool> DidUserBoughtProductAsync(Guid productId, string userId)
        {
            return await _context
                .CompletedOrders
                .AsNoTracking()
                .AnyAsync(co => co.BuyerId == userId && co.ProductId == productId);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        public async Task<Result> CanUserCreateSellOrdersOfSpecificProductAsync(Guid productId, string userId, int ordersCount)
        {
            var product = await _context
                .Products
                .Include(p => p.SellOrders)
                .AsNoTracking()
                .Where(p => p.Id == productId)
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
                return new Result(errorCode: ProductErrorCodes.ProductNotFound);
                //return "The product was not found!";
            }

            if (user == null)
            {
                return new Result(errorCode: UserErrorCodes.UserNotFound);
                //return "You cannot create sell orders if you are not logged in!";
            }

            if (user.Id != product.CreatorId)
            {
                return new Result(errorCode: ProductErrorCodes.ProductInvalidCreator);
                //return $"You are not allowed to create sell orders of products created by other users!";
            }

            if (product.Status != ProductStatus.approved)
            {
                return new Result(errorCode: ProductErrorCodes.ProductInvalidStatus);
                //return $"You cannot create a sell order of the product {product.Name} because it's status is not 'approved'!";
            }

            if (product.ActiveSellOrdersCount >= ApplicationConstants.ProductMaxActiveSellOrders)
            {
                return new Result(errorCode: ProductErrorCodes.ProductMaxActiveSellOrdersReached);
                //return $"You cannot create sell orders for product {product.Name} because the product has reached the maximum number of active sale orders!";
            }

            if (user.ActiveSellOrdersCount >= ApplicationConstants.UserMaxActiveSellOrders)
            {
                return new Result(errorCode: UserErrorCodes.UserMaxActiveSellOrdersReached);
                //return $"You cannot create sell orders because you reached the maximum number of active sale orders.";
            }

            //the success message is the number of sell orders which the user is allowed to create (this will always be either the input value of the orders count or the maximum number of sell orders which the user can create for the product)
            int numberOfAllowedSellOrdersToCreate = FitOrdersCreationCountInBoundaries(newOrdersCount: ordersCount, productId: productId, userId: userId, productActiveSellOrdersCount: product.ActiveSellOrdersCount, userActiveSellOrdersCount: user.ActiveSellOrdersCount);
            return new Result(successMessage: numberOfAllowedSellOrdersToCreate.ToString());
            //return "";

        }



        
        public async Task<Result> CanUserCancelSellOrdersOfSpecificProductAsync(Guid productId, string userId, int ordersCount)
        {
            var product = await _context
                .Products
                .Include(p => p.SellOrders)
                .AsNoTracking()
                .Where(p => p.Id == productId)
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
                return new Result(errorCode: ProductErrorCodes.ProductNotFound);
                //return "The product was not found!";
            }

            if (user == null)
            {
                return new Result(errorCode: UserErrorCodes.UserNotFound);
                //return "You cannot cancel sell orders if you are not logged in!";
            }

            if (user.Id != product.CreatorId)
            {
                return new Result(errorCode: ProductErrorCodes.ProductInvalidCreator);
                //return $"You are not allowed to cancel sell orders of products created by other users!";
            }

            if (product.Status != ProductStatus.approved)
            {
                return new Result(errorCode: ProductErrorCodes.ProductInvalidStatus);
                //return $"You cannot create a sell order of the product {product.Name} because it's status is not 'approved'!";
            }

            if (product.ActiveSellOrdersCount < 1)
            {
                return new Result(errorCode: ProductErrorCodes.ProductHasNoActiveSaleOrders);
                //return $"The product {product.Name} has no active sell orders to cancel!";
            }

            // the success message is the number of sell orders which the user is allowed to cancel(this will always be either the input value of the orders count or all active sell orders of the product)
            int numberOfAllowedSellOrdersToCancel = FitOrdersCancelationCountInBoundaries(newOrdersCount: ordersCount, productId: productId, productActiveSellOrdersCount: product.ActiveSellOrdersCount);
            return new Result(successMessage: numberOfAllowedSellOrdersToCancel.ToString());
            //return "";

        }




        //this method returns an error message; if the output is '' than there are no errors (the user can create a sell order of the product)
        public async Task<Result> CanUserBuySellOrderOfSpecificProductAsync(Guid productId, string userId)
        {
            var product = await _context
                .Products
                .Include(p => p.SellOrders)
                .AsNoTracking()
                .Where(p => p.Id == productId)
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
                return new Result(errorCode: ProductErrorCodes.ProductNotFound);
                //return "The product was not found!";
            }

            if (user == null)
            {
                return new Result(errorCode: UserErrorCodes.UserNotFound);
                //return "You cannot buy products if you are not logged in!";
            }

            if (user.Id == product.CreatorId)
            {
                return new Result(errorCode: ProductErrorCodes.ProductInvalidCreator);
                //return $"You are not allowed to buy your own products!";
            }

            if (product.Status != ProductStatus.approved)
            {
                return new Result(errorCode: ProductErrorCodes.ProductInvalidStatus);
                //return $"You cannot buy the product {product.Name} because it's status is not 'approved'!";
            }


            if (product.ActiveSellOrdersCount < 1)
            {
                return new Result(errorCode: ProductErrorCodes.ProductHasNoActiveSaleOrders);
                //return $"You cannot buy the product {product.Name} because it has no active sell orders!";
            }

            if(user.Balance < product.Price)
            {
                return new Result(errorCode: UserErrorCodes.UserInsufficientBalance);
                //return $"You do not have enough money in your balance to buy the product {product.Name}!";
            }

            bool didUserBoughtProduct = await DidUserBoughtProductAsync(productId: productId, userId: userId);
            if(didUserBoughtProduct == true)
            {
                return new Result(errorCode: ProductErrorCodes.ProductAlreadyPurchased);
                //return $"You are not allowed to buy products you previosly purchased! You can find and dowload the product {product.Name} in the ivoices page.";
            }

            return new Result();
            //return "";

        }
                
    }
}
