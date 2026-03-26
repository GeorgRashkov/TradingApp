
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TradingApp.GCommon;
using TradingApp.GCommon.Enums;
using TradingApp.GCommon.ErrorCodes;
using TradingApp.Services.Core.Interfaces;
using TradingApp.ViewModels.Input_ProductReport;

namespace TradingApp.Controllers
{
    public class ProductReportOperationsController : ControllerBase
    {
        private IProductReportOperationsService _productReportOperationsService;
        private IProductBoolsService _productBoolsService;
        public ProductReportOperationsController(IProductReportOperationsService productReportOperationsService, IProductBoolsService productBoolsService)
        {
            _productReportOperationsService = productReportOperationsService;
            _productBoolsService = productBoolsService;
        }

        [HttpGet]
        public async Task<IActionResult> Create_ProductReport(Guid reportedProductId)
        {
            bool isReportedProductCreatedByCurrentUser = await _productBoolsService.DoesProductCreatedByUserExistAsync(userId: LoggedUserId, productId: reportedProductId);
            if(isReportedProductCreatedByCurrentUser == true) 
            {
                TempData["title"] = "Error";
                TempData["message"] = Get_Create_ProductReportErrorMessage(errorCode: ProductReportErrorCodes.ProductReportInvalidCreator);
                return RedirectToAction(nameof(Message));
            }

            Created_ProductReportModel productReport = new Created_ProductReportModel()
            {
                ReportedProductId = reportedProductId
            };

            ViewData["reportTypes"] = Enum.GetValues(enumType: typeof(ProductReportType));

            return View(model: productReport);
        }

        //this method is formed based on the logic of the service method for creating a product report
        //its purpose is to provide a proper error message which will be shown to the user based on the error code
        private string Get_Create_ProductReportErrorMessage(string errorCode)
        {

            string errorMessage = errorCode switch
            {
                string code when code == UserErrorCodes.UserNotFound =>
                "You have to login in order to report a product.",

                string code when code == ProductErrorCodes.ProductNotFound =>
                "The product you are trying to report was not found.",

                string code when code == ProductReportErrorCodes.ProductReportInvalidCreator =>
                "You are not allowed to report your own products.",


                _ => "Something went wrong."
            };

            return errorMessage;
        }

        [HttpPost]
        public async Task<IActionResult> Create_ProductReport(Created_ProductReportModel productReport)
        {
            if (ModelState.IsValid == false)
            {
                ViewData["reportTypes"] = Enum.GetValues(enumType: typeof(ProductReportType));
                return View(model: productReport); 
            }

            Result result;
            try
            {
                result = await _productReportOperationsService.CreateReportAsync(
                    reporterId: LoggedUserId,
                    reportedProductId: productReport.ReportedProductId,
                    title: productReport.Title,
                    message: productReport.Message,
                    reportType: productReport.Type
                    );
            }
            catch (Exception e)
            {
                Console.Write(e.Message.ToString());

                TempData["title"] = "Error";
                TempData["message"] = "An error occured while attempting to create the product report! Please try again later.";
                return RedirectToAction(nameof(Message));
            }

            if (result.Success == false)
            {
                string errorMessage = Get_Create_ProductReportErrorMessage(result.ErrorCode);
                ModelState.AddModelError(key: string.Empty, errorMessage: errorMessage);
                ViewData["reportTypes"] = Enum.GetValues(enumType: typeof(ProductReportType));
                return View(model: productReport);
            }

            TempData["title"] = "Success";
            TempData["message"] = "The product was succesfully reported.";
            return RedirectToAction(nameof(Message));
        }
    }
}
