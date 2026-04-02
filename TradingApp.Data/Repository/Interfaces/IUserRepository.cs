
using TradingApp.Data.Dtos.User;

namespace TradingApp.Data.Repository.Interfaces
{
    public interface IUserRepository
    {
        //<bool methods
        Task<bool> DoesUserExistAsync(string userId);
        Task<bool> DidUserBoughtProductAsync(Guid productId, string userId);
        //bool methods>

        //<number methods
        Task<int> GetUserActiveSellOrdersCountAsync(string userId);
        //number methods>

        //<text methods
        Task<string?> GetCreatorNameOfProductAsync(Guid productId);
        Task<string?> GetCreatorIdOfRequestAsync(Guid orderRequestId);        
        Task<string?> GetUserIdAsync(string userName);
        //text methods>

        //<dto methods
        Task<User_CreateSellOrderEligibilityDto?> GetUserForCreateSellOrderAsync(string userId);
        Task<User_CancelSellOrderEligibilityDto?> GetUserForCancelSellOrderAsync(string userId);
        Task<User_BuySellOrderEligibilityDto?> GetUserForBuySellOrderAsync(string userId);
        //dto methods>
    }
}
