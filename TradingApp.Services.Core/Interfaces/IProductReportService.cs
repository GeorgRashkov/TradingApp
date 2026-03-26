
using TradingApp.ViewModels.ProductReport;

namespace TradingApp.Services.Core.Interfaces
{
    public interface IProductReportService
    {
        int ProductReportPageIndex { get; set; }
        Task<List<ProductsReportsViewModel>> GetReportsAsync(int pageIndex);
        Task<List<ProductsReportsViewModel>> GetReportsForProductAsync(int pageIndex, Guid reportedProductId);
        Task<ProductReportViewModel?> GetProductReportAsync(Guid reportId);
    }
}
