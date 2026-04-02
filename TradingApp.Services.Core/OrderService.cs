
using Microsoft.EntityFrameworkCore;
using TradingApp.Data.Dtos.Product;
using TradingApp.Data.Dtos.User;
using TradingApp.Data.Models;
using TradingApp.Data.Repository.Interfaces;
using TradingApp.GCommon;
using TradingApp.GCommon.Enums;
using TradingApp.GCommon.ErrorCodes;
using TradingApp.Services.Core.Interfaces;

namespace TradingApp.Services.Core
{
    public class OrderService : IOrderService
    {
        private readonly ISellOrderRepository _sellOrderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;

        public OrderService(ISellOrderRepository sellOrderRepository, IProductRepository productRepository, IUserRepository userRepository)
        {
            _sellOrderRepository = sellOrderRepository;
            _productRepository = productRepository;
            _userRepository = userRepository;
        }

        public async Task<Result> CreateSellOrdersAsync(string creatorId, Guid productId, int ordersCount)
        {
            Result result = await CanUserCreateSellOrdersOfSpecificProductAsync(productId: productId, userId: creatorId, ordersCount: ordersCount);

            if (result.Success == false)
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

            await _sellOrderRepository.CreateSellOrdersAsync(sellOrders: sellOrders);

            return result;
        }


        public async Task<Result> CancelSellOrdersAsync(string creatorId, Guid productId, int ordersCount)
        {
            Result result = await CanUserCancelSellOrdersOfSpecificProductAsync(productId: productId, userId: creatorId, ordersCount: ordersCount);

            if (result.Success == false)
            {
                return result;
            }

            ordersCount = int.Parse(result.SuccessMessage!);

            IEnumerable<SellOrder> sellOrders = await _sellOrderRepository.GetActiveSellOrdersOfProductAsync(productId: productId, ordersCount: ordersCount);

            await _sellOrderRepository.CancelSellOrdersAsync(sellOrders: sellOrders);

            return result;
        }


        public async Task<Result> BuySellOrderAsync(Guid productId, string buyerId)
        {

            Result result = await CanUserBuySellOrderOfSpecificProductAsync(productId: productId, userId: buyerId);

            if (result.Success == false)
            {
                return result;
            }

            await _sellOrderRepository.BuySellOrderAsync(productId: productId, buyerId: buyerId);

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
       
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        public async Task<Result> CanUserCreateSellOrdersOfSpecificProductAsync(Guid productId, string userId, int ordersCount)
        {
            Product_CreateSellOrderEligibilityDto? product = await _productRepository.GetProductForCreateSellOrderAsync(productId: productId);

            User_CreateSellOrderEligibilityDto? user = await _userRepository.GetUserForCreateSellOrderAsync(userId: userId);

            if (product == null)
            {
                return new Result(errorCode: ProductErrorCodes.ProductNotFound);
            }

            if (user == null)
            {
                return new Result(errorCode: UserErrorCodes.UserNotFound);
            }

            if (user.UserId != product.CreatorId)
            {
                return new Result(errorCode: ProductErrorCodes.ProductInvalidCreator);
            }

            if (product.Status != ProductStatus.approved)
            {
                return new Result(errorCode: ProductErrorCodes.ProductInvalidStatus);
            }

            if (product.ActiveSellOrdersCount >= ApplicationConstants.ProductMaxActiveSellOrders)
            {
                return new Result(errorCode: ProductErrorCodes.ProductMaxActiveSellOrdersReached);
            }

            if (user.ActiveSellOrdersCount >= ApplicationConstants.UserMaxActiveSellOrders)
            {
                return new Result(errorCode: UserErrorCodes.UserMaxActiveSellOrdersReached);
            }

            //the success message is the number of sell orders which the user is allowed to create (this will always be either the input value of the orders count or the maximum number of sell orders which the user can create for the product)
            int numberOfAllowedSellOrdersToCreate = FitOrdersCreationCountInBoundaries(newOrdersCount: ordersCount, productId: productId, userId: userId, productActiveSellOrdersCount: product.ActiveSellOrdersCount, userActiveSellOrdersCount: user.ActiveSellOrdersCount);
            return new Result(successMessage: numberOfAllowedSellOrdersToCreate.ToString());
        }




        public async Task<Result> CanUserCancelSellOrdersOfSpecificProductAsync(Guid productId, string userId, int ordersCount)
        {
            Product_CancelSellOrderEligibilityDto? product = await _productRepository.GetProductForCancelSellOrderAsync(productId: productId);

            User_CancelSellOrderEligibilityDto? user = await _userRepository.GetUserForCancelSellOrderAsync(userId: userId);
            
            if (product == null)
            {
                return new Result(errorCode: ProductErrorCodes.ProductNotFound);
            }

            if (user == null)
            {
                return new Result(errorCode: UserErrorCodes.UserNotFound);
            }

            if (user.UserId != product.CreatorId)
            {
                return new Result(errorCode: ProductErrorCodes.ProductInvalidCreator);
            }

            if (product.Status != ProductStatus.approved)
            {
                return new Result(errorCode: ProductErrorCodes.ProductInvalidStatus);
            }

            if (product.ActiveSellOrdersCount < 1)
            {
                return new Result(errorCode: ProductErrorCodes.ProductHasNoActiveSaleOrders);
            }

            // the success message is the number of sell orders which the user is allowed to cancel(this will always be either the input value of the orders count or all active sell orders of the product)
            int numberOfAllowedSellOrdersToCancel = FitOrdersCancelationCountInBoundaries(newOrdersCount: ordersCount, productId: productId, productActiveSellOrdersCount: product.ActiveSellOrdersCount);
            return new Result(successMessage: numberOfAllowedSellOrdersToCancel.ToString());
        }





        public async Task<Result> CanUserBuySellOrderOfSpecificProductAsync(Guid productId, string userId)
        {

            Product_BuySellOrderEligibilityDto? product = await _productRepository.GetProductForBuySellOrderAsync(productId: productId);

            User_BuySellOrderEligibilityDto? user = await _userRepository.GetUserForBuySellOrderAsync(userId: userId);

            if (product == null)
            {
                return new Result(errorCode: ProductErrorCodes.ProductNotFound);
            }

            if (user == null)
            {
                return new Result(errorCode: UserErrorCodes.UserNotFound);
            }

            if (user.UserId == product.CreatorId)
            {
                return new Result(errorCode: ProductErrorCodes.ProductInvalidCreator);
            }

            if (product.Status != ProductStatus.approved)
            {
                return new Result(errorCode: ProductErrorCodes.ProductInvalidStatus);
            }


            if (product.ActiveSellOrdersCount < 1)
            {
                return new Result(errorCode: ProductErrorCodes.ProductHasNoActiveSaleOrders);
            }

            if (user.Balance < product.Price)
            {
                return new Result(errorCode: UserErrorCodes.UserInsufficientBalance);
            }

            bool didUserBoughtProduct = await _userRepository.DidUserBoughtProductAsync(productId: productId, userId: userId);
            if (didUserBoughtProduct == true)
            {
                return new Result(errorCode: ProductErrorCodes.ProductAlreadyPurchased);
            }

            return new Result();
        }

    }
}
