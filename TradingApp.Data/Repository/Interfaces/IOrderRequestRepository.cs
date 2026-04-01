
using TradingApp.Data.Dtos.OrderRequest;
using TradingApp.Data.Models;
using TradingApp.GCommon.Enums;

namespace TradingApp.Data.Repository.Interfaces
{
    public interface IOrderRequestRepository
    {
        //< Bool methods
        Task<bool> DoesOrderRequestExistAsync(Guid orderRequestId);
        Task<bool> DoesOrderRequestCreatedByUserExistAsync(string userId, string orderRequestTitle);
        Task<bool> DoesOrderRequestCreatedByUserExistAsync(string userId, string orderRequestTitle, Guid[] orderRequestIdsToIgnore);
        Task<bool> IsOrderRequestActiveAsync(Guid orderRequestId);
        //Bool methods >

        //<number methods
        Task<int> GetActiveRequestsCountAsync();
        Task<int> GetUserActiveRequestsCountAsync(string userId);
        //number methods>

        //< Dto methods
        Task<IEnumerable<OrderRequestDto>> GetActiveRequestsAsync(int skipCount, int takeCount);
        Task<IEnumerable<OrderRequestDto>> GetActiveRequestsCreatedByUserAsync(string userId, int skipCount, int takeCount);
        Task<OrderRequestDetailsDto?> GetDetailsForActiveRequestAsync(Guid requestId);
        Task<OrderRequestDetailsDto?> GetDetailsForActiveRequestCreatedByUserAsync(Guid requestId, string userId);
        //Dto methods>

        //<entity methods
        Task<OrderRequest?> GetRequestAsync(Guid requestId);
        //entity methods>

        //<operation methods
        Task CreateSellOrderSuggestionAsync(SellOrderSuggestion sellOrderSuggestion);
        Task CreateOrderRequestAsync(OrderRequest orderRequest);
        Task UpdateOrderRequest(OrderRequest orderRequest, string newTitle, string newDescription, decimal newMaxPrice);
        Task UpdateOrderRequestStatusAsync(OrderRequest orderRequest, OrderRequestStatus newStatus);
        //operation methods>
    }
}
