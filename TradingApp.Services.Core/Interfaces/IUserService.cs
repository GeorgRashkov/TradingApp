
using TradingApp.ViewModels.InputUser;
using TradingApp.ViewModels.User;

namespace TradingApp.Services.Core.Interfaces
{
    public interface IUserService
    {        
        Task<string?> GetCreatorNameOfProductAsync(Guid productId);
        Task<string?> GetCreatorIdOfRequestAsync(Guid orderRequestId);
        Task<int> GetUserActiveSellOrdersCountAsync(string userId);
        Task<string?> GetUserIdAsync(string userName);
        Task<ManagedUserModel?> GetManagedUserAsync(string userId);
        Task<IEnumerable<UsersViewModel>> GetUsersAsync(int pageIndex);

        int UserPageIndex { get;}
    }
}
