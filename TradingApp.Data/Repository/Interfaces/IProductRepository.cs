
using TradingApp.Data.Dtos.Product;
using TradingApp.Data.Models;
using TradingApp.GCommon.Enums;
using TradingApp.GCommon.Filters;

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
        Task<int> GetProductsCountAsync();
        Task<int> GetProductsCountCreatedByUserAsync(string userId);
        Task<int> GetProductActiveSellOrdersCountAsync(Guid productId);
        Task<int> GetCountOf_ApprovedProductsWithActiveSellOrdersAsync(ProductFilter? productFilter);
        //number methods>

        //<text methods
        Task<string?> GetProductNameAsync(Guid productId);
        //text methods>

        //<entity methods
        Task<Product?> GetProductByIdAsync(Guid productId);
        //entity methods>

        //<dto methods
        Task<IEnumerable<ProductDto>> GetDtosOf_ProductsAsync(int skipCount, int takeCount);
        Task<ProductDetailsDto?> GetProductDetailsDtoAsync(Guid productId);
        Task<ProductDetailsDto?> GetProductDetailsDtoOf_ApprovedProductWithActiveSellOrdersAsync(Guid productId);
        Task<IEnumerable<ProductDto>> GetDtosOf_ProductsCreatedByUserAsync(string userId, int skipCount, int takeCount);
        Task<IEnumerable<ProductListItemDto>> GetProductListItemDtosOf_ApprovedProductsWithActiveSaleOrdersCreatedByUserAsync(string userId);
        Task<IEnumerable<ProductDto>> GetDtosOf_ApprovedProductsWithActiveSellOrdersAsync(ProductFilter? productFilter, int skipCount, int takeCount);
        Task<Product_UpdateProductDto?> GetProductToUpdateAsync(Guid productId);
        Task<Product_DeleteProductDto?> GetProductToDeleteAsync(Guid productId);
        Task<Product_ManageProductDto?> GetProductToManageAsync(Guid productId);
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
