
using TradingApp.Data.Dtos.Product;
using TradingApp.Data.Models;
using TradingApp.GCommon.Enums;

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
        Task<bool> DoesProductHaveNonResolvedReports(Guid productId);
        //bool methods>

        //<number methods
        Task<int> GetProductActiveSellOrdersCountAsync(Guid productId);
        //number methods>

        //<entity methods
        Task<Product?> GetProductByIdAsync(Guid productId);
        //entity methods>

        //<dto methods
        Task<Product_CreateSellOrderEligibilityDto?> GetProductForCreateSellOrderAsync(Guid productId);
        Task<Product_CancelSellOrderEligibilityDto?> GetProductForCancelSellOrderAsync(Guid productId);
        Task<Product_BuySellOrderEligibilityDto?> GetProductForBuySellOrderAsync(Guid productId);
        //dto methods>

        //<operation methods
        Task CreateProductAsync(Product product);
        Task UpdateProductAsync(Product product, string newName, string newDescription, decimal newPrice);        
        Task ChangeProductStatusAsync(Product product, ProductStatus newStatus);
        Task DeleteProductAsync(Product product);
        //operation methods>
    }
}
