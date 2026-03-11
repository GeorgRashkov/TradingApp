
using Microsoft.EntityFrameworkCore;
using TradingApp.Data;
using TradingApp.Services.Core.Interfaces;

namespace TradingApp.Services.Core
{
    public class OrderRequestBoolsService: IOrderRequestBoolsService
    {
        private ApplicationDbContext _context;
        public OrderRequestBoolsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> DoesOrderRequestExistAsync(Guid orderRequestId)
        {
            return await _context
                .OrderRequests
                .AsNoTracking()
                .AnyAsync(or => or.Id == orderRequestId);
        }

        public async Task<bool> IsOrderRequestActiveAsync(Guid orderRequestId)
        {
            return await _context
                .OrderRequests
                .AsNoTracking()
                .AnyAsync(or => or.Id == orderRequestId && or.Status == GCommon.Enums.OrderRequestStatus.active);
        }
    }
}
