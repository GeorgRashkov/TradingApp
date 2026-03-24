using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TradingApp.Services.Core.Interfaces;
using TradingApp.ViewModels.ProductReport;

namespace TradingApp.Areas.Admin.Controllers
{
    public class ProductReportsController : ControllerBase
    {
        private IReportedProductService _reportedProductService;
        public ProductReportsController(IReportedProductService reportedProductService) 
        {
            _reportedProductService = reportedProductService;
        }


        [HttpGet]
        public async Task<IActionResult> ProductsReports(int pageIndex)
        {
            List<ProductsReportsViewModel> productReports = await _reportedProductService.GetReportsAsync(pageIndex: pageIndex);

            if (productReports.Count() == 0)
            { return View(model: null); }

            ViewData["page"] = _reportedProductService.ProductReportPageIndex;
            ViewData["action"] = "ProductsReports";

            return View(viewName: nameof(ProductReports), model: productReports);
        }


        [HttpGet]
        public async Task<IActionResult> ProductReports(int pageIndex, string reportedProductId)
        {
            Guid? lastUsed_ReportedProductId = Get_LastUsed_ReportedProductId(reportedProductId: reportedProductId);
            if (lastUsed_ReportedProductId == null)
            { return View(viewName: nameof(ProductsReports), model: null); }

            IEnumerable<ProductsReportsViewModel> productReports = await _reportedProductService.GetReportsForProductAsync(pageIndex: pageIndex, reportedProductId: (Guid)lastUsed_ReportedProductId);
            if (productReports.Count() == 0)
            { return View(model: null); }

            ViewData["page"] = _reportedProductService.ProductReportPageIndex;
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
    }
}
