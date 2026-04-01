
namespace TradingApp.Data.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> DoesUserExistAsync(string userId);
        Task<string?> GetCreatorNameOfProductAsync(Guid productId);
        Task<string?> GetCreatorIdOfRequestAsync(Guid orderRequestId);
        Task<int> GetUserActiveSellOrdersCountAsync(string userId);
        Task<string?> GetUserIdAsync(string userName);
    }
}
