
using TradingApp.GCommon;
using TradingApp.GCommon.Enums;

namespace TradingApp.Services.Core.Interfaces
{
    public interface IReportedProductOperationsService
    {
        Task<Result> CreateReportAsync(string reporterId, Guid reportedProductId, string title, string message, ProductReportType reportType);
    }
}
