using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TradingApp.Data;
using TradingApp.ViewModels.Invoice;
using TradingApp.Services.Core.Interfaces;


namespace TradingApp.Controllers
{
    public class InvoiceController : Controller
    {        
        private IInvoiceService _invoiceService;
        private IProductFileService _productFileService;

        public InvoiceController(ApplicationDbContext context, IInvoiceService invoiceService, IProductFileService productFileService)
        {           
            _invoiceService = invoiceService;
            _productFileService = productFileService;
        }

        //this is the Id of the currently logged user; if the user is not logged the value will be null 
        private string LoggedUserId
        {
            get { return User.FindFirst(ClaimTypes.NameIdentifier)?.Value; }
        }




        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Invoices(int pageIndex)
        {
            IEnumerable<InvoicesViewModel> loggedUserCompletedOrders = await _invoiceService.GetCompletedOrdersAsync(userId: LoggedUserId, pageIndex: pageIndex);
            ViewData["page"] = _invoiceService.InvoicePageIndex;
            return View(model: loggedUserCompletedOrders);
        }

        [Authorize]
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
        

        [Authorize]
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
                Console.WriteLine(e.Message);
                TempData["title"] = "Error";
                TempData["message"] = "An error occured while attempting to dowload the file!";
                return View(viewName: "Message");
            }

            return File(bytes, "image/jpg", invoiceViewModel.ProductName + ".jpg");
        }
        
    }
}
