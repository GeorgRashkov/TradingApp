
using TradingApp.GCommon;

namespace TradingApp.Services.Core.Interfaces
{
    public interface IOrderService
    {
        Task<Result> CreateSellOrdersAsync(string creatorId, Guid productId, int ordersCount);
        Task<Result> CancelSellOrdersAsync(string creatorId, Guid productId, int ordersCount);
        Task<Result> BuySellOrderAsync(Guid productId, string buyerId);

        Task<Result> CanUserCreateSellOrdersOfSpecificProductAsync(Guid productId, string userId, int ordersCount);
        Task<Result> CanUserCancelSellOrdersOfSpecificProductAsync(Guid productId, string userId, int ordersCount);
        Task<Result> CanUserBuySellOrderOfSpecificProductAsync(Guid productId, string userId);
        
    }
}
