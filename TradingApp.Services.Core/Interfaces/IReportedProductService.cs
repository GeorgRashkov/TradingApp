
using TradingApp.ViewModels.ProductReport;

namespace TradingApp.Services.Core.Interfaces
{
    public interface IReportedProductService
    {
        Task<List<ProductsReportsViewModel>> GetReportsAsync(int pageIndex);
        Task<List<ProductsReportsViewModel>> GetReportsForProductAsync(int pageIndex, Guid reportedProductId);
        int ProductReportPageIndex { get; set; }
    }
}
