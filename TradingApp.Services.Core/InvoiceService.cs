using Microsoft.EntityFrameworkCore;
using System.Globalization;
using TradingApp.Data;
using TradingApp.Data.Models;
using TradingApp.GCommon;
using TradingApp.Services.Core.Interfaces;
using TradingApp.ViewModels.Invoice;
using TradingApp.ViewModels.Product;

namespace TradingApp.Services.Core
{
    public class InvoiceService: IInvoiceService
    {
        private ApplicationDbContext _context;
        private const int _invoicesPerPage = ApplicationConstants.InvoicesPerPage;

        public InvoiceService(ApplicationDbContext context) 
        {
            _context = context;
        }

        public int InvoicePageIndex { get; private set; }

        private void SetInvoicePage(int pageIndex, int userInvoicesCount)
        {
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageIndex = pageIndex * _invoicesPerPage >= userInvoicesCount ? (int)Math.Ceiling((decimal)userInvoicesCount / (decimal)_invoicesPerPage) - 1 : pageIndex;
            InvoicePageIndex = pageIndex;
        }

        public async Task<IEnumerable<InvoicesViewModel>> GetCompletedOrdersAsync(string userId, int pageIndex)
        {
            int userInvoicesCount = await _context
                .CompletedOrders
                .Where(co => co.SellerId == userId || co.BuyerId == userId)
                .CountAsync();

            if (userInvoicesCount == 0)
            { return new List<InvoicesViewModel>(); }

            SetInvoicePage(pageIndex, userInvoicesCount);

            List<InvoicesViewModel> userCompletedOrders = await _context
                .CompletedOrders
                .AsNoTracking()
                .Where(co => userId == co.BuyerId || userId == co.SellerId)
                .OrderByDescending(co => co.CompletedAt)
                .Skip(InvoicePageIndex * _invoicesPerPage).Take(_invoicesPerPage)
                .Select(co => new InvoicesViewModel
                {
                    Id = co.Id,
                    Title = co.BuyerId == userId ? co.TitleForBuyer:co.TitleForSeller,
                    CompletedAt = co.CompletedAt.ToString(ApplicationConstants.DateTimeFormat, CultureInfo.InvariantCulture)
                }).ToListAsync();

            return userCompletedOrders;
        }

        public async Task<InvoiceViewModel?> GetCompletedOrderAsync(string userId, Guid completedOrderId)
        {
            CompletedOrder? completedOrder = await _context
                .CompletedOrders
                .AsNoTracking()
                .Include(co => co.Seller)
                .Include(co => co.Buyer)
                .Include(co => co.Product)
                .Where(co => co.Id == completedOrderId)
                .SingleOrDefaultAsync();

            //checks whether the completed order exists
            if (completedOrder is null)
            {
                return null;
            }

            //checks whether the logged user is the buyer or the seller of the completed order
            if (userId != completedOrder.BuyerId && userId != completedOrder.SellerId)
            {
                return null;
            }

            bool isUserTheBuyer = userId == completedOrder.BuyerId;
            decimal price = isUserTheBuyer ? completedOrder.PricePaid : completedOrder.SellerRevenue;
            string title = isUserTheBuyer ? completedOrder.TitleForBuyer : completedOrder.TitleForSeller;
            
            InvoiceViewModel invoiceViewModel = new InvoiceViewModel()
            {
                Id = completedOrder.Id,
                Title = title,
                CompletedAt = completedOrder.CompletedAt.ToString(ApplicationConstants.DateTimeFormat, CultureInfo.InvariantCulture),
                ProductId = completedOrder.Product?.Id,
                ProductName = completedOrder.Product?.Name,
                ProductCreatorName = completedOrder.Seller?.UserName,
                Price = price.ToString("f2"),
                IsUserTheBuyer = isUserTheBuyer
            };

            return invoiceViewModel;
        }
    }
}
