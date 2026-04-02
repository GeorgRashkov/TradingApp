
using TradingApp.Data.Dtos.ProductReport;
using TradingApp.Data.Models;
using TradingApp.GCommon.Enums;

namespace TradingApp.Data.Repository.Interfaces
{
    public interface IProductReportRepository
    {
        //<number methods
        Task<int> GetReportsCountAsync();
        Task<int> GetReportsCountForProductAsync(Guid productId);
        //number methods>

        //<entity methods
        Task<ProductReport?> GetProductReportByIdAsync(Guid reportId);
        //entity methods>

        //<dto methods
        Task<IEnumerable<ProductReportDto>> GetProductReportsAsync(int skipCount, int takeCount);
        Task<IEnumerable<ProductReportDto>> GetReportsForProductAsync(int skipCount, int takeCount, Guid reportedProductId);
        Task<ProductReportDetailsDto?> GetProductReportAsync(Guid reportId);
        //dto methods>


        //<operation methods
        Task CreateReportAsync(ProductReport report);
        Task SetReportStatusAsync(ProductReport report, ProductReportStatus newReportStatus);
        //operation methods>
    }
}
