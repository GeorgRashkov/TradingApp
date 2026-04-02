
using TradingApp.Data.Models;

namespace TradingApp.Data.Repository.Interfaces
{
    public interface ISellOrderRepository
    {       
        //<entity methods
        Task<IEnumerable<SellOrder>> GetActiveSellOrdersOfProductAsync(Guid productId, int ordersCount);
        //entity methods>


        //<operation methods
        Task CreateSellOrdersAsync(IEnumerable<SellOrder> sellOrders);
        Task CancelSellOrdersAsync(IEnumerable<SellOrder> sellOrders);
        Task BuySellOrderAsync(Guid productId, string buyerId);
        //operation methods>
    }
}
