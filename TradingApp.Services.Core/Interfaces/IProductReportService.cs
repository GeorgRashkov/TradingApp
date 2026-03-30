
using TradingApp.ViewModels.ProductReport;

namespace TradingApp.Services.Core.Interfaces
{
    public interface IProductReportService
    {
        int ProductReportPageIndex { get; set; }
        Task<List<ProductReportViewModel>> GetReportsAsync(int pageIndex);
        Task<List<ProductReportViewModel>> GetReportsForProductAsync(int pageIndex, Guid reportedProductId);
        Task<ProductReportDetailsViewModel?> GetProductReportAsync(Guid reportId);
    }
}
