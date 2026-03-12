

using Azure.Core;
using TradingApp.Data;
using TradingApp.Data.Models;
using TradingApp.GCommon;
using TradingApp.GCommon.ErrorCodes;
using TradingApp.Services.Core.Interfaces;

namespace TradingApp.Services.Core
{
    public class OrderRequestOperationsService : IOrderRequestOperationsService
    {
        private ApplicationDbContext _context;
        private IProductBoolsService _productBoolsService;
        private IOrderRequestBoolsService _orderRequestBoolsService;
        private IUserService _userService;

        public OrderRequestOperationsService(ApplicationDbContext context, IProductBoolsService productBoolsService, IOrderRequestBoolsService orderRequestBoolsService, IUserService userService)
        {
            _context = context;
            _productBoolsService = productBoolsService;
            _orderRequestBoolsService = orderRequestBoolsService;
            _userService = userService;
        }

        public async Task<Result> CreateSuggestionForOrderRequest(Guid productId, string suggesterId, Guid requestId)
        {
            //<request validations
            bool doesOrderRequestExist = await _orderRequestBoolsService.DoesOrderRequestExistAsync(orderRequestId: requestId);
            if (doesOrderRequestExist == false)
            { return new Result(errorCode: OrderRequestErrorCodes.RequestNotFound); }

            string orderRequestCreatorId = (await _userService.GetCreatorIdOfRequestAsync(orderRequestId: requestId))!;
            if (suggesterId == orderRequestCreatorId)
            { return new Result(errorCode: OrderRequestErrorCodes.RequestSuggestionSameCreator);}

            bool isOrderRequestActive = await _orderRequestBoolsService.IsOrderRequestActiveAsync(orderRequestId: requestId);
            if (doesOrderRequestExist == false)
            { return new Result(errorCode: OrderRequestErrorCodes.RequestInvalidStatus); }
            //request validations>


            //<product validations
            bool doesProductExist = await _productBoolsService.DoesProductExistAsync(productId: productId);
            if (doesProductExist == false)
            { return new Result(errorCode: ProductErrorCodes.ProductNotFound); }

            bool isSuggesterCreatorOfTheProduct = await _productBoolsService.DoesProductCreatedByUserExistAsync(userId: suggesterId, productId: productId);
            if (isSuggesterCreatorOfTheProduct == false)
            { return new Result(errorCode: ProductErrorCodes.ProductInvalidCreator); }

            bool isProductApproved = await _productBoolsService.IsProductApprovedAsync(productId: productId);
            if (isProductApproved == false)
            { return new Result(errorCode: ProductErrorCodes.ProductInvalidStatus); }

            bool doesProductHaveActiveSaleOrders = await _productBoolsService.DoesProductHaveActiveSaleOrdersAsync(productId: productId);
            if (doesProductHaveActiveSaleOrders == false)
            { return new Result(errorCode: ProductErrorCodes.ProductHasNoActiveSaleOrders); }

            bool isProductAlreadySuggestedToOrderRequest = await _productBoolsService.IsProductSuggestedToOrderRequestAsync(productId: productId, orderRequestId: requestId);
            if (isProductAlreadySuggestedToOrderRequest == true)
            { return new Result(errorCode: ProductErrorCodes.ProductAlreadySuggestedToRequest); }
            //product validations>

           

            SellOrderSuggestion sellOrderSuggestion = new SellOrderSuggestion
            {
                ProductId = productId,
                OrderRequestId = requestId
            };

            await _context.SellOrderSuggestions.AddAsync(sellOrderSuggestion);
            await _context.SaveChangesAsync();

            return new Result();
        }

        public async Task<Result> CreateOrderRequest(string title, string description, decimal maxPrice, string creatorId)
        {
            bool doesUserExist = await _userService.DoesUserExistAsync(userId: creatorId);
            if (doesUserExist == false)
            { return new Result(errorCode: ProductErrorCodes.ProductNotFound); }

            bool doesOrderRequestCreatedByUserExist = await _orderRequestBoolsService.DoesOrderRequestCreatedByUserExistAsync(userId: creatorId, orderRequestTitle: title);
            if(doesOrderRequestCreatedByUserExist == true) 
            { return new Result(errorCode: OrderRequestErrorCodes.RequestWithSameTitleAlreadyExists); }

            OrderRequest orderRequest = new OrderRequest
            {
                Title = title,
                Description = description,
                MaxPrice = maxPrice,
                CreatedAt = DateTime.UtcNow,
                Status = ApplicationConstants.CreatedOrderRequestDefaultStatus,
                CreatorId = creatorId
            };

            await _context.OrderRequests.AddAsync(orderRequest);
            await _context.SaveChangesAsync();

            return new Result();
        }

        public async Task<Result> UpdateOrderRequest(Guid id, string title, string description, decimal maxPrice, string creatorId)
        {
            string? orderRequestCreatorId = await _userService.GetCreatorIdOfRequestAsync(orderRequestId: id);
            if(orderRequestCreatorId == null) 
            { return new Result(errorCode: OrderRequestErrorCodes.RequestNotFound); }
            else if (orderRequestCreatorId != creatorId)
            { return new Result(errorCode: OrderRequestErrorCodes.RequestInvalidCreator); }

            bool doesOrderRequestCreatedByUserExist = await _orderRequestBoolsService.DoesOrderRequestCreatedByUserExistAsync(userId: creatorId, orderRequestTitle: title, orderRequestIdsToIgnore: new Guid[1] { id });
            if (doesOrderRequestCreatedByUserExist == true)
            { return new Result(errorCode: OrderRequestErrorCodes.RequestWithSameTitleAlreadyExists); }

            //update the order request
            OrderRequest orderRequest = (await _context.OrderRequests.FindAsync(id))!;
            orderRequest.Title = title;
            orderRequest.Description = description;
            orderRequest.MaxPrice = maxPrice;

            //apply the changes to the database
            await _context.SaveChangesAsync();
            return new Result();
        }
    }
}
