
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using TradingApp.Data.Dtos.OrderRequest;
using TradingApp.Data.Models;
using TradingApp.Data.Repository.Interfaces;
using TradingApp.GCommon;
using TradingApp.GCommon.Enums;

namespace TradingApp.Data.Repository
{
    public class OrderRequestRepository : IOrderRequestRepository
    {
        private readonly ApplicationDbContext _context;
        public OrderRequestRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        //< Bool methods
        public async Task<bool> DoesOrderRequestExistAsync(Guid orderRequestId)
        {
            return await _context
                .OrderRequests
                .AsNoTracking()
                .AnyAsync(or => or.Id == orderRequestId);
        }

        public async Task<bool> DoesOrderRequestCreatedByUserExistAsync(string userId, string orderRequestTitle)
        {
            return await _context
                .OrderRequests
                .AsNoTracking()
                .AnyAsync(or => or.CreatorId == userId && or.Title == orderRequestTitle);
        }

        public async Task<bool> DoesOrderRequestCreatedByUserExistAsync(string userId, string orderRequestTitle, Guid[] orderRequestIdsToIgnore)
        {
            return await _context
                .OrderRequests
                .AsNoTracking()
                .AnyAsync(or => or.CreatorId == userId && or.Title == orderRequestTitle && orderRequestIdsToIgnore.Contains(or.Id) == false);
        }


        public async Task<bool> IsOrderRequestActiveAsync(Guid orderRequestId)
        {
            return await _context
                .OrderRequests
                .AsNoTracking()
                .AnyAsync(or => or.Id == orderRequestId && or.Status == GCommon.Enums.OrderRequestStatus.active);
        }

        //Bool methods>

        //<number methods
        public async Task<int> GetActiveRequestsCountAsync()
        {
            int requestsCount = await _context
                .OrderRequests
                .AsNoTracking()
                .Where(or => or.Status == OrderRequestStatus.active)
                .CountAsync();

            return requestsCount;
        }

        public async Task<int> GetUserActiveRequestsCountAsync(string userId)
        {
            int requestsCount = await _context
                .OrderRequests
                .AsNoTracking()
                .Where(or => or.Status == OrderRequestStatus.active && or.CreatorId == userId)
                .CountAsync();

            return requestsCount;
        }
        //number methods>


        //< Dto methods
        public async Task<IEnumerable<OrderRequestDto>> GetActiveRequestsAsync(int skipCount, int takeCount)
        {
            IEnumerable<OrderRequestDto> orderRequests = await _context
                .OrderRequests
                .AsNoTracking()
                .Where(or => or.Status == OrderRequestStatus.active)
                .Skip(skipCount).Take(takeCount)
                .Select(or => new OrderRequestDto
                {
                    Id = or.Id,
                    Title = or.Title,
                    MaxPrice = or.MaxPrice
                })
                .ToListAsync();

            return orderRequests;
        }

        public async Task<IEnumerable<OrderRequestDto>> GetActiveRequestsCreatedByUserAsync(string userId, int skipCount, int takeCount)
        {
            IEnumerable<OrderRequestDto> orderRequests = await _context
               .OrderRequests
               .AsNoTracking()
               .Where(or => or.Status == OrderRequestStatus.active && or.CreatorId == userId)
               .Skip(skipCount).Take(takeCount)
               .Select(or => new OrderRequestDto
               {
                   Id = or.Id,
                   Title = or.Title,
                   MaxPrice = or.MaxPrice
               }).ToListAsync();

            return orderRequests;
        }

        public async Task<OrderRequestDetailsDto?> GetDetailsForActiveRequestAsync(Guid requestId)
        {
            OrderRequestDetailsDto? request = await _context
                 .OrderRequests
                 .Include(or => or.Creator)
                 .Include(or => or.SellOrderSuggestions)
                 .ThenInclude(sos => sos.Product)
                 .ThenInclude(p => p.SellOrders)
                 .AsNoTracking()
                 .Where(or => or.Id == requestId && or.Status == OrderRequestStatus.active)
                 .Select(or => new OrderRequestDetailsDto
                 {
                     Id = or.Id,
                     Title = or.Title,
                     Description = or.Description,
                     MaxPrice = or.MaxPrice,
                     CreatedAt = or.CreatedAt,
                     CreatorUserName = or.Creator.UserName,
                     HasSuggestions = or.SellOrderSuggestions.Any(sos => sos.Product.Status == ProductStatus.approved && sos.Product.SellOrders.Any(so => so.Status == SellOrderStatus.active))
                 }).SingleOrDefaultAsync();

            return request;
        }
        public async Task<OrderRequestDetailsDto?> GetDetailsForActiveRequestCreatedByUserAsync(Guid requestId, string userId)
        {
            OrderRequestDetailsDto? request = await _context
                 .OrderRequests
                 .Include(or => or.Creator)
                 .Include(or => or.SellOrderSuggestions)
                 .ThenInclude(sos => sos.Product)
                 .ThenInclude(p => p.SellOrders)
                 .AsNoTracking()
                 .Where(or => or.Id == requestId && or.Status == OrderRequestStatus.active && or.CreatorId == userId)
                 .Select(or => new OrderRequestDetailsDto
                 {
                     Id = or.Id,
                     Title = or.Title,
                     Description = or.Description,
                     MaxPrice = or.MaxPrice,
                     CreatedAt = or.CreatedAt,
                     CreatorUserName = or.Creator.UserName,
                     HasSuggestions = or.SellOrderSuggestions.Any(sos => sos.Product.Status == ProductStatus.approved && sos.Product.SellOrders.Any(so => so.Status == SellOrderStatus.active))
                 }).SingleOrDefaultAsync();

            return request;
        }
        //Dto methods>

        //<entity methods
        public async Task<OrderRequest?> GetRequestAsync(Guid requestId)
        {
            OrderRequest? request = await _context.OrderRequests.FindAsync(requestId);

            return request;
        }
        //entity methods>



        //<operation methods

        public async Task CreateSellOrderSuggestionAsync(SellOrderSuggestion sellOrderSuggestion)
        {
            await _context.SellOrderSuggestions.AddAsync(sellOrderSuggestion);
            int affectedEntities = await _context.SaveChangesAsync();
            if (affectedEntities != 1)
            {
                throw new Exception("Failed to create sell order suggestion.");
            }
        }

        public async Task CreateOrderRequestAsync(OrderRequest orderRequest)
        {
            await _context.OrderRequests.AddAsync(orderRequest);
            int affectedEntities = await _context.SaveChangesAsync();
            if (affectedEntities != 1)
            {
                throw new Exception("Failed to create order request.");
            }
        }
        public async Task UpdateOrderRequest(OrderRequest orderRequest, string newTitle, string newDescription, decimal newMaxPrice)
        {
            _context.Attach<OrderRequest>(orderRequest);

            orderRequest.Title = newTitle;
            orderRequest.Description = newDescription;
            orderRequest.MaxPrice = newMaxPrice;

            int affectedEntities = await _context.SaveChangesAsync();
            if (affectedEntities != 1)
            {
                throw new Exception("Failed to update order request.");
            }
        }


        public async Task UpdateOrderRequestStatusAsync(OrderRequest orderRequest, OrderRequestStatus newStatus)
        {
            _context.Attach<OrderRequest>(orderRequest);
            orderRequest.Status = newStatus;

            int affectedEntities = await _context.SaveChangesAsync();
            if (affectedEntities != 1)
            {
                throw new Exception("Failed to update the status of order request.");
            }
        }
        //operation methods>
    }
}
