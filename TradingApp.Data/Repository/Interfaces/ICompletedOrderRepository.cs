
using TradingApp.Data.Dtos.CompletedOrder;
using TradingApp.Data.Models;

namespace TradingApp.Data.Repository.Interfaces
{
    public interface ICompletedOrderRepository
    {
        Task<int> GetCompletedOrdersCountAsync(string userId);
        Task<IEnumerable<CompletedOrderDto>> GetCompletedOrdersAsync(string userId, int skipCount, int takeCount);
        Task<CompletedOrder?> GetCompletedOrderAsync(Guid completedOrderId);
    }
}
