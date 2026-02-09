using Microsoft.AspNetCore.Mvc;

namespace TradingApp.Controllers
{
    public class OrderController : Controller
    {
        [HttpGet]
        public IActionResult CreateSellOrder(Guid productId, int ordersCount)
        {
            return View();
        }
        [HttpPost]
        public IActionResult CreateSellOrder_execute(Guid productId, int ordersCount)
        {
            return View();
        }



        [HttpGet]
        public IActionResult CancelSellOrder(Guid productId, int ordersCount)
        {
            return View();
        }

        [HttpPost]
        public IActionResult CancelSellOrder_execute(Guid productId, int ordersCount)
        {
            return View();
        }




        [HttpGet]
        public IActionResult BuySellOrder(Guid productId)
        {
            return View();
        }

        [HttpPost]
        public IActionResult BuySellOrder_execute(Guid productId)
        {
            return View();
        }
    }
}
