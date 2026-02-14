using TradingApp.ViewModels.Invoice;

namespace TradingApp.Services.Core.Interfaces
{
    public interface IInvoiceService
    {
        Task<IEnumerable<InvoicesViewModel>> GetCompletedOrdersAsync(string userId);
        Task<InvoiceViewModel?> GetCompletedOrderAsync(string userId, Guid completedOrderId);       
    }
}
