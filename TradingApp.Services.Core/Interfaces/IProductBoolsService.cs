
namespace TradingApp.Services.Core.Interfaces
{
    public interface IProductBoolsService
    {
        Task<bool> DoesProductCreatedByUserExistAsync(string userId, string productName);
        Task<bool> DoesProductExistAsync(Guid productId);

        Task<bool> DoesUserExistAsync(string userId);
        Task<bool> DoesProductCreatedByUserExistAsync(string userId, Guid productId);
        Task<bool> DoesProductHaveActiveSaleOrdersAsync(Guid productId);
        Task<bool> IsProductApprovedAsync(Guid productId);
        Task<bool> IsProductSuggestedToOrderRequestAsync(Guid productId, Guid orderRequestId);
    }
}
