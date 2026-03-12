using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TradingApp.GCommon;
using TradingApp.GCommon.ErrorCodes;
using TradingApp.Services.Core.Interfaces;
using TradingApp.ViewModels.InputOrderRequest;
using TradingApp.ViewModels.InputProduct;

namespace TradingApp.Controllers
{
    public class OrderRequestOperationsController : ControllerBase
    {
        private IOrderRequestOperationsService _orderRequestOperationsService;
        private IProductService _productService;
        public OrderRequestOperationsController(IOrderRequestOperationsService orderRequestOperationsService, IProductService productService)
        {
            _orderRequestOperationsService = orderRequestOperationsService;
            _productService = productService;
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
                Console.Write(e.Message.ToString());

                TempData["title"] = "Error";
                TempData["message"] = "An error occurred while creating the order request. Please try again later.";
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
    }
}
