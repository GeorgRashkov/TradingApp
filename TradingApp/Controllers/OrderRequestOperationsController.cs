using Microsoft.AspNetCore.Mvc;
using TradingApp.GCommon;
using TradingApp.GCommon.ErrorCodes;
using TradingApp.Services.Core.Interfaces;
using TradingApp.ViewModels.InputOrderRequest;

namespace TradingApp.Controllers
{
    public class OrderRequestOperationsController : ControllerBase
    {
        private IOrderRequestService _orderRequestService;
        private IOrderRequestOperationsService _orderRequestOperationsService;
        private IProductService _productService;
        private IUserService _userService;

        private ILogger<InvoiceController> _logger;
        public OrderRequestOperationsController(IOrderRequestService orderRequestService, IOrderRequestOperationsService orderRequestOperationsService, IProductService productService, IUserService userService, ILogger<InvoiceController> logger)
        {
            _orderRequestOperationsService = orderRequestOperationsService;
            _productService = productService;
            _userService = userService;
            _orderRequestService = orderRequestService;

            _logger = logger;
        }


        public async Task<IActionResult> CreateSuggestionForOrderRequest(Guid productId, Guid requestId)
        {
            Result result = await _orderRequestOperationsService.CreateSuggestionForOrderRequest(productId: productId, suggesterId: LoggedUserId, requestId: requestId);
            string? productName = await _productService.GetProductNameAsync(productId);

            if (result.Success == false)
            {
                string errorMessage = Get_CreateSuggestion_ErrorMessage(result.ErrorCode, productName);
                TempData["title"] = "Error";
                TempData["message"] = errorMessage;
                return RedirectToAction(nameof(Message));
            }

            //return a success message page
            TempData["title"] = "Success";
            TempData["message"] = $"You successfully suggested the product {productName}.";
            return RedirectToAction(nameof(Message));
        }

        //this method is formed based on the logic of the service method for creating a suggestion for order request 
        //its purpose is to provide a proper error message which will be shown to the user based on the error code
        public string Get_CreateSuggestion_ErrorMessage(string errorCode, string? productName)
        {

            string errorMessage = errorCode switch
            {
                string code when code == OrderRequestErrorCodes.RequestNotFound =>
               "The request was not found!",

                string code when code == OrderRequestErrorCodes.RequestSuggestionSameCreator =>
              "You are not allowed to suggest products to your own requests!",


                string code when code == OrderRequestErrorCodes.RequestInvalidStatus =>
                    "The request cannot receice product suggestions because its status is not `active`!",

                string code when code == ProductErrorCodes.ProductNotFound =>
                    "The product was not found!",

                string code when code == ProductErrorCodes.ProductInvalidCreator =>
                    "You are not allowed to suggest products created by other users!",

                string code when code == ProductErrorCodes.ProductInvalidStatus =>
                $"You cannot suggest the product {productName} because its status is not 'approved'!",

                string code when code == ProductErrorCodes.ProductHasNoActiveSaleOrders =>
                    $"You cannot suggest the product {productName} because it has no active sale orders!",

                string code when code == ProductErrorCodes.ProductAlreadySuggestedToRequest =>
                    $"You have already suggested the product {productName} to this request!",

                _ => "Something went wrong."
            };

            return errorMessage;
        }




        [HttpGet]
        public IActionResult CreateOrderRequest()
        {
            return View();
        }

        //this method is formed based on the logic of the service method for creating an order request 
        //its purpose is to provide a proper error message which will be shown to the user based on the error code
        public string Get_CreateOrderRequest_ErrorMessage(string errorCode)
        {

            string errorMessage = errorCode switch
            {
                string code when code == UserErrorCodes.UserNotFound =>
                    "You have to login in order to create a request.",

                string code when code == OrderRequestErrorCodes.RequestWithSameTitleAlreadyExists =>
                    "A request with the same title already exists!\n Consider using unique title before creating the request.",

                _ => "Something went wrong."
            };

            return errorMessage;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrderRequest_execute([FromForm] CreatedOrderRequestModel createdOrderRequestModel)
        {
            if (ModelState.IsValid == false)
            {
                return View(viewName: nameof(CreateOrderRequest), model: createdOrderRequestModel);
            }

            Result result;

            try
            {
                result = await _orderRequestOperationsService.CreateOrderRequest(title: createdOrderRequestModel.Title,
                    description: createdOrderRequestModel.Description,
                    maxPrice: createdOrderRequestModel.MaxPrice,
                    creatorId: LoggedUserId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while attempting to create an order request!");

                TempData["title"] = "Error";
                TempData["message"] = "An error occurred while creating the order request! Please try again later.";
                return RedirectToAction(nameof(Message));
            }

            if (result.Success == false)
            {
                string errorMessage = Get_CreateOrderRequest_ErrorMessage(result.ErrorCode);
                ModelState.AddModelError(key: string.Empty, errorMessage: errorMessage);
                return View(viewName: nameof(CreateOrderRequest), model: createdOrderRequestModel);
            }
            else
            {
                TempData["title"] = "Success";
                TempData["message"] = $"Your request was created succesfully.";
                return RedirectToAction(nameof(Message));
            }

        }




        [HttpGet]
        public async Task<IActionResult> UpdateOrderRequest(Guid orderRequestId)
        {
            string? userId = await _userService.GetCreatorIdOfRequestAsync(orderRequestId: orderRequestId);

            if (userId == null)
            { return NotFound(); }
            else if (userId != LoggedUserId)
            { return Unauthorized(); }

            UpdatedOrderRequestModel updatedOrderRequestModel = (await _orderRequestService.GetUpdatedOrderRequestModelAsync(orderRequestId: orderRequestId))!;

            return View(updatedOrderRequestModel);
        }

        //this method is formed based on the logic of the service method for updating an order request 
        //its purpose is to provide a proper error message which will be shown to the user based on the error code
        public string Get_UpdateOrderRequest_ErrorMessage(string errorCode)
        {

            string errorMessage = errorCode switch
            {
                string code when code == OrderRequestErrorCodes.RequestNotFound =>
                    "The request you are trying to edit could not be found!",

                string code when code == OrderRequestErrorCodes.RequestInvalidCreator =>
                    "You are not allowed to edit requests created by other users!",

                string code when code == OrderRequestErrorCodes.RequestWithSameTitleAlreadyExists =>
                "A request with the same title already exists!\n Consider using unique title before updating the request.",

                _ => "Something went wrong."
            };

            return errorMessage;
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrderRequest_execute(UpdatedOrderRequestModel updatedOrderRequestModel)
        {
            if (ModelState.IsValid == false)
            {
                return View(viewName: nameof(UpdateOrderRequest), model: updatedOrderRequestModel);
            }

            Result result;

            try
            {
                result = await _orderRequestOperationsService.UpdateOrderRequest(
                    id: updatedOrderRequestModel.Id,
                    title: updatedOrderRequestModel.Title,
                    description: updatedOrderRequestModel.Description,
                    maxPrice: updatedOrderRequestModel.MaxPrice,
                    creatorId: LoggedUserId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while attempting to update an order request!");

                TempData["title"] = "Error";
                TempData["message"] = "An error occurred while updating the order request! Please try again later.";
                return RedirectToAction(nameof(Message));
            }

            if (result.Success == false)
            {
                string errorMessage = Get_UpdateOrderRequest_ErrorMessage(result.ErrorCode);
                ModelState.AddModelError(key: string.Empty, errorMessage: errorMessage);
                return View(viewName: nameof(UpdateOrderRequest), model: updatedOrderRequestModel);
            }
            else
            {
                TempData["title"] = "Success";
                TempData["message"] = $"Your request was updated succesfully.";
                return RedirectToAction(nameof(Message));
            }

        }






        [HttpGet]
        public async Task<IActionResult> CancelOrderRequest(Guid orderRequestId)
        {
            string? userId = await _userService.GetCreatorIdOfRequestAsync(orderRequestId: orderRequestId);

            if (userId == null)
            { return NotFound(); }
            else if (userId != LoggedUserId)
            { return Unauthorized(); }

            return View(model: orderRequestId);
        }


        //this method is formed based on the logic of the service method for cancelling an order request 
        //its purpose is to provide a proper error message which will be shown to the user based on the error code
        public string Get_CancelOrderRequest_ErrorMessage(string errorCode)
        {

            string errorMessage = errorCode switch
            {
                string code when code == OrderRequestErrorCodes.RequestNotFound =>
                    "The request you are trying to delete could not be found!",

                string code when code == OrderRequestErrorCodes.RequestInvalidCreator =>
                    "You are not allowed to delete requests created by other users!",

                string code when code == OrderRequestErrorCodes.RequestInvalidStatus =>
                "The request cannot be deleted because its status is not `active`.",

                _ => "Something went wrong."
            };

            return errorMessage;
        }

        [HttpPost]
        public async Task<IActionResult> CancelOrderRequest_execute(Guid orderRequestId)
        {
            Result result;
            try
            {
                result = await _orderRequestOperationsService.CancelOrderRequestAsync(id: orderRequestId, userId: LoggedUserId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while attempting to cancel an order request!");

                TempData["title"] = "Error";
                TempData["message"] = "An error occurred while deleting the request. Please try again later.";
                return RedirectToAction(nameof(Message));
            }

            if (result.Success == false)
            {
                TempData["title"] = "Error";
                TempData["message"] = Get_CancelOrderRequest_ErrorMessage(result.ErrorCode);
                return RedirectToAction(nameof(Message));
            }
            else
            {
                TempData["title"] = "Success";
                TempData["message"] = $"Your request was deleted succesfully.";
                return RedirectToAction(nameof(Message));
            }
        }
    }
}
