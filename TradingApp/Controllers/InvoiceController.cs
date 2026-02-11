using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;
using TradingApp.Data;
using TradingApp.Data.Models;
using TradingApp.Services;
using TradingApp.ViewModels.Invoice;

namespace TradingApp.Controllers
{
    public class InvoiceController : Controller
    {
        private CrudDb _crudDb;
        private CrudFile _crudFile;

        public InvoiceController(ApplicationDbContext context)
        {
            _crudDb = new CrudDb(context);
            _crudFile = new CrudFile();
        }

        //this is the Id of the currently logged user; if the user is not logged the value will be null 
        private string LoggedUserId
        {
            get { return User.FindFirst(ClaimTypes.NameIdentifier)?.Value; }
        }


        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Invoices()
        {
            List<InvoicesViewModel> loggedUserCompletedOrders = await _crudDb.
            GetCompletedOrdersAsync<InvoicesViewModel>(userId: LoggedUserId,
            buyerSelector: co => new InvoicesViewModel
            {
                Id = co.Id,
                Title = co.TitleForBuyer,
                CompletedAt = co.CompletedAt.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)
            },
            sellerSelector: co => new InvoicesViewModel
            {
                Id = co.Id,
                Title = co.TitleForSeller,
                CompletedAt = co.CompletedAt.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)
            }
            );            

            loggedUserCompletedOrders.OrderBy(co => DateTime.Parse(co.CompletedAt));

            return View(model: loggedUserCompletedOrders);
        }


        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Invoice(Guid completedOrderId)
        {
            CompletedOrder? completedOrder = await _crudDb.GetCompletedOrderAsync(completedOrderId);

            //checks whether the completed order exists
            if (completedOrder is null) 
            { return NotFound(); }

            string loggedUserId = LoggedUserId;

            //checks whether the logged user is the buyer or the seller of the completed order
            if (loggedUserId != completedOrder.BuyerId && loggedUserId != completedOrder.SellerId) 
            { return Forbid(); }

            bool isUserTheBuyer = loggedUserId == completedOrder.BuyerId;
            decimal price = isUserTheBuyer ? completedOrder.PricePaid : completedOrder.SellerRevenue;
            string title = isUserTheBuyer ? completedOrder.TitleForBuyer : completedOrder.TitleForSeller;
            string productCreatorName = (await _crudDb.GetUserAsync(completedOrder.Product.CreatorId)).UserName;
            
            InvoiceViewModel invoiceViewModel = new InvoiceViewModel()
            {
                Id = completedOrder.Id,
                Title = title,
                CompletedAt = completedOrder.CompletedAt.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                ProductId = completedOrder.Product.Id,
                ProductName = completedOrder.Product.Name,
                ProductCreatorName = productCreatorName,
                Price = price.ToString("f2"),
                IsUserTheBuyer = isUserTheBuyer
            };

            return View(model: invoiceViewModel);
        }


        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Download3dModelFile(Guid completedOrderId)
        {
            CompletedOrder? completedOrder = await _crudDb.GetCompletedOrderAsync(completedOrderId);

            if (completedOrder is null)
            {
                return NotFound();
            }

            if(LoggedUserId != completedOrder.BuyerId)
            {
                return Forbid();
            }

            string productCreatorName = (await _crudDb.GetUserAsync(completedOrder.Product.CreatorId)).UserName;
            string productName = completedOrder.Product.Name;
            byte[] bytes;
            try
            {
                bytes = _crudFile.Get3dModelFileBytes(creatorName: productCreatorName, productName: productName);
            }
            catch (Exception e) 
            {
                Console.WriteLine(e.Message);
                TempData["title"] = "Error";
                TempData["message"] = "An error occured while attempting to dowload the file!";
                return View(viewName: "Message");
            }

            return File(bytes, "image/jpg", productName+".jpg");
        }
    }
}
