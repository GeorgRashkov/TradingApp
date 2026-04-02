
using TradingApp.Data.Dtos.Product;

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

        //<dto methods
        Task<Product_CreateSellOrderEligibilityDto?> GetProductForCreateSellOrderAsync(Guid productId);
        Task<Product_CancelSellOrderEligibilityDto?> GetProductForCancelSellOrderAsync(Guid productId);
        Task<Product_BuySellOrderEligibilityDto?> GetProductForBuySellOrderAsync(Guid productId);
        //dto methods>
    }
}
