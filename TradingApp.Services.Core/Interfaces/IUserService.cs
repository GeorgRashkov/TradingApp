
namespace TradingApp.Services.Core.Interfaces
{
    public interface IUserService
    {
        Task<bool> DoesUserExistAsync(string userId);
        Task<string?> GetCreatorNameOfProductAsync(Guid productId);
        Task<string?> GetCreatorIdOfRequestAsync(Guid orderRequestId);
        Task<int> GetUserActiveSellOrdersCountAsync(string userId);
    }
}
