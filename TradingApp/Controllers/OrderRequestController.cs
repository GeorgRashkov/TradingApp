using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TradingApp.GCommon;
using TradingApp.Services.Core.Interfaces;
using TradingApp.ViewModels.OrderRequest;

namespace TradingApp.Controllers
{
    public class OrderRequestController : ControllerBase
    {
        private IOrderRequestService _orderRequestService;
        public OrderRequestController(IOrderRequestService orderRequestService)
        {
            _orderRequestService = orderRequestService;
        }

        [HttpGet]
        public async Task<IActionResult> OrderRequests(int pageIndex)
        {
            IEnumerable<OrderRequestsViewModel> orderRequests = await _orderRequestService.GetActiveRequestsAsync(pageIndex: pageIndex);
            if (orderRequests.Count() == 0)
            { return View(model: null); }

            ViewData["page"] = _orderRequestService.RequestPageIndex;

            return View(model: orderRequests);
        }

        [HttpGet]
        public async Task<IActionResult> OrderRequest(Guid orderRequestId)
        {
            OrderRequestViewModel? orderRequest = await _orderRequestService.GetDetailsForActiveRequestAsync(requestId: orderRequestId);

            if (orderRequest == null)
            { return NotFound(); }

            return View(model: orderRequest);
        }

        [HttpGet]
        public async Task<IActionResult> MyOrderRequests(int pageIndex)
        {
            IEnumerable<MyOrderRequestsViewModel> orderRequests = await _orderRequestService.GetActiveRequestsCreatedByUserAsync(pageIndex: pageIndex, userId: LoggedUserId);

            if (orderRequests.Count() == 0)
            { return View(model: null); }

            ViewData["page"] = _orderRequestService.RequestPageIndex;

            return View(model: orderRequests);
        }

        [HttpGet]
        public async Task<IActionResult> MyOrderRequest(Guid orderRequestId)
        {
            MyOrderRequestViewModel? orderRequest = await _orderRequestService.GetDetailsForActiveRequestCreatedByUserAsync(requestId: orderRequestId, userId: LoggedUserId);

            if (orderRequest == null)
            { return NotFound(); }

            int loggedUserActiveOrderSuggestionsCount = await _orderRequestService.GetUserActiveRequestsCountAsync(LoggedUserId);
            ViewData["currentUserMaxSuggestionsCountReached"] = loggedUserActiveOrderSuggestionsCount >= ApplicationConstants.UserMaxActiveOrderSuggetions ? true : false;

            return View(model: orderRequest);
        }
    }
}
