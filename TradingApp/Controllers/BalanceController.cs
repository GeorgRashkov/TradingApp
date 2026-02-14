using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using TradingApp.Services.Core.Interfaces;

namespace TradingApp.Controllers
{
    public class BalanceController : Controller
    {
        private readonly IBalanceService _balanceService;
        public BalanceController(IBalanceService balanceService) 
        {
            _balanceService = balanceService;
        }

        //this is the Id of the currently logged user; if the user is not logged the value will be null 
        private string LoggedUserId
        {
            get { return User.FindFirst(ClaimTypes.NameIdentifier)?.Value; }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Balance()
        {
            decimal balance = await _balanceService.GetUserBalanceAsync(LoggedUserId);
            return View(model:balance.ToString("f2"));
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> IncreaseBalance(decimal increasement)
        {
            try
            {
                await _balanceService.IncreaseUserBalanceAsync(userId: LoggedUserId, increasement: increasement);
            }
            catch (Exception e)
            {
                Console.Write(e.Message.ToString());

                TempData["title"] = "Error";
                TempData["message"] = $"An error occured while attempting to increase your balance!";
                return RedirectToAction(nameof(Message));
            }

            decimal balance = await _balanceService.GetUserBalanceAsync(LoggedUserId);
            TempData["title"] = "Success";
            TempData["message"] = $"Your balance was increased successfully! Your current balance is {balance.ToString("f2")}";
            return RedirectToAction(nameof(Message));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DecreaseBalance(decimal decreasement)
        {
            try
            {
                await _balanceService.DecreaseUserBalanceAsync(userId: LoggedUserId, decreasement: decreasement);
            }
            catch (Exception e)
            {
                Console.Write(e.Message.ToString());

                TempData["title"] = "Error";
                TempData["message"] = $"An error occurred while attempting to transfer money from your balance to your bank account!";
                return RedirectToAction(nameof(Message));
            }

            decimal balance = await _balanceService.GetUserBalanceAsync(LoggedUserId);
            TempData["title"] = "Success";
            TempData["message"] = $"Money from your balance were transfered successfully to your bank account! Your current balance is {balance.ToString("f2")}";
            return RedirectToAction(nameof(Message));
        }


        public IActionResult Message()
        {
            return View();
        }
    }
}
