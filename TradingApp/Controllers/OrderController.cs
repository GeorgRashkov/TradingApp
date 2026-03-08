using Microsoft.AspNetCore.Mvc;
using TradingApp.GCommon;
using TradingApp.GCommon.ErrorCodes;
using TradingApp.Services.Core.Interfaces;
using TradingApp.ViewModels.Order;

namespace TradingApp.Controllers
{
    public class OrderController : ControllerBase
    {

        private IProductService _productService;
        private IOrderService _orderService;

        public OrderController(IProductService productService, IOrderService orderService)
        {
            _productService = productService;
            _orderService = orderService;

        }



        //<Create sell order methods

        [HttpPost]
        //this method will show the confirmation page when the user presses the button for creating an order
        public async Task<IActionResult> CreateSellOrder(Guid productId, int ordersCount)
        {
            Result result = await _orderService.CanUserCreateSellOrdersOfSpecificProductAsync(productId: productId, userId: LoggedUserId, ordersCount: ordersCount);
            string? productName = await _productService.GetProductNameAsync(productId: productId);


            if (result.Success == false)
            {
                string errorMessage = Get_CreateSellOrder_ErrorMessage(errorCode: result.ErrorCode, productName: productName);
                TempData["title"] = "Error";
                TempData["message"] = errorMessage;
                return RedirectToAction(nameof(Message));
            }

            //the success message will always contain the count of sell orders which the user can create for the specified product
            ordersCount = int.Parse(result.SuccessMessage!);//await _orderService.FitOrdersCreationCountInBoundariesAsync(ordersCount: ordersCount, productId: productId, userId: LoggedUserId);

            //create the order view model            
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

        //this method is formed based on the logic of the service method for creating a sell order of a product 
        //it's purpose is to provide a proper error message which will be shown to the user based on the error code
        private string Get_CreateSellOrder_ErrorMessage(string errorCode, string? productName)
        {

            string errorMessage = errorCode switch
            {
                string code when code == ProductErrorCodes.ProductNotFound =>
                    "The product was not found!",

                string code when code == UserErrorCodes.UserNotFound =>
                    "You cannot create sell orders if you are not logged in!",

                string code when code == ProductErrorCodes.ProductInvalidCreator =>
                    "You are not allowed to create sell orders of products created by other users!",

                string code when code == ProductErrorCodes.ProductInvalidStatus =>
                $"You cannot create a sell order of the product {productName} because it's status is not 'approved'!",

                string code when code == ProductErrorCodes.ProductMaxActiveSellOrdersReached =>
                    $"You cannot create sell orders for product {productName} because the product has reached the maximum number of active sale orders!",

                string code when code == UserErrorCodes.UserMaxActiveSellOrdersReached =>
                "You cannot create sell orders because you reached the maximum number of active sale orders.",

                string code when code == ProductErrorCodes.ProductNotFound =>
                    "The product was not found!",

                _ => "Something went wrong."
            };

            return errorMessage;
        }

        [HttpPost]
        //this method will create the order when the user confirms the creation
        public async Task<IActionResult> CreateSellOrder_execute(Guid productId, int ordersCount)
        {
            Result result = null!;

            try
            {
                //try to create and save the sell orders of the product in the DB
                result = await _orderService.CreateSellOrdersAsync(creatorId: LoggedUserId, productId: productId, ordersCount: ordersCount);
            }
            catch (Exception e)
            {
                Console.Write(e.Message.ToString());

                TempData["title"] = "Error";
                TempData["message"] = $"An error occured while attempting to create your order/s! Please try again later.";
                return RedirectToAction(nameof(Message));
            }

            string? productName = await _productService.GetProductNameAsync(productId: productId);

            if (result.Success == false)
            {
                TempData["title"] = "Error";
                TempData["message"] = Get_CreateSellOrder_ErrorMessage(errorCode: result.ErrorCode, productName: productName);
                return RedirectToAction(nameof(Message));
            }

            //return a success message page
            ordersCount = int.Parse(result.SuccessMessage!);//the success message will always contain the count of sell orders which the user can create for the specified product
            TempData["title"] = "Success";
            TempData["message"] = $"You successfully created {ordersCount} sell order/s of the product {productName}.";
            return RedirectToAction(nameof(Message));
        }
        //Create sell order methods>







        //<Cancel sell order methods

        [HttpPost]
        //this method will show the confirmation page when the user presses the button for cancelling an order
        public async Task<IActionResult> CancelSellOrder(Guid productId, int ordersCount)
        {
            Result result = await _orderService.CanUserCancelSellOrdersOfSpecificProductAsync(productId: productId, userId: LoggedUserId, ordersCount: ordersCount);
            string? productName = await _productService.GetProductNameAsync(productId: productId);

            if (result.Success == false)
            {
                string errorMessage = Get_CancelSellOrder_ErrorMessage(errorCode: result.ErrorCode, productName: productName);
                TempData["title"] = "Error";
                TempData["message"] = errorMessage;
                return RedirectToAction(nameof(Message));
            }

            //the success message will always contain the count of sell orders which the user can cancel for the specified product
            ordersCount = int.Parse(result.SuccessMessage!);

            //create the order view model
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

        //this method is formed based on the logic of the service method for cancelling a sell order of a product 
        //it's purpose is to provide a proper error message which will be shown to the user based on the error code
        private string Get_CancelSellOrder_ErrorMessage(string errorCode, string? productName)
        {

            string errorMessage = errorCode switch
            {
                string code when code == ProductErrorCodes.ProductNotFound =>
                    "The product was not found!",

                string code when code == UserErrorCodes.UserNotFound =>
                    "You cannot cancel sell orders if you are not logged in!",

                string code when code == ProductErrorCodes.ProductInvalidCreator =>
                    "You are not allowed to cancel sell orders of products created by other users!",

                string code when code == ProductErrorCodes.ProductInvalidStatus =>
                $"You cannot create a sell order of the product {productName} because it's status is not 'approved'!",

                string code when code == ProductErrorCodes.ProductHasNoActiveSaleOrders =>
                $"The product {productName} has no active sell orders to cancel!",

                _ => "Something went wrong."
            };

            return errorMessage;
        }

        [HttpPost]
        //this method will show the confirmation page when the user presses the button for cancelling an order
        public async Task<IActionResult> CancelSellOrder_execute(Guid productId, int ordersCount)
        {
            Result result = null!;
            try
            {
                //try to set the status of the orders to cancelled
                result = await _orderService.CancelSellOrdersAsync(creatorId: LoggedUserId, productId: productId, ordersCount: ordersCount);
            }
            catch (Exception e)
            {
                Console.Write(e.Message.ToString());

                TempData["title"] = "Error";
                TempData["message"] = $"An error occured while attempting to cancel your active sell order/s! Please try again later.";
                return RedirectToAction(nameof(Message));
            }

            string? productName = await _productService.GetProductNameAsync(productId: productId);

            if (result.Success == false)
            {
                string errorMessage = Get_CancelSellOrder_ErrorMessage(errorCode: result.ErrorCode, productName: productName);
                TempData["title"] = "Error";
                TempData["message"] = errorMessage;
                return RedirectToAction(nameof(Message));
            }

            //the success message will always contain the count of sell orders which the user can cancel for the specified product
            ordersCount = int.Parse(result.SuccessMessage!);


            //return a success message page
            TempData["title"] = "Success";
            TempData["message"] = $"You successfully cancelled {ordersCount} sell order/s of the product {productName}.";
            return RedirectToAction(nameof(Message));
        }

        //Create sell order methods>







        //<Buy sell order methods

        [HttpGet]
        public async Task<IActionResult> BuySellOrder(Guid productId)
        {
            Result result = await _orderService.CanUserBuySellOrderOfSpecificProductAsync(productId: productId, userId: LoggedUserId);
            string? productName = await _productService.GetProductNameAsync(productId: productId);

            if (result.Success == false)
            {
                string errorMessage = Get_BuySellOrder_ErrorMessage(errorCode: result.ErrorCode, productName: productName);
                TempData["title"] = "Error";
                TempData["message"] = errorMessage;
                return RedirectToAction(nameof(Message));
            }

            //create the order view model            
            OrderViewModel order = new OrderViewModel()
            {
                Message = $"You are about to purchase the product {productName}",
                ProductId = productId
            };

            //return a confirmation view
            TempData["ReturnUrl"] = Referer;
            return View(order);
        }

        //this method is formed based on the logic of the service method for buying a sell order of a product 
        //it's purpose is to provide a proper error message which will be shown to the user based on the error code
        private string Get_BuySellOrder_ErrorMessage(string errorCode, string? productName)
        {

            string errorMessage = errorCode switch
            {
                string code when code == ProductErrorCodes.ProductNotFound =>
                    "The product was not found!",

                string code when code == UserErrorCodes.UserNotFound =>
                    "You cannot buy products if you are not logged in!",

                string code when code == ProductErrorCodes.ProductInvalidCreator =>
                    "You are not allowed to buy your own products!",

                string code when code == ProductErrorCodes.ProductInvalidStatus =>
                $"You cannot buy the product {productName} because it's status is not 'approved'!",

                string code when code == ProductErrorCodes.ProductHasNoActiveSaleOrders =>
                $"You cannot buy the product {productName} because it has no active sell orders!",

                string code when code == UserErrorCodes.UserInsufficientBalance =>
                   $"You do not have enough money in your balance to buy the product {productName}!",

                string code when code == ProductErrorCodes.ProductAlreadyPurchased =>
                $"You are not allowed to buy products you previosly purchased! You can find and dowload the product {productName} in the invoices page.",

                _ => "Something went wrong."
            };

            return errorMessage;
        }

        [HttpPost]
        public async Task<IActionResult> BuySellOrder_execute(Guid productId)
        {
            string? productName = await _productService.GetProductNameAsync(productId: productId);
            Result result = null!;
            try
            {
                result = await _orderService.BuySellOrderAsync(productId: productId, buyerId: LoggedUserId);
            }
            catch (Exception e)
            {
                Console.Write(e.Message.ToString());

                TempData["title"] = "Error";
                TempData["message"] = $"An error occured while attempting to purchase product {productName}! Please try again later.";
                return RedirectToAction(nameof(Message));
            }

            if (result.Success == false)
            {
                string errorMessage = Get_BuySellOrder_ErrorMessage(errorCode: result.ErrorCode, productName: productName);
                TempData["title"] = "Error";
                TempData["message"] = errorMessage;
                return RedirectToAction(nameof(Message));
            }

            //return a success message page
            TempData["title"] = "Success";
            TempData["message"] = $"You successfully purchased product {productName}.";
            return RedirectToAction(nameof(Message));
        }

        //Buy sell order methods>

    }
}
