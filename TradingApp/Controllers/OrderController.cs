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
        //this method will show the confirmation page when the user presses the button for creating an order
        public async Task<IActionResult> CreateSellOrder(Guid productId, int ordersCount)
        {
            ProductFilter filter = new ProductFilter()
            {
                PorductId = productId,
                ProductStatus = Data.Enums.ProductStatus.approved,
            };

            //get the approved product based on its Id
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
            ordersCount = FitOrdersCreationCountInBoundaries(ordersCount: ordersCount, productActiveSellOrdersCount: product.activeSellOrdersCount, userActiveSellOrdersCount: currentUserActiveSellOrdersCount);

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

        private int FitOrdersCreationCountInBoundaries(int ordersCount, int productActiveSellOrdersCount, int userActiveSellOrdersCount)
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
        //this method will create the order when the user confirms the creation
        public async Task<IActionResult> CreateSellOrder_execute(Guid productId, int ordersCount)
        {
            ProductFilter filter = new ProductFilter()
            {
                PorductId = productId,
                ProductStatus = Data.Enums.ProductStatus.approved,
            };

            //get the approved product based on its Id
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
            ordersCount = FitOrdersCreationCountInBoundaries(ordersCount: ordersCount, productActiveSellOrdersCount: product.activeSellOrdersCount, userActiveSellOrdersCount: currentUserActiveSellOrdersCount);

            //create the order
            SellOrder sellOrder = new SellOrder()
            {
                Status = Data.Enums.SellOrderStatus.active,
                CreatedAt = DateTime.UtcNow,
                CreatorId = product.CreatorId,
                ProductId = product.Id,
            };

            //save the orders to the DB
            await _crudDb.CreateSellOrders(sellOrder: sellOrder, ordersCount: ordersCount);

            //return a success message page
            TempData["title"] = "Success";
            TempData["message"] = $"You successfully created {ordersCount} sell order/s of the product {product.Name}.";
            return RedirectToAction(nameof(Message));
        }









        [HttpPost]
        //this method will show the confirmation page when the user presses the button for cancelling an order
        public async Task<IActionResult> CancelSellOrder(Guid productId, int ordersCount)
        {
            ProductFilter filter = new ProductFilter()
            {
                PorductId = productId,
                ProductStatus = Data.Enums.ProductStatus.approved,
            };

            //get the approved product based on its Id
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

            //execute this code if the product has no active sell orders
            if (product.activeSellOrdersCount < 1)
            {
                TempData["title"] = "Not allowed";
                TempData["message"] = $"The product {product.Name} has no active sell orders to cancel!";
                return RedirectToAction(nameof(Message));
            }

            //make the count of the orders (which are about to be cancelled) to be positive and also equal or below the count of the active sell orders of the product
            ordersCount = Math.Max(1, ordersCount);
            ordersCount = Math.Min(ordersCount, product.activeSellOrdersCount);

            //create the order view model
            OrderViewModel order = new OrderViewModel()
            {
                Message = $"You are about to cancel {ordersCount} sell orders of the product {product.Name}",
                ProductId = product.Id,
                OrdersCount = ordersCount,
            };

            //return a confirmation view
            TempData["ReturnUrl"] = Referer;
            return View(order);
        }

        [HttpPost]
        //this method will cancel the order when the user confirms the cancelation
        public async Task<IActionResult> CancelSellOrder_execute(Guid productId, int ordersCount)
        {
            ProductFilter filter = new ProductFilter()
            {
                PorductId = productId,
                ProductStatus = Data.Enums.ProductStatus.approved,
            };

            //get the approved product based on its Id
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

            //execute this code if the product has no active sell orders
            if (product.activeSellOrdersCount < 1)
            {
                TempData["title"] = "Not allowed";
                TempData["message"] = $"The product {product.Name} has no active sell orders to cancel!";
                return RedirectToAction(nameof(Message));
            }

            //make the count of the orders (which are about to be cancelled) to be positive and also equal or below the count of the active sell orders of the product
            ordersCount = Math.Max(1, ordersCount);
            ordersCount = Math.Min(ordersCount, product.activeSellOrdersCount);

            //set the status of the orders to cancelled
            await _crudDb.CancelSellOrdersAsync(product.Id, ordersCount);

            //return a success message page
            TempData["title"] = "Success";
            TempData["message"] = $"You successfully cancelled {ordersCount} sell order/s of the product {product.Name}.";
            return RedirectToAction(nameof(Message));
        }







        [HttpGet]
        public async Task<IActionResult> BuySellOrder(Guid productId)
        {
            ProductFilter filter = new ProductFilter()
            {
                PorductId = productId,
                ProductStatus = Data.Enums.ProductStatus.approved,
            };

            //get the approved product based on its Id
            var product = await _crudDb.GetProductAsync(productFilter: filter,
               selector: p => new
               {
                   Id = p.Id,
                   Name = p.Name,
                   CreatorId = p.CreatorId,
                   Price = p.Price,
                   activeSellOrdersCount = p.SellOrders.Where(so => so.Status == Data.Enums.SellOrderStatus.active).Count(),
               });

            //execute this code if no approved product has the specified id
            if (product is null)
            {
                return NotFound();
            }

            //execute this code if the logged user is the creator
            if (product.CreatorId == LoggedUserId)
            {
                TempData["title"] = "Not allowed";
                TempData["message"] = $"You are not allowed to buy your own products!";
                return RedirectToAction(nameof(Message));
            }

            //execute this code if the product has no active sell orders
            if (product.activeSellOrdersCount < 1)
            {
                TempData["title"] = "Not allowed";
                TempData["message"] = $"You cannot buy the product {product.Name} because it has no active sell orders!";
                return RedirectToAction(nameof(Message));
            }

            decimal userBalance = await _crudDb.GetUserBalanceAsync(LoggedUserId);
            //execute this code if the user balance is below the product price
            if (userBalance < product.Price)
            {
                TempData["title"] = "Not allowed";
                TempData["message"] = $"You do not have enough money in your balance to buy the product {product.Name}!";
                return RedirectToAction(nameof(Message));
            }

            //create the order view model
            OrderViewModel order = new OrderViewModel()
            {
                Message = $"You are about to purchase the product {product.Name}",
                ProductId = product.Id
            };

            //return a confirmation view
            TempData["ReturnUrl"] = Referer;
            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> BuySellOrder_execute(Guid productId)
        {
            ProductFilter filter = new ProductFilter()
            {
                PorductId = productId,
                ProductStatus = Data.Enums.ProductStatus.approved,
            };

            //get the approved product based on its Id
            var product = await _crudDb.GetProductAsync(productFilter: filter,
               selector: p => new
               {
                   Id = p.Id,
                   Name = p.Name,
                   CreatorId = p.CreatorId,
                   Price = p.Price,
                   activeSellOrdersCount = p.SellOrders.Where(so => so.Status == Data.Enums.SellOrderStatus.active).Count(),
               });

            //execute this code if no approved product has the specified id
            if (product is null)
            {
                return NotFound();
            }

            //execute this code if the logged user is the creator
            if (product.CreatorId == LoggedUserId)
            {
                TempData["title"] = "Not allowed";
                TempData["message"] = $"You are not allowed to buy your own products!";
                return RedirectToAction(nameof(Message));
            }

            //execute this code if the product has no active sell orders
            if (product.activeSellOrdersCount < 1)
            {
                TempData["title"] = "Not allowed";
                TempData["message"] = $"You cannot buy the product {product.Name} because it has no active sell orders!";
                return RedirectToAction(nameof(Message));
            }


            decimal userBalance = await _crudDb.GetUserBalanceAsync(LoggedUserId);
            //execute this code if the user balance is below the product price
            if (userBalance < product.Price)
            {
                TempData["title"] = "Not allowed";
                TempData["message"] = $"You do not have enough money in your balance to buy the product {product.Name}!";
                return RedirectToAction(nameof(Message));
            }

            //the sell order is bough and the changes are applied to the DB (executes only when when the sell order can be bought)
            await _crudDb.BuySellOrderAsync(productId: productId, buyerId: LoggedUserId);

            //return a success message page
            TempData["title"] = "Success";
            TempData["message"] = $"You successfully purchased the product {product.Name}.";
            return RedirectToAction(nameof(Message));
        }



        public IActionResult Message()
        {
            return View();
        }
    }
}
