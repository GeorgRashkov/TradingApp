
using TradingApp.ViewModels.InputProduct;
using TradingApp.ViewModels.Product;

namespace TradingApp.Services.Core.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductsViewModel>> GetApprovedProductsWithActiveSellOrdersAsync(int pageIndex);
        Task<IEnumerable<ProductsViewModel>> Get_SuggestedApprovedProductsWithActiveSellOrders_for_OrderRequest_Async(int pageIndex, Guid orderRequestId);
        Task<Dictionary<string, string>> GetIdsAndNamesOfApprovedProductsWithActiveSaleOrdersCreatedByUserAsync(string userId);
        Task<ProductViewModel?> GetDetailsForApprovedProductWithActiveSellOrdersAsync(Guid productId);
        Task<IEnumerable<MyProductsViewModel>> GetProductsCreatedByUserAsync(int pageIndex, string userId);
        Task<MyProductViewModel?> GetDetailsForProductAsync(Guid productId);
        Task<int> GetUserActiveSellOrdersCountAsync(string userId);

        Task<int> GetProductActiveSellOrdersCountAsync(Guid productId);

        Task<UpdatedProductModel?> GetUpdatedProductModelAsync(Guid productId);
        Task<DeletedProductModel?> GetDeletedProductModelAsync(Guid productId);

        Task<string?> GetProductNameAsync(Guid productId);
        Task<string?> GetCreatorNameOfProductAsync(Guid productId);

        int ProductPageIndex { get; }
    }
}
