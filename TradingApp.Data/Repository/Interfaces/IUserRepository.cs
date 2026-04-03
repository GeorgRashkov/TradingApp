
using TradingApp.Data.Dtos.User;
using TradingApp.Data.Models;

namespace TradingApp.Data.Repository.Interfaces
{
    public interface IUserRepository
    {
        //<bool methods
        Task<bool> DoesUserExistAsync(string userId);
        Task<bool> DidUserBoughtProductAsync(Guid productId, string userId);
        Task<bool> DoesCreatorHaveOtherProductsWithTheSameNameAsync(string productName, Guid productId, string creatorId);
        //bool methods>

        //<number methods
        Task<int> GetUsersCountAsync();
        Task<int> GetUserActiveSellOrdersCountAsync(string userId);
        //number methods>

        //<text methods
        Task<string?> GetCreatorNameOfProductAsync(Guid productId);
        Task<string?> GetCreatorIdOfRequestAsync(Guid orderRequestId);        
        Task<string?> GetUserIdAsync(string userName);
        //text methods>

        //<entity methods
        Task<IEnumerable<User>> GetUsersAsync(int skipCount, int takeCount);
        //entity methods>

        //<dto methods
        Task<User_CreateSellOrderEligibilityDto?> GetUserForCreateSellOrderAsync(string userId);
        Task<User_CancelSellOrderEligibilityDto?> GetUserForCancelSellOrderAsync(string userId);
        Task<User_BuySellOrderEligibilityDto?> GetUserForBuySellOrderAsync(string userId);
        //dto methods>

        //<operations methods
        Task ManageUserAsync(User user, string? lockoutMessage, bool lockoutEnabled, DateTimeOffset lockoutEnd);
        //operations methods>
    }
}
