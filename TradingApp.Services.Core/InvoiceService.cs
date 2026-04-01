
using System.Globalization;
using TradingApp.Data.Dtos.CompletedOrder;
using TradingApp.Data.Models;
using TradingApp.Data.Repository.Interfaces;
using TradingApp.GCommon;
using TradingApp.Services.Core.Interfaces;
using TradingApp.ViewModels.Invoice;

namespace TradingApp.Services.Core
{
    public class InvoiceService: IInvoiceService
    {
        
        private const int _invoicesPerPage = ApplicationConstants.InvoicesPerPage;
        private readonly ICompletedOrderRepository _completedOrderRepository;

        public InvoiceService(ICompletedOrderRepository completedOrderRepository) 
        {
            _completedOrderRepository = completedOrderRepository;
        }

        public int InvoicePageIndex { get; private set; }

        private void SetInvoicePage(int pageIndex, int userInvoicesCount)
        {
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageIndex = pageIndex * _invoicesPerPage >= userInvoicesCount ? (int)Math.Ceiling((decimal)userInvoicesCount / (decimal)_invoicesPerPage) - 1 : pageIndex;
            InvoicePageIndex = pageIndex;
        }

        public async Task<IEnumerable<InvoiceViewModel>> GetCompletedOrdersAsync(string userId, int pageIndex)
        {
            int userInvoicesCount = await _completedOrderRepository
                .GetCompletedOrdersCountAsync(userId: userId);
                

            if (userInvoicesCount == 0)
            { return new List<InvoiceViewModel>(); }

            SetInvoicePage(pageIndex, userInvoicesCount);

            IEnumerable<CompletedOrderDto> userCompletedOrders = await _completedOrderRepository
                .GetCompletedOrdersAsync(userId: userId, skipCount: InvoicePageIndex * _invoicesPerPage, takeCount: _invoicesPerPage);


            List<InvoiceViewModel> userCompletedOrdersOutput = userCompletedOrders.Select(co => new InvoiceViewModel
            {
                Id = co.Id,
                Title = co.BuyerId == userId ? co.TitleForBuyer : co.TitleForSeller,
                CompletedAt = co.CompletedAt.ToString(ApplicationConstants.DateTimeFormat, CultureInfo.InvariantCulture)
            }).ToList();

            return userCompletedOrdersOutput;
        }

        public async Task<InvoiceDetailsViewModel?> GetCompletedOrderAsync(string userId, Guid completedOrderId)
        {
            CompletedOrder? completedOrder = await _completedOrderRepository.GetCompletedOrderAsync(completedOrderId: completedOrderId);

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
            
            InvoiceDetailsViewModel invoiceViewModel = new InvoiceDetailsViewModel()
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
