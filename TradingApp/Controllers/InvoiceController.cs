using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Security.Claims;
using TradingApp.Data;
using TradingApp.Services;
using TradingApp.ViewModels.Invoice;

namespace TradingApp.Controllers
{
    public class InvoiceController : Controller
    {
        private CrudDb _crudDb;

        public InvoiceController(ApplicationDbContext context)
        {
            _crudDb = new CrudDb(context);
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

            return View(loggedUserCompletedOrders);
        }
    }
}
