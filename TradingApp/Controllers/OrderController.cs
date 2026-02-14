using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TradingApp.Services.Core.Interfaces;
using TradingApp.ViewModels.Order;

namespace TradingApp.Controllers
{
    public class OrderController : Controller
    {

        private IProductService _productService;
        private IOrderService _orderService;


        public OrderController(IProductService productService, IOrderService orderService)
        {
            _productService = productService;
            _orderService = orderService;

        }

        private string LoggedUserId
        {
            get { return User.FindFirst(ClaimTypes.NameIdentifier)?.Value; }
        }

        private string Referer
        {
            get { return Request.Headers["Referer"].ToString(); }
        }






        [HttpPost]
        //this method will show the confirmation page when the user presses the button for creating an order
        public async Task<IActionResult> CreateSellOrder(Guid productId, int ordersCount)
        {
            string errorMessage = await _orderService.CanUserCreateSellOrderOfSpecificProductAsync(productId: productId, userId: LoggedUserId);

            if (errorMessage != "")
            {
                TempData["title"] = "Error";
                TempData["message"] = errorMessage;
                return RedirectToAction(nameof(Message));
            }

            //make sure the created orders are 1 or more and that their count is not above the max number of total sale orders per product nor above the max number of total sale orders per user
            ordersCount = await _orderService.FitOrdersCreationCountInBoundariesAsync(ordersCount: ordersCount, productId: productId, userId: LoggedUserId);

            //create the order view model
            string productName = await _productService.GetProductNameAsync(productId);
            OrderViewModel order = new OrderViewModel()
            {
                Message = $"You are about to create {ordersCount} sell orders of the product {productName}",
                ProductId = productId,
                OrdersCount = ordersCount,
            };

            //return a confirmation view
            TempData["ReturnUrl"] = Referer;
            return View(order);
        }

        [HttpPost]
        //this method will create the order when the user confirms the creation
        public async Task<IActionResult> CreateSellOrder_execute(Guid productId, int ordersCount)
        {
            string errorMessage = await _orderService.CanUserCreateSellOrderOfSpecificProductAsync(productId: productId, userId: LoggedUserId);

            if (errorMessage != "")
            {
                TempData["title"] = "Error";
                TempData["message"] = errorMessage;
                return RedirectToAction(nameof(Message));
            }

            //make sure the created orders are 1 or more and that their count is not above the max number of total sale orders per product nor above the max number of total sale orders per user
            ordersCount = await _orderService.FitOrdersCreationCountInBoundariesAsync(ordersCount: ordersCount, productId: productId, userId: LoggedUserId);

            try
            {
                //try to create and save the sell orders of the product in the DB
                await _orderService.CreateSellOrders(creatorId: LoggedUserId, productId: productId, ordersCount: ordersCount);
            }
            catch (Exception e)
            {
                Console.Write(e.Message.ToString());

                TempData["title"] = "Error";
                TempData["message"] = $"An error occured while attempting to create your order/s!";
                return RedirectToAction(nameof(Message));
            }

            //return a success message page
            string productName = await _productService.GetProductNameAsync(productId);
            TempData["title"] = "Success";
            TempData["message"] = $"You successfully created {ordersCount} sell order/s of the product {productName}.";
            return RedirectToAction(nameof(Message));
        }









        [HttpPost]
        //this method will show the confirmation page when the user presses the button for cancelling an order
        public async Task<IActionResult> CancelSellOrder(Guid productId, int ordersCount)
        {
            string errorMessage = await _orderService.CanUserCancelSellOrderOfSpecificProductAsync(productId: productId, userId: LoggedUserId);

            if (errorMessage != "")
            {
                TempData["title"] = "Error";
                TempData["message"] = errorMessage;
                return RedirectToAction(nameof(Message));
            }

            //make sure the created orders are 1 or more and that their count is not above the max number of total sale orders per product nor above the max number of total sale orders per user
            ordersCount = await _orderService.FitOrdersCancelationCountInBoundariesAsync(ordersCount: ordersCount, productId: productId);

            //create the order view model
            string productName = await _productService.GetProductNameAsync(productId);
            OrderViewModel order = new OrderViewModel()
            {
                Message = $"You are about to cancel {ordersCount} sell orders of the product {productName}",
                ProductId = productId,
                OrdersCount = ordersCount,
            };

            //return a confirmation view
            TempData["ReturnUrl"] = Referer;
            return View(order);
        }

        [HttpPost]
        //this method will show the confirmation page when the user presses the button for cancelling an order
        public async Task<IActionResult> CancelSellOrder_execute(Guid productId, int ordersCount)
        {

            string errorMessage = await _orderService.CanUserCancelSellOrderOfSpecificProductAsync(productId: productId, userId: LoggedUserId);

            if (errorMessage != "")
            {
                TempData["title"] = "Error";
                TempData["message"] = errorMessage;
                return RedirectToAction(nameof(Message));
            }

            //make sure the created orders are 1 or more and that their count is not above the max number of total sale orders per product nor above the max number of total sale orders per user
            ordersCount = await _orderService.FitOrdersCancelationCountInBoundariesAsync(ordersCount: ordersCount, productId: productId);

            string productName = await _productService.GetProductNameAsync(productId);

            try
            {
                //try to set the status of the orders to cancelled
                await _orderService.CancelSellOrdersAsync(productId: productId, ordersCount: ordersCount);
            }
            catch (Exception e)
            {
                Console.Write(e.Message.ToString());

                TempData["title"] = "Error";
                TempData["message"] = $"An error occured while attempting to cancel your order/s!";
                return RedirectToAction(nameof(Message));
            }

            //return a success message page
            TempData["title"] = "Success";
            TempData["message"] = $"You successfully cancelled {ordersCount} sell order/s of the product {productName}.";
            return RedirectToAction(nameof(Message));
        }









        [HttpGet]
        public async Task<IActionResult> BuySellOrder(Guid productId)
        {
            string errorMessage = await _orderService.CanUserBuySellOrderOfSpecificProductAsync(productId: productId, userId: LoggedUserId);

            if (errorMessage != "")
            {
                TempData["title"] = "Error";
                TempData["message"] = errorMessage;
                return RedirectToAction(nameof(Message));
            }

            //create the order view model
            string productName = await _productService.GetProductNameAsync(productId);
            OrderViewModel order = new OrderViewModel()
            {
                Message = $"You are about to purchase the product {productName}",
                ProductId = productId
            };
            //return a confirmation view
            TempData["ReturnUrl"] = Referer;
            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> BuySellOrder_execute(Guid productId)
        {
            string errorMessage = await _orderService.CanUserBuySellOrderOfSpecificProductAsync(productId: productId, userId: LoggedUserId);

            if (errorMessage != "")
            {
                TempData["title"] = "Error";
                TempData["message"] = errorMessage;
                return RedirectToAction(nameof(Message));
            }

            //create the order view model
            string productName = await _productService.GetProductNameAsync(productId);
            try
            {
                //the sell order is bought and the changes are applied to the DB (executes only when when the sell order can be bought)
                await _orderService.BuySellOrderAsync(productId: productId, buyerId: LoggedUserId);
            }
            catch (Exception e)
            {
                Console.Write(e.Message.ToString());

                TempData["title"] = "Error";
                TempData["message"] = $"An error occured while attempting to purchase product {productName}!";
                return RedirectToAction(nameof(Message));
            }


            //return a success message page
            TempData["title"] = "Success";
            TempData["message"] = $"You successfully purchased product {productName}.";
            return RedirectToAction(nameof(Message));
        }


        public IActionResult Message()
        {
            return View();
        }
    }
}
