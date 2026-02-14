using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradingApp.ViewModels.Invoice;

namespace TradingApp.Services.Core.Interfaces
{
    public interface IInvoiceService
    {
        Task<IEnumerable<InvoicesViewModel>> GetCompletedOrdersAsync(string userId);
        Task<InvoiceViewModel?> GetCompletedOrderAsync(string userId, Guid completedOrderId);       
    }
}
