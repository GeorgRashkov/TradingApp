

using TradingApp.Data.Models;
using TradingApp.Data.Repository.Interfaces;
using TradingApp.GCommon;
using TradingApp.GCommon.ErrorCodes;
using TradingApp.Services.Core.Interfaces;

namespace TradingApp.Services.Core
{
    public class OrderRequestOperationsService : IOrderRequestOperationsService
    {       

        private IProductRepository _productRepository;
        private IOrderRequestRepository _orderRequestRepository;        
        private IUserRepository _userRepository;
        public OrderRequestOperationsService(IProductRepository productRepository, IOrderRequestRepository orderRequestRepository, IUserRepository userRepository)
        {
            _productRepository = productRepository;
            _orderRequestRepository = orderRequestRepository;            
            _userRepository = userRepository;
        }

        public async Task<Result> CreateSuggestionForOrderRequest(Guid productId, string suggesterId, Guid requestId)
        {
            //<request validations
            bool doesOrderRequestExist = await _orderRequestRepository.DoesOrderRequestExistAsync(orderRequestId: requestId);
            if (doesOrderRequestExist == false)
            { return new Result(errorCode: OrderRequestErrorCodes.RequestNotFound); }

            string orderRequestCreatorId = (await _userRepository.GetCreatorIdOfRequestAsync(orderRequestId: requestId))!;
            if (suggesterId == orderRequestCreatorId)
            { return new Result(errorCode: OrderRequestErrorCodes.RequestSuggestionSameCreator);}

            bool isOrderRequestActive = await _orderRequestRepository.IsOrderRequestActiveAsync(orderRequestId: requestId);
            if (doesOrderRequestExist == false)
            { return new Result(errorCode: OrderRequestErrorCodes.RequestInvalidStatus); }
            //request validations>


            //<product validations
            bool doesProductExist = await _productRepository.DoesProductExistAsync(productId: productId);
            if (doesProductExist == false)
            { return new Result(errorCode: ProductErrorCodes.ProductNotFound); }

            bool isSuggesterCreatorOfTheProduct = await _productRepository.DoesProductCreatedByUserExistAsync(userId: suggesterId, productId: productId);
            if (isSuggesterCreatorOfTheProduct == false)
            { return new Result(errorCode: ProductErrorCodes.ProductInvalidCreator); }

            bool isProductApproved = await _productRepository.IsProductApprovedAsync(productId: productId);
            if (isProductApproved == false)
            { return new Result(errorCode: ProductErrorCodes.ProductInvalidStatus); }

            bool doesProductHaveActiveSaleOrders = await _productRepository.DoesProductHaveActiveSaleOrdersAsync(productId: productId);
            if (doesProductHaveActiveSaleOrders == false)
            { return new Result(errorCode: ProductErrorCodes.ProductHasNoActiveSaleOrders); }

            bool isProductAlreadySuggestedToOrderRequest = await _productRepository.IsProductSuggestedToOrderRequestAsync(productId: productId, orderRequestId: requestId);
            if (isProductAlreadySuggestedToOrderRequest == true)
            { return new Result(errorCode: ProductErrorCodes.ProductAlreadySuggestedToRequest); }
            //product validations>

           

            SellOrderSuggestion sellOrderSuggestion = new SellOrderSuggestion
            {
                ProductId = productId,
                OrderRequestId = requestId
            };

            await _orderRequestRepository.CreateSellOrderSuggestionAsync(sellOrderSuggestion: sellOrderSuggestion);
           
            return new Result();
        }

        public async Task<Result> CreateOrderRequest(string title, string description, decimal maxPrice, string creatorId)
        {
            bool doesUserExist = await _userRepository.DoesUserExistAsync(userId: creatorId);
            if (doesUserExist == false)
            { return new Result(errorCode: ProductErrorCodes.ProductNotFound); }

            bool doesOrderRequestCreatedByUserExist = await _orderRequestRepository.DoesOrderRequestCreatedByUserExistAsync(userId: creatorId, orderRequestTitle: title);
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

            await _orderRequestRepository.CreateOrderRequestAsync(orderRequest: orderRequest);
           
            return new Result();
        }

        public async Task<Result> UpdateOrderRequest(Guid id, string title, string description, decimal maxPrice, string creatorId)
        {
            string? orderRequestCreatorId = await _userRepository.GetCreatorIdOfRequestAsync(orderRequestId: id);
            if(orderRequestCreatorId == null) 
            { return new Result(errorCode: OrderRequestErrorCodes.RequestNotFound); }
            else if (orderRequestCreatorId != creatorId)
            { return new Result(errorCode: OrderRequestErrorCodes.RequestInvalidCreator); }

            bool doesOrderRequestCreatedByUserExist = await _orderRequestRepository.DoesOrderRequestCreatedByUserExistAsync(userId: creatorId, orderRequestTitle: title, orderRequestIdsToIgnore: new Guid[1] { id });
            if (doesOrderRequestCreatedByUserExist == true)
            { return new Result(errorCode: OrderRequestErrorCodes.RequestWithSameTitleAlreadyExists); }

            //update the order request
            OrderRequest orderRequest = (await _orderRequestRepository.GetRequestAsync(requestId: id))!;
            await _orderRequestRepository.UpdateOrderRequest(orderRequest: orderRequest, newTitle: title, newDescription: description, newMaxPrice:maxPrice);
            
            return new Result();
        }
        

        public async Task<Result> CancelOrderRequestAsync(Guid id, string userId)
        {
            OrderRequest? orderRequest = await _orderRequestRepository.GetRequestAsync(requestId: id);
            if (orderRequest == null)
            { return new Result(errorCode: OrderRequestErrorCodes.RequestNotFound); }
            else if (orderRequest.CreatorId != userId)
            { return new Result(errorCode: OrderRequestErrorCodes.RequestInvalidCreator); }
            else if (orderRequest.Status != GCommon.Enums.OrderRequestStatus.active)
            { return new Result(errorCode: OrderRequestErrorCodes.RequestInvalidStatus); }

            await _orderRequestRepository.UpdateOrderRequestStatusAsync(orderRequest: orderRequest, newStatus: GCommon.Enums.OrderRequestStatus.cancelled);

            return new Result();
        }
    }
}
