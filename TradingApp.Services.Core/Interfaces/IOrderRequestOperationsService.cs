
using TradingApp.GCommon;

namespace TradingApp.Services.Core.Interfaces
{
    public interface IOrderRequestOperationsService
    {
        Task<Result> CreateSuggestionForOrderRequest(Guid productId, string suggesterId, Guid requestId);
        Task<Result> CreateOrderRequest(string title, string description, decimal maxPrice, string creatorId);

        Task<Result> UpdateOrderRequest(Guid id, string title, string description, decimal maxPrice, string creatorId);
    }
}
