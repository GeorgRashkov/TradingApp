

using TradingApp.GCommon;
using TradingApp.GCommon.ErrorCodes;
using TradingApp.Data;
using TradingApp.Services.Core.Interfaces;
using TradingApp.Data.Models;

namespace TradingApp.Services.Core
{
    public class OrderRequestOperationsService: IOrderRequestOperationsService
    {
        private ApplicationDbContext _context;
        private IProductBoolsService _productBoolsService;
        private IOrderRequestBoolsService _orderRequestBoolsService;

        public OrderRequestOperationsService(ApplicationDbContext context, IProductBoolsService productBoolsService, IOrderRequestBoolsService orderRequestBoolsService)
        {
            _context = context;
            _productBoolsService = productBoolsService;
            _orderRequestBoolsService = orderRequestBoolsService;
        }

        public async Task<Result> CreateSuggestionForOrderRequest(Guid productId, string userId, Guid requestId)
        {
            //<product validations
            bool doesProductExist = await _productBoolsService.DoesProductExistAsync(productId: productId);
            if (doesProductExist == false)
            { return new Result(errorCode: ProductErrorCodes.ProductNotFound); }

            bool isUserCreatorOfTheProduct = await _productBoolsService.DoesProductCreatedByUserExistAsync(userId: userId, productId: productId);
            if (isUserCreatorOfTheProduct == false)
            { return new Result(errorCode: ProductErrorCodes.ProductInvalidCreator); }

            bool isProductApproved = await _productBoolsService.IsProductApprovedAsync(productId: productId);
            if (isProductApproved == false)
            { return new Result(errorCode: ProductErrorCodes.ProductInvalidStatus); }

            bool doesProductHaveActiveSaleOrders = await _productBoolsService.DoesProductHaveActiveSaleOrdersAsync(productId: productId);
            if (doesProductHaveActiveSaleOrders == false)
            { return new Result(errorCode: ProductErrorCodes.ProductHasNoActiveSaleOrders); }
            
            bool isProductAlreadySuggestedToOrderRequest = await _productBoolsService.IsProductSuggestedToOrderRequestAsync(productId: productId, orderRequestId: requestId);
            if(isProductAlreadySuggestedToOrderRequest == true) 
            { return new Result(errorCode: ProductErrorCodes.ProductAlreadySuggestedToRequest); }
            //product validations>

            //<request validations
            bool doesOrderRequestExist = await _orderRequestBoolsService.DoesOrderRequestExistAsync(orderRequestId: requestId);
            if (doesOrderRequestExist == false)
            { return new Result(errorCode: OrderRequestErrorCodes.RequestNotFound); }

            bool isOrderRequestActive = await _orderRequestBoolsService.IsOrderRequestActiveAsync(orderRequestId: requestId);
            if (doesOrderRequestExist == false)
            { return new Result(errorCode: OrderRequestErrorCodes.RequestInvalidStatus); }
            //request validations>


            SellOrderSuggestion sellOrderSuggestion = new SellOrderSuggestion
            {
                ProductId = productId,
                OrderRequestId = requestId
            };

            await _context.SellOrderSuggestions.AddAsync(sellOrderSuggestion);
            await _context.SaveChangesAsync();

            return new Result();
        }
    }
}
