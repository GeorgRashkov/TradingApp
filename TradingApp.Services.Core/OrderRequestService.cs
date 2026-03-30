

using Microsoft.EntityFrameworkCore;
using TradingApp.Data;
using TradingApp.GCommon;
using TradingApp.GCommon.Enums;
using TradingApp.Services.Core.Interfaces;
using TradingApp.ViewModels.InputOrderRequest;
using TradingApp.ViewModels.OrderRequest;

namespace TradingApp.Services.Core
{
    public class OrderRequestService: IOrderRequestService
    {
        private ApplicationDbContext _context;
        private const int _requestsPerPage = ApplicationConstants.RequestsPerPage;

        public OrderRequestService(ApplicationDbContext context)
        {
            _context = context;
        }

        public int RequestPageIndex { get; private set; }

        private void SetRequestPage(int pageIndex, int requestsCount)
        {
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageIndex = pageIndex * _requestsPerPage >= requestsCount ? (int)Math.Ceiling((decimal)requestsCount / (decimal)_requestsPerPage) - 1 : pageIndex;
            RequestPageIndex = pageIndex;
        }


        public async Task<IEnumerable<OrderRequestViewModel>> GetActiveRequestsAsync(int pageIndex)
        {
            int requestsCount = await _context
                .OrderRequests
                .AsNoTracking()
                .Where(or => or.Status == OrderRequestStatus.active)
                .CountAsync();

            if (requestsCount == 0)
            { return new List<OrderRequestViewModel>(); }

            SetRequestPage(pageIndex, requestsCount);


            IEnumerable<OrderRequestViewModel> orderRequests = await _context
                .OrderRequests
                .AsNoTracking()
                .Where(or => or.Status == OrderRequestStatus.active)
                .Skip(RequestPageIndex * _requestsPerPage).Take(_requestsPerPage)
                .Select(or => new OrderRequestViewModel
                {
                    Id = or.Id,
                    Title = or.Title,
                    MaxPrice = or.MaxPrice.ToString("f2")
                })
                .ToListAsync();

            return orderRequests;
        }

        public async Task<OrderRequestDetailsViewModel?> GetDetailsForActiveRequestAsync(Guid requestId)
        {
            OrderRequestDetailsViewModel? request = await _context
                .OrderRequests
                .Include(or => or.Creator)
                .AsNoTracking()
                .Where(or => or.Id == requestId && or.Status == OrderRequestStatus.active)
                .Select(or => new OrderRequestDetailsViewModel
                {
                    Id = or.Id,
                    Title = or.Title,
                    Description = or.Description,
                    MaxPrice = or.MaxPrice.ToString("f2"),
                    CreationDate = or.CreatedAt.ToString(ApplicationConstants.DateFormat),
                    CreatorName = or.Creator.UserName!
                }).SingleOrDefaultAsync();

            return request;
        }




        public async Task<IEnumerable<MyOrderRequestViewModel>> GetActiveRequestsCreatedByUserAsync(int pageIndex, string userId)
        {
            int requestsCount = await GetUserActiveRequestsCountAsync(userId);

            if (requestsCount == 0)
            { return new List<MyOrderRequestViewModel>(); }

            SetRequestPage(pageIndex, requestsCount);

            IEnumerable<MyOrderRequestViewModel> orderRequests = await _context
                .OrderRequests
                .AsNoTracking()
                .Where(or => or.Status == OrderRequestStatus.active && or.CreatorId == userId)
                .Skip(RequestPageIndex * _requestsPerPage).Take(_requestsPerPage)
                .Select(or => new MyOrderRequestViewModel
                {
                    Id = or.Id,
                    Title = or.Title,
                    MaxPrice = or.MaxPrice.ToString("f2")
                }).ToListAsync();
            
            return orderRequests;
        }


        public async Task<MyOrderRequestDetailsViewModel?> GetDetailsForActiveRequestCreatedByUserAsync(Guid requestId, string userId)
        {
            MyOrderRequestDetailsViewModel? request = await _context
                .OrderRequests 
                .Include(or => or.SellOrderSuggestions)
                .ThenInclude(sos => sos.Product)
                .ThenInclude(p => p.SellOrders)
                .AsNoTracking()
                .Where(or => or.Id == requestId && or.Status == OrderRequestStatus.active && or.CreatorId == userId)
                .Select(or => new MyOrderRequestDetailsViewModel
                {
                    Id = or.Id,
                    Title = or.Title,
                    Description = or.Description,
                    MaxPrice = or.MaxPrice.ToString("f2"),
                    CreationDate = or.CreatedAt.ToString(ApplicationConstants.DateFormat),
                    HasSuggestions = or.SellOrderSuggestions.Any(sos => sos.Product.Status == ProductStatus.approved && sos.Product.SellOrders.Any(so => so.Status == SellOrderStatus.active))
                }).SingleOrDefaultAsync();

            return request;
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


        public async Task<UpdatedOrderRequestModel?> GetUpdatedOrderRequestModelAsync(Guid orderRequestId)
        {
            UpdatedOrderRequestModel? updatedOrderRequestModel = await _context
                .OrderRequests
                .AsNoTracking()
                .Where(or => or.Id == orderRequestId)
                .Select(or => new UpdatedOrderRequestModel
                {
                    Id = or.Id,
                    Title = or.Title,
                    Description = or.Description,
                    MaxPrice = or.MaxPrice
                }).SingleOrDefaultAsync();

            return updatedOrderRequestModel;
        }
    }
}
