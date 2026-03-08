

using Microsoft.EntityFrameworkCore;
using TradingApp.Data;
using TradingApp.GCommon;
using TradingApp.GCommon.Enums;
using TradingApp.Services.Core.Interfaces;
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


        public async Task<IEnumerable<OrderRequestsViewModel>> GetActiveRequestsAsync(int pageIndex)
        {
            int requestsCount = await _context
                .OrderRequests
                .AsNoTracking()
                .Where(or => or.Status == OrderRequestStatus.active)
                .CountAsync();

            if (requestsCount == 0)
            { return new List<OrderRequestsViewModel>(); }

            SetRequestPage(pageIndex, requestsCount);


            IEnumerable<OrderRequestsViewModel> orderRequests = await _context
                .OrderRequests
                .AsNoTracking()
                .Where(or => or.Status == OrderRequestStatus.active)
                .Skip(RequestPageIndex * _requestsPerPage).Take(_requestsPerPage)
                .Select(or => new OrderRequestsViewModel
                {
                    Title = or.Title,
                    MaxPrice = or.MaxPrice
                })
                .ToListAsync();

            return orderRequests;
        }

        public async Task<OrderRequestViewModel?> GetDetailsForActiveRequestAsync(Guid requestId)
        {
            OrderRequestViewModel? request = await _context
                .OrderRequests
                .Include(or => or.Creator)
                .AsNoTracking()
                .Where(or => or.Id == requestId && or.Status == OrderRequestStatus.active)
                .Select(or => new OrderRequestViewModel
                {
                    Title = or.Title,
                    Description = or.Description,
                    MaxPrice = or.MaxPrice,
                    CreationDate = or.CreatedAt.ToString(ApplicationConstants.DateFormat),
                    CreatorName = or.Creator.UserName!
                }).SingleOrDefaultAsync();

            return request;
        }




        public async Task<IEnumerable<MyOrderRequestsViewModel>> GetActiveRequestsCreatedByUserAsync(int pageIndex, string userId)
        {
            int requestsCount = await _context
               .OrderRequests
               .AsNoTracking()
               .Where(or => or.Status == OrderRequestStatus.active && or.CreatorId == userId)
               .CountAsync();

            if (requestsCount == 0)
            { return new List<MyOrderRequestsViewModel>(); }

            SetRequestPage(pageIndex, requestsCount);

            IEnumerable<MyOrderRequestsViewModel> orderRequests = await _context
                .OrderRequests
                .AsNoTracking()
                .Where(or => or.Status == OrderRequestStatus.active && or.CreatorId == userId)
                .Skip(RequestPageIndex * _requestsPerPage).Take(_requestsPerPage)
                .Select(or => new MyOrderRequestsViewModel
                {
                    Title = or.Title,
                    MaxPrice = or.MaxPrice
                }).ToListAsync();
            
            return orderRequests;
        }


        public async Task<MyOrderRequestViewModel?> GetDetailsForActiveRequestCreatedByUserAsync(Guid requestId, string userId)
        {
            MyOrderRequestViewModel? request = await _context
                .OrderRequests                
                .AsNoTracking()
                .Where(or => or.Id == requestId && or.Status == OrderRequestStatus.active && or.CreatorId == userId)
                .Select(or => new MyOrderRequestViewModel
                {
                    Title = or.Title,
                    Description = or.Description,
                    MaxPrice = or.MaxPrice,
                    CreationDate = or.CreatedAt.ToString(ApplicationConstants.DateFormat)                   
                }).SingleOrDefaultAsync();

            return request;
        }
    }
}
