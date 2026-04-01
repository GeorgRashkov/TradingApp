
using TradingApp.Data.Models;

namespace TradingApp.Data.Repository.Interfaces
{
    public interface IBalanceRepository
    {
        Task<Balance> GetBalanceAsync(string userId);
        Task CreateBalanceAsync(Balance balance);
        Task<decimal> GetBalanceAmountAsync(string userId);
        Task IncreaseBalanceAsync(string userId, decimal increasement);
        Task DecreaseBalanceAsync(string userId, decimal decreasement);
    }
}
