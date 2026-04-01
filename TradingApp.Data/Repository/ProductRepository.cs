
using Microsoft.EntityFrameworkCore;
using TradingApp.Data.Repository.Interfaces;

namespace TradingApp.Data.Repository
{
    public class ProductRepository: IProductRepository
    {
        private readonly ApplicationDbContext _context;
        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        //<bool methods

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
                .AnyAsync(so => so.ProductId == productId && so.Status == GCommon.Enums.SellOrderStatus.active);
        }

        public async Task<bool> IsProductApprovedAsync(Guid productId)
        {
            return await _context.Products
                    .AsNoTracking()
                    .AnyAsync(p => p.Id == productId && p.Status == GCommon.Enums.ProductStatus.approved);
        }

        public async Task<bool> IsProductSuggestedToOrderRequestAsync(Guid productId, Guid orderRequestId)
        {
            return await _context
                .SellOrderSuggestions
                .AsNoTracking()
                .Where(sos => sos.ProductId == productId && sos.OrderRequestId == orderRequestId)
                .AnyAsync();
        }
        //bool methods>
    }
}
