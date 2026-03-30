using TradingApp.ViewModels.Invoice;

namespace TradingApp.Services.Core.Interfaces
{
    public interface IInvoiceService
    {
        Task<IEnumerable<InvoiceViewModel>> GetCompletedOrdersAsync(string userId, int pageIndex);
        Task<InvoiceDetailsViewModel?> GetCompletedOrderAsync(string userId, Guid completedOrderId);
        int InvoicePageIndex { get; }
    }
}
