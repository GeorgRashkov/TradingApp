using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradingApp.Data;
using TradingApp.Data.Models;
using TradingApp.ViewModels.Order;
using static TradingApp.GCommon.EntityValidation;

namespace TradingApp.Services.Core.Interfaces
{
    public interface IOrderService
    {
        Task CreateSellOrders(string creatorId, Guid productId, int ordersCount);
        Task CancelSellOrdersAsync(Guid productId, int ordersCount);
        Task<bool> DidUserBoughtProductAsync(Guid productId, string userId);




        Task<int> FitOrdersCreationCountInBoundariesAsync(int ordersCount, Guid productId, string userId);
        Task<int> FitOrdersCancelationCountInBoundariesAsync(int ordersCount, Guid productId);        
        Task<string> CanUserCreateSellOrderOfSpecificProductAsync(Guid productId, string userId);
        Task<string> CanUserCancelSellOrderOfSpecificProductAsync(Guid productId, string userId);
        Task<string> CanUserBuySellOrderOfSpecificProductAsync(Guid productId, string userId);
        Task BuySellOrderAsync(Guid productId, string buyerId);
    }
}
