
using TradingApp.Data.Models;

namespace TradingApp.Services.Core.Interfaces
{
    public interface IProductBoolsService
    {
        Task<bool> DoesProductCreatedByUserExistAsync(string userId, string productName);
        Task<bool> DoesUserExistAsync(string userId);

        Task<bool> DoesProductCreatedByUserExistAsync(string userId, Guid productId);
    }
}
