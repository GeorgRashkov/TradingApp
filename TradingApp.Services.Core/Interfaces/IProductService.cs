using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradingApp.ViewModels.Product;

namespace TradingApp.Services.Core.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductsViewModel>> GetApprovedProductsWithActiveSellOrdersAsync(int pageIndex);
        Task<ProductViewModel?> GetDetailsForApprovedProductWithActiveSellOrdersAsync(Guid productId);
        Task<IEnumerable<MyProductsViewModel>> GetProductsCreatedByUserAsync(int pageIndex, string userId);
        Task<MyProductViewModel?> GetDetailsForProductAsync(Guid productId);
        Task<int> GetUserActiveSellOrdersCountAsync(string userId);

        int ProductPageIndex { get; }
    }
}
