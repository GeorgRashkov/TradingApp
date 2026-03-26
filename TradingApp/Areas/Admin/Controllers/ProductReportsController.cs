using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;
using TradingApp.GCommon;
using TradingApp.GCommon.Enums;
using TradingApp.GCommon.ErrorCodes;
using TradingApp.Services.Core.Interfaces;
using TradingApp.ViewModels.ProductReport;

namespace TradingApp.Areas.Admin.Controllers
{
    public class ProductReportsController : ControllerBase
    {
        private IProductReportService _productReport_Service;
        IProductReportOperationsService _productReport_OperationsService;
        public ProductReportsController(IProductReportService productReport_Service, IProductReportOperationsService productReport_OperationsService) 
        {
            _productReport_Service = productReport_Service;
            _productReport_OperationsService = productReport_OperationsService;
        }


        [HttpGet]
        public async Task<IActionResult> ProductsReports(int pageIndex)
        {
            List<ProductsReportsViewModel> productReports = await _productReport_Service.GetReportsAsync(pageIndex: pageIndex);

            if (productReports.Count() == 0)
            { return View(viewName: nameof(ProductReports), model: null); }

            ViewData["page"] = _productReport_Service.ProductReportPageIndex;
            ViewData["action"] = "ProductsReports";

            return View(viewName: nameof(ProductReports), model: productReports);
        }


        [HttpGet]
        public async Task<IActionResult> ProductReports(int pageIndex, string reportedProductId)
        {
            Guid? lastUsed_ReportedProductId = Get_LastUsed_ReportedProductId(reportedProductId: reportedProductId);
            if (lastUsed_ReportedProductId == null)
            { return View(viewName: nameof(ProductsReports), model: null); }

            IEnumerable<ProductsReportsViewModel> productReports = await _productReport_Service.GetReportsForProductAsync(pageIndex: pageIndex, reportedProductId: (Guid)lastUsed_ReportedProductId);
            if (productReports.Count() == 0)
            { return View(model: null); }

            ViewData["page"] = _productReport_Service.ProductReportPageIndex;
            ViewData["action"] = "ProductReports";

            return View(model: productReports);
        }

        
        private Guid? Get_LastUsed_ReportedProductId(string? reportedProductId)
        {
            if (reportedProductId.IsNullOrEmpty() == false)
            {
                bool is_ReportedProductId_validGuid = Guid.TryParse(reportedProductId, out Guid reportedProductIdAsGuid);
                if (is_ReportedProductId_validGuid == true)
                { TempData["_lastUsed_ReportedProductId"] = reportedProductIdAsGuid; }
            }
            return (Guid?)TempData.Peek("_lastUsed_ReportedProductId");
        }


        public async Task<IActionResult> ProductReport(Guid reportId) 
        {
            ProductReportViewModel? productReport = await _productReport_Service.GetProductReportAsync(reportId: reportId);

            if(productReport == null) 
            { return NotFound(); }

            List<string> productReportStatuses = Enum.GetValues(typeof(ProductReportStatus)).Cast<ProductReportStatus>().Select(element => element.ToString()).ToList();
            ViewData["productReportStatuses"] = productReportStatuses;

            return View(model: productReport);

        }



        //this method is formed based on the logic of the service method for changing the status of a product report 
        //its purpose is to provide a proper error message which will be shown to the user based on the error code
        private string Get_SetProductReportStatus_ErrorMessage(string errorCode)
        {

            string errorMessage = errorCode switch
            {
                string code when code == ProductReportErrorCodes.ProductReportNotFound =>
                    "The report you are trying to manage could not be found!",

                _ => "Something went wrong."
            };

            return errorMessage;
        }
        public async Task<IActionResult> SetProductReportStatus(Guid reportId, ProductReportStatus reportStatus) 
        {
            Result result;
            try
            {
                result = await _productReport_OperationsService.SetReportStatusAsync(reportId: reportId, newReportStatus: reportStatus);
            }
            catch (Exception e) 
            {
                Console.Write(e.Message.ToString());

                TempData["title"] = "Error";
                TempData["message"] = "An error occured while attempting to change the report status! Please try again later.";
                return RedirectToAction(nameof(Message));
            }

            if (result.Success == false)
            {
                string errorMessage = Get_SetProductReportStatus_ErrorMessage(result.ErrorCode);
                TempData["title"] = "Error";
                TempData["message"] = errorMessage;
                return RedirectToAction(nameof(Message));
            }

            TempData["title"] = "Success";
            TempData["message"] = $"The report status was updated succesfully.";
            return RedirectToAction(nameof(Message));
        }
    }
}
