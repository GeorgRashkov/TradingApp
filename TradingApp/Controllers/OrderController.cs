using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TradingApp.Common;
using TradingApp.Data;
using TradingApp.Data.Models;
using TradingApp.Services;
using TradingApp.ViewModels.Order;
using static TradingApp.Common.EntityValidation;

namespace TradingApp.Controllers
{
    public class OrderController : Controller
    {

        private CrudDb _crudDb;

        private int _productsMaxActiveSellOrdersPerUser = 3;
        private int _productMaxActiveSellOrdersPerUser = 2;

        public OrderController(ApplicationDbContext context)
        {
            _crudDb = new CrudDb(context);
        }

        private string LoggedUserId
        {
            get { return User.FindFirst(ClaimTypes.NameIdentifier)?.Value; }
        }

        private string Referer
        {
            get { return Request.Headers["Referer"].ToString(); }
        }


        [HttpPost]
        public async Task<IActionResult> CreateSellOrder(Guid productId, int ordersCount)//this method will show the confirmation page when the user presses the button for creating an order
        {
            ProductFilter filter = new ProductFilter()
            {
                PorductId = productId,
                ProductStatus = Data.Enums.ProductStatus.approved,
            };

            //get the approved product based on it's Id
            var product = await _crudDb.GetProductAsync(productFilter: filter,
               selector: p => new
               {
                   Id = p.Id,
                   Name = p.Name,
                   CreatorId = p.CreatorId,
                   activeSellOrdersCount = p.SellOrders.Where(so => so.Status == Data.Enums.SellOrderStatus.active).Count(),
               });

            //execute this code if no approved product has the specified id
            if(product is null)
            {
                return NotFound();
            }

            //execute this code if the logged user is not creator
            if (product.CreatorId != LoggedUserId)
            {
                return Forbid();
            }

            //execute this code if the active sell orders of the product are above the max number of total sale orders per product
            if (product.activeSellOrdersCount >= _productMaxActiveSellOrdersPerUser)
            {
                TempData["title"] = "Not allowed";
                TempData["message"] = $"You cannot create sell orders for product {product.Name} because the product has reached the maximum number of active sale orders.";
                return RedirectToAction(nameof(Message));
            }

            //execute this code if the active sell orders of the user are above the max number of total sale orders per user
            int currentUserActiveSellOrdersCount = await _crudDb.GetSellOrdersCountAsync(userId: product.CreatorId);
            if (currentUserActiveSellOrdersCount >= _productsMaxActiveSellOrdersPerUser)
            {
                TempData["title"] = "Not allowed";
                TempData["message"] = $"You cannot create sell orders because you reached the maximum number of active sale orders.";
                return RedirectToAction(nameof(Message));
            }

            //make sure the created orders are 1 or more and that their count is not above the max number of total sale orders per product nor above the max number of total sale orders per user
            ordersCount = FitOrdersCountInBoundaries(ordersCount: ordersCount, productActiveSellOrdersCount: product.activeSellOrdersCount, userActiveSellOrdersCount: currentUserActiveSellOrdersCount);

            //create the order view model
            OrderViewModel order = new OrderViewModel()
            {
                Message = $"You are about to create {ordersCount} sell orders of the product {product.Name}",
                ProductId = product.Id,
                OrdersCount = ordersCount,
            };

            //return a confirmation view
            TempData["ReturnUrl"] = Referer;
            return View(order);
        }

        private int FitOrdersCountInBoundaries(int ordersCount, int productActiveSellOrdersCount, int userActiveSellOrdersCount)
        {
            if (ordersCount < 1)
            { ordersCount = 1; }

            //make sure the count of the created orders are not above the max number of total sale orders per product nor above the max number of total sale orders per user
            if (ordersCount + productActiveSellOrdersCount > _productMaxActiveSellOrdersPerUser)
            { ordersCount = _productMaxActiveSellOrdersPerUser - productActiveSellOrdersCount; }
            if (ordersCount + userActiveSellOrdersCount > _productsMaxActiveSellOrdersPerUser)
            { ordersCount = _productsMaxActiveSellOrdersPerUser - userActiveSellOrdersCount; }

            return ordersCount;
        }

        [HttpPost]
        public async Task<IActionResult> CreateSellOrder_execute(Guid productId, int ordersCount)//this method will create the order when the user confirms the creation
        {
            ProductFilter filter = new ProductFilter()
            {
                PorductId = productId,
                ProductStatus = Data.Enums.ProductStatus.approved,
            };

            //get the approved product based on it's Id
            var product = await _crudDb.GetProductAsync(productFilter: filter,
               selector: p => new
               {
                   Id = p.Id,
                   Name = p.Name,
                   CreatorId = p.CreatorId,
                   activeSellOrdersCount = p.SellOrders.Where(so => so.Status == Data.Enums.SellOrderStatus.active).Count(),
               });

            //execute this code if no approved product has the specified id
            if (product is null)
            {
                return NotFound();
            }

            //execute this code if the logged user is not creator
            if (product.CreatorId != LoggedUserId)
            {
                return Forbid();
            }

            //execute this code if the active sell orders of the product are above the max number of total sale orders per product
            if (product.activeSellOrdersCount >= _productMaxActiveSellOrdersPerUser)
            {
                TempData["title"] = "Not allowed";
                TempData["message"] = $"You cannot create sell orders for product {product.Name} because the product has reached the maximum number of active sale orders.";
                return RedirectToAction(nameof(Message));
            }

            //execute this code if the active sell orders of the user are above the max number of total sale orders per user
            int currentUserActiveSellOrdersCount = await _crudDb.GetSellOrdersCountAsync(userId: product.CreatorId);
            if (currentUserActiveSellOrdersCount >= _productsMaxActiveSellOrdersPerUser)
            {
                TempData["title"] = "Not allowed";
                TempData["message"] = $"You cannot create sell orders because you reached the maximum number of active sale orders.";
                return RedirectToAction(nameof(Message));
            }

            //make sure the created orders are 1 or more and that their count is not above the max number of total sale orders per product nor above the max number of total sale orders per user
            ordersCount = FitOrdersCountInBoundaries(ordersCount: ordersCount, productActiveSellOrdersCount: product.activeSellOrdersCount, userActiveSellOrdersCount: currentUserActiveSellOrdersCount);

            //create the order
            SellOrder sellOrder = new SellOrder()
            {
                Status = Data.Enums.SellOrderStatus.active,
                CreatedAt = DateTime.UtcNow,
                CreatorId = product.CreatorId,
                ProductId = product.Id,
            };

            //save the orders to the DB
            await _crudDb.CreateSellOrders(sellOrder: sellOrder, ordersCount:ordersCount);

            //return a success message page
            TempData["title"] = "Success";
            TempData["message"] = $"You successfully created {ordersCount} sell order/s of the product {product.Name}.";
            return RedirectToAction(nameof(Message));
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



        public IActionResult Message()
        {
            return View();
        }
    }
}
