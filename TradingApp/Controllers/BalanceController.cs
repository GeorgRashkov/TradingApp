using Microsoft.AspNetCore.Mvc;
using TradingApp.Services.Core.Interfaces;

namespace TradingApp.Controllers
{
    public class BalanceController : ControllerBase
    {
        private readonly IBalanceService _balanceService;

        private ILogger<BalanceController> _logger;
        public BalanceController(IBalanceService balanceService, ILogger<BalanceController> logger) 
        {
            _balanceService = balanceService;

            _logger = logger;
        }

        

        [HttpGet]
        public async Task<IActionResult> Balance()
        {
            decimal balance = await _balanceService.GetUserBalanceAsync(LoggedUserId);
            return View(model:balance.ToString("f2"));
        }


        [HttpPost]
        public async Task<IActionResult> IncreaseBalance(decimal increasement)
        {
            try
            {
                await _balanceService.IncreaseUserBalanceAsync(userId: LoggedUserId, increasement: increasement);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occured while attempting to increase the user balance!");

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
        public async Task<IActionResult> DecreaseBalance(decimal decreasement)
        {
            try
            {
                await _balanceService.DecreaseUserBalanceAsync(userId: LoggedUserId, decreasement: decreasement);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occured while attempting to decrease the user balance!");

                TempData["title"] = "Error";
                TempData["message"] = $"An error occurred while attempting to transfer money from your balance to your bank account!";
                return RedirectToAction(nameof(Message));
            }

            decimal balance = await _balanceService.GetUserBalanceAsync(LoggedUserId);
            TempData["title"] = "Success";
            TempData["message"] = $"Money from your balance were transfered successfully to your bank account! Your current balance is {balance.ToString("f2")}";
            return RedirectToAction(nameof(Message));
        }
    }
}
