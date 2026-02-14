using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using TradingApp.Data;
using TradingApp.Data.Helpers;
using TradingApp.Data.Models;
using TradingApp.GCommon;
using TradingApp.Services.Core.Interfaces;
using TradingApp.ViewModels.Invoice;
using static TradingApp.GCommon.EntityValidation;

namespace TradingApp.Services.Core
{
    public class InvoiceService: IInvoiceService
    {
        private ApplicationDbContext _context;
        public InvoiceService(ApplicationDbContext context) 
        {
            _context = context;
        }

        public async Task<IEnumerable<InvoicesViewModel>> GetCompletedOrdersAsync(string userId)
        {
            List<InvoicesViewModel> buyerCompletedOrders = await _context
                .CompletedOrders
                .AsNoTracking()
                .Where(co => co.BuyerId == userId)
                .Select(co => new InvoicesViewModel
                {
                    Id = co.Id,
                    Title = co.TitleForBuyer,
                    CompletedAt = co.CompletedAt.ToString(ApplicationConstants.DateFormat, CultureInfo.InvariantCulture)
                }).ToListAsync();


            List<InvoicesViewModel> sellerCompletedOrders = await _context
                .CompletedOrders
                .AsNoTracking()
                .Where(co => co.SellerId == userId)
                .Select(co => new InvoicesViewModel
                {
                    Id = co.Id,
                    Title = co.TitleForSeller,
                    CompletedAt = co.CompletedAt.ToString(ApplicationConstants.DateFormat, CultureInfo.InvariantCulture)
                }).ToListAsync();


            List<InvoicesViewModel> userCompletedOrders = [.. buyerCompletedOrders, .. sellerCompletedOrders];

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
                CompletedAt = completedOrder.CompletedAt.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                ProductId = completedOrder.Product.Id,
                ProductName = completedOrder.Product.Name,
                ProductCreatorName = completedOrder.Seller.UserName,
                Price = price.ToString("f2"),
                IsUserTheBuyer = isUserTheBuyer
            };

            return invoiceViewModel;
        }
    }
}
