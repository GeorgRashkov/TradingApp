using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TradingApp.GCommon;
using TradingApp.GCommon.ErrorCodes;
using TradingApp.Services.Core.Interfaces;

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
            Result result = await _orderRequestOperationsService.CreateSuggestionForOrderRequest(productId: productId, userId: LoggedUserId, requestId: requestId);
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
                string code when code == ProductErrorCodes.ProductNotFound =>
                    "The product was not found!",

                string code when code == ProductErrorCodes.ProductInvalidCreator =>
                    "You are not allowed to suggest products created by other users!",

                string code when code == ProductErrorCodes.ProductInvalidStatus =>
                $"You cannot suggest the product {productName} because its status is not 'approved'!",

                string code when code == ProductErrorCodes.ProductHasNoActiveSaleOrders =>
                    $"You cannot suggest the product {productName} because it has no active sale orders!",

                string code when code == OrderRequestErrorCodes.RequestNotFound =>
                "The request was not found!",

                string code when code == OrderRequestErrorCodes.RequestInvalidStatus =>
                    "The request cannot receice product suggestions because its status is not `active`!",
                
                string code when code == ProductErrorCodes.ProductAlreadySuggestedToRequest =>
                    $"You have already suggested the product {productName} to this request!",

                _ => "Something went wrong."
            };

            return errorMessage;
        }
    }
}
