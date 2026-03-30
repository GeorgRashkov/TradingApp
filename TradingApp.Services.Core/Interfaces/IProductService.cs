
using TradingApp.ViewModels.InputProduct;
using TradingApp.ViewModels.Product;

namespace TradingApp.Services.Core.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductViewModel>> GetApprovedProductsWithActiveSellOrdersAsync(int pageIndex);
        Task<IEnumerable<ProductViewModel>> GetProductsAsync(int pageIndex);
        Task<IEnumerable<ProductViewModel>> Get_SuggestedApprovedProductsWithActiveSellOrders_for_OrderRequest_Async(int pageIndex, Guid orderRequestId);
        Task<Dictionary<string, string>> GetIdsAndNamesOfApprovedProductsWithActiveSaleOrdersCreatedByUserAsync(string userId);
        Task<ProductDetailsViewModel?> GetDetailsForApprovedProductWithActiveSellOrdersAsync(Guid productId);
        Task<IEnumerable<MyProductViewModel>> GetProductsCreatedByUserAsync(int pageIndex, string userId);
        Task<MyProductDetailsViewModel?> GetDetailsForProductAsync(Guid productId);
        Task<int> GetProductActiveSellOrdersCountAsync(Guid productId);
        Task<UpdatedProductModel?> GetUpdatedProductModelAsync(Guid productId);
        Task<ManagedProductModel?> GetManagedProductModelAsync(Guid productId);
        Task<DeletedProductModel?> GetDeletedProductModelAsync(Guid productId);

        Task<string?> GetProductNameAsync(Guid productId);        

        int ProductPageIndex { get; }
    }
}
