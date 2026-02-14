
namespace TradingApp.Services.Core.Interfaces
{
    public interface IBalanceService
    {
        Task<decimal> GetUserBalanceAsync(string userId);
        Task IncreaseUserBalanceAsync(string userId, decimal increasement);
        Task DecreaseUserBalanceAsync(string userId, decimal decreasement);
    }
}
