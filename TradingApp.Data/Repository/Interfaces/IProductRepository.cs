
namespace TradingApp.Data.Repository.Interfaces
{
    public interface IProductRepository
    {
        //<bool methods
        Task<bool> DoesProductCreatedByUserExistAsync(string userId, string productName);
        Task<bool> DoesProductExistAsync(Guid productId);

        Task<bool> DoesProductCreatedByUserExistAsync(string userId, Guid productId);
        Task<bool> DoesProductHaveActiveSaleOrdersAsync(Guid productId);
        Task<bool> IsProductApprovedAsync(Guid productId);
        Task<bool> IsProductSuggestedToOrderRequestAsync(Guid productId, Guid orderRequestId);
        //bool methods>
    }
}
