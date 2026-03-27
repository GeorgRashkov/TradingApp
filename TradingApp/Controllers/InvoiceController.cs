using Microsoft.AspNetCore.Mvc;
using TradingApp.Data;
using TradingApp.ViewModels.Invoice;
using TradingApp.Services.Core.Interfaces;


namespace TradingApp.Controllers
{
    public class InvoiceController : ControllerBase
    {        
        private IInvoiceService _invoiceService;
        private IProductFileService _productFileService;

        private ILogger<InvoiceController> _logger;

        public InvoiceController(ApplicationDbContext context, IInvoiceService invoiceService, IProductFileService productFileService, ILogger<InvoiceController> logger)
        {           
            _invoiceService = invoiceService;
            _productFileService = productFileService;

            _logger = logger;
        }


        
        [HttpGet]
        public async Task<IActionResult> Invoices(int pageIndex)
        {
            IEnumerable<InvoicesViewModel> loggedUserCompletedOrders = await _invoiceService.GetCompletedOrdersAsync(userId: LoggedUserId, pageIndex: pageIndex);
            ViewData["page"] = _invoiceService.InvoicePageIndex;
            return View(model: loggedUserCompletedOrders);
        }

        
        [HttpGet]
        public async Task<IActionResult> Invoice(Guid completedOrderId)
        {
            InvoiceViewModel? invoiceViewModel = await _invoiceService.GetCompletedOrderAsync(userId: LoggedUserId, completedOrderId: completedOrderId);

            if(invoiceViewModel == null)
            {
                return NotFound();
            }

            return View(model: invoiceViewModel);
        }
        

        
        [HttpGet]
        public async Task<IActionResult> Download3dModelFile(Guid completedOrderId)
        {
            InvoiceViewModel? invoiceViewModel = await _invoiceService.GetCompletedOrderAsync(userId:LoggedUserId,completedOrderId: completedOrderId);

            if (invoiceViewModel == null)
            {
                return NotFound();
            }
                           
            byte[] bytes;
            try
            {
                bytes = _productFileService.Get3dModelFileBytes(productCreatorName: invoiceViewModel.ProductCreatorName, productName: invoiceViewModel.ProductName);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occured while attempting to dowload a 3D model file!");

                TempData["title"] = "Error";
                TempData["message"] = "An error occured while attempting to dowload the file!";
                return View(viewName: "Message");
            }

            return File(bytes, "image/jpg", invoiceViewModel.ProductName + ".jpg");
        }
        
    }
}
