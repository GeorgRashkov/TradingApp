
using Microsoft.EntityFrameworkCore;
using TradingApp.Data;
using TradingApp.GCommon.Enums;
using TradingApp.Services.Core.Interfaces;

namespace TradingApp.Services.Core
{
    public class ProductBoolsService: IProductBoolsService
    {
        private ApplicationDbContext _context;
        public ProductBoolsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> DoesUserExistAsync(string userId)
        {
            return await _context.Users
                    .AsNoTracking()
                    .AnyAsync(u => u.Id == userId);
        }

        public async Task<bool> DoesProductExistAsync(Guid productId)
        {
            return await _context.Products
                    .AsNoTracking()
                    .AnyAsync(p => p.Id == productId);
        }
        public async Task<bool> DoesProductCreatedByUserExistAsync(string userId, string productName)
        {
            return await _context.Products
                    .AsNoTracking()
                    .AnyAsync(p => p.CreatorId == userId && p.Name == productName);
        }
       
        public async Task<bool> DoesProductCreatedByUserExistAsync(string userId, Guid productId)
        {
            return await _context.Products
                    .AsNoTracking()
                    .AnyAsync(p => p.CreatorId == userId && p.Id == productId);
        }

        public async Task<bool> DoesProductHaveActiveSaleOrdersAsync(Guid productId)
        {
            return await _context.SellOrders
                .AsNoTracking()
                .AnyAsync(so => so.ProductId == productId && so.Status == SellOrderStatus.active);                     
        }

        public async Task<bool> IsProductApprovedAsync(Guid productId)
        {
            return await _context.Products
                    .AsNoTracking()
                    .AnyAsync(p => p.Id == productId && p.Status==GCommon.Enums.ProductStatus.approved);
        }

        public async Task<bool> IsProductSuggestedToOrderRequestAsync(Guid productId, Guid orderRequestId)
        {
            return await _context
                .SellOrderSuggestions
                .AsNoTracking()
                .Where(sos => sos.ProductId == productId && sos.OrderRequestId == orderRequestId)
                .AnyAsync();
        }
    }
}
