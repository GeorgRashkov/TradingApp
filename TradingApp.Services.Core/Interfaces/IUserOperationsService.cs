

using TradingApp.GCommon;
using TradingApp.ViewModels.InputUser;

namespace TradingApp.Services.Core.Interfaces
{
    public interface IUserOperationsService
    {
        Task<Result> ManageUserAsync(ManagedUserModel user);
    }
}
