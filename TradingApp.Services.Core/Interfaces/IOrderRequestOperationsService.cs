
using TradingApp.GCommon;

namespace TradingApp.Services.Core.Interfaces
{
    public interface IOrderRequestOperationsService
    {
        Task<Result> CreateSuggestionForOrderRequest(Guid productId, string userId, Guid requestId);
    }
}
