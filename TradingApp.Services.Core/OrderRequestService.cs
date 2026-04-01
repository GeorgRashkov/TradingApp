

using TradingApp.Data.Dtos.OrderRequest;
using TradingApp.Data.Models;
using TradingApp.Data.Repository.Interfaces;
using TradingApp.GCommon;
using TradingApp.Services.Core.Interfaces;
using TradingApp.ViewModels.InputOrderRequest;
using TradingApp.ViewModels.OrderRequest;

namespace TradingApp.Services.Core
{
    public class OrderRequestService : IOrderRequestService
    {
        private const int _requestsPerPage = ApplicationConstants.RequestsPerPage;
        private readonly IOrderRequestRepository _orderRequestRepository;

        public OrderRequestService(IOrderRequestRepository orderRequestRepository)
        {
            _orderRequestRepository = orderRequestRepository;
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
            int requestsCount = await _orderRequestRepository.GetActiveRequestsCountAsync();

            if (requestsCount == 0)
            { return new List<OrderRequestViewModel>(); }

            SetRequestPage(pageIndex, requestsCount);

            IEnumerable<OrderRequestDto> orderRequestsDtos = await _orderRequestRepository
                .GetActiveRequestsAsync(skipCount: RequestPageIndex * _requestsPerPage, takeCount: _requestsPerPage);

            List<OrderRequestViewModel> orderRequests = orderRequestsDtos
             .Select(or => new OrderRequestViewModel
             {
                 Id = or.Id,
                 Title = or.Title,
                 MaxPrice = or.MaxPrice.ToString("f2")
             })
             .ToList();


            return orderRequests;
        }

        public async Task<OrderRequestDetailsViewModel?> GetDetailsForActiveRequestAsync(Guid requestId)
        {
            OrderRequestDetailsDto? request = await _orderRequestRepository
                .GetDetailsForActiveRequestAsync(requestId: requestId);

            if (request == null)
            { return null; }

            OrderRequestDetailsViewModel requestViewModel = new OrderRequestDetailsViewModel
            {
                Id = request.Id,
                Title = request.Title,
                Description = request.Description,
                MaxPrice = request.MaxPrice.ToString("f2"),
                CreationDate = request.CreatedAt.ToString(ApplicationConstants.DateFormat),
                CreatorName = request.CreatorUserName
            };

            return requestViewModel;
        }




        public async Task<IEnumerable<MyOrderRequestViewModel>> GetActiveRequestsCreatedByUserAsync(int pageIndex, string userId)
        {
            int requestsCount = await _orderRequestRepository.GetUserActiveRequestsCountAsync(userId);

            if (requestsCount == 0)
            { return new List<MyOrderRequestViewModel>(); }

            SetRequestPage(pageIndex, requestsCount);

            IEnumerable<OrderRequestDto> orderRequestsDto = await _orderRequestRepository
                .GetActiveRequestsCreatedByUserAsync(userId: userId, skipCount: RequestPageIndex * _requestsPerPage, takeCount: _requestsPerPage);

            List<MyOrderRequestViewModel> orderRequests = orderRequestsDto
                .Select(or => new MyOrderRequestViewModel
                {
                    Id = or.Id,
                    Title = or.Title,
                    MaxPrice = or.MaxPrice.ToString("f2")
                }).ToList();

            return orderRequests;
        }

        public async Task<MyOrderRequestDetailsViewModel?> GetDetailsForActiveRequestCreatedByUserAsync(Guid requestId, string userId)
        {
            OrderRequestDetailsDto? requestDto = await _orderRequestRepository
                .GetDetailsForActiveRequestCreatedByUserAsync(requestId: requestId, userId: userId);

            if (requestDto == null)
            { return null; }

            MyOrderRequestDetailsViewModel request = new MyOrderRequestDetailsViewModel
            {
                Id = requestDto.Id,
                Title = requestDto.Title,
                Description = requestDto.Description,
                MaxPrice = requestDto.MaxPrice.ToString("f2"),
                CreationDate = requestDto.CreatedAt.ToString(ApplicationConstants.DateFormat),
                HasSuggestions = requestDto.HasSuggestions
            };

            return request;
        }        

        
        public async Task<int> GetUserActiveRequestsCountAsync(string userId)
        {
            int requestsCount = await _orderRequestRepository.GetUserActiveRequestsCountAsync(userId: userId);
            return requestsCount;
        }
        

        public async Task<UpdatedOrderRequestModel?> GetUpdatedOrderRequestModelAsync(Guid orderRequestId)
        {
            OrderRequest? orderRequest = await _orderRequestRepository.GetRequestAsync(requestId: orderRequestId);

            if(orderRequest == null) 
            { return null; }

            UpdatedOrderRequestModel? updatedOrderRequestModel = new UpdatedOrderRequestModel
            {
                Id = orderRequest.Id,
                Title = orderRequest.Title,
                Description = orderRequest.Description,
                MaxPrice = orderRequest.MaxPrice
            };
            return updatedOrderRequestModel;                        
        }
    }
}
