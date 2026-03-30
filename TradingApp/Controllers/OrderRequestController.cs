using Microsoft.AspNetCore.Authorization;
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
        private IProductService _productService;
        public OrderRequestController(IOrderRequestService orderRequestService, IProductService productService)
        {
            _orderRequestService = orderRequestService;
            _productService = productService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> OrderRequests(int pageIndex)
        {
            IEnumerable<OrderRequestViewModel> orderRequests = await _orderRequestService.GetActiveRequestsAsync(pageIndex: pageIndex);
            if (orderRequests.Count() == 0)
            { return View(model: null); }

            ViewData["page"] = _orderRequestService.RequestPageIndex;

            return View(model: orderRequests);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> OrderRequest(Guid orderRequestId)
        {
            OrderRequestDetailsViewModel? orderRequest = await _orderRequestService.GetDetailsForActiveRequestAsync(requestId: orderRequestId);

            if (orderRequest == null)
            { return NotFound(); }

            Dictionary<string, string> idsAndNamesOfProducts = await _productService.GetIdsAndNamesOfApprovedProductsWithActiveSaleOrdersCreatedByUserAsync(LoggedUserId);

            return View(model: (orderRequest, idsAndNamesOfProducts));
        }

        [HttpGet]
        public async Task<IActionResult> MyOrderRequests(int pageIndex)
        {
            IEnumerable<MyOrderRequestViewModel> orderRequests = await _orderRequestService.GetActiveRequestsCreatedByUserAsync(pageIndex: pageIndex, userId: LoggedUserId);

            if (orderRequests.Count() == 0)
            { return View(model: null); }

            ViewData["page"] = _orderRequestService.RequestPageIndex;

            return View(model: orderRequests);
        }

        [HttpGet]
        public async Task<IActionResult> MyOrderRequest(Guid orderRequestId)
        {
            MyOrderRequestDetailsViewModel? orderRequest = await _orderRequestService.GetDetailsForActiveRequestCreatedByUserAsync(requestId: orderRequestId, userId: LoggedUserId);

            if (orderRequest == null)
            { return NotFound(); }

            int loggedUserActiveOrderSuggestionsCount = await _orderRequestService.GetUserActiveRequestsCountAsync(LoggedUserId);
            ViewData["currentUserMaxSuggestionsCountReached"] = loggedUserActiveOrderSuggestionsCount >= ApplicationConstants.UserMaxActiveOrderSuggetions ? true : false;

            return View(model: orderRequest);
        }
    }
}
