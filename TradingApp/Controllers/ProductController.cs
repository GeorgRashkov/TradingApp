using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Security.Claims;
using TradingApp.Data;
using TradingApp.Data.Helpers;
using TradingApp.GCommon;
using TradingApp.Services.Core;
using TradingApp.Services.Core.Interfaces;
using TradingApp.ViewModels.Product;


namespace TradingApp.Controllers
{
    public class ProductController : Controller
    {                    
        private IProductService _productService;

        public ProductController(ApplicationDbContext context, IProductService productService)
        {          
            _productService = productService;
        }
       
        //this is the Id of the currently logged user; if the user is not logged the value will be null 
        private string LoggedUserId
        {
            get { return User.FindFirst(ClaimTypes.NameIdentifier)?.Value; }
        }

        //this is the username of the currently logged user; if the user is not logged the value will be null 
        private string LoggedUserUsername
        {
            get { return User.Identity?.Name; }
        }

        [HttpGet]
        public async Task<IActionResult> Products(int pageIndex)
        {
            IEnumerable<ProductsViewModel> products = await _productService.GetApprovedProductsWithActiveSellOrdersAsync(pageIndex: pageIndex);
            if (products.Count() == 0)
            { return View(model: null); }

            ViewData["page"] = _productService.ProductPageIndex;

            return View(model: products);
        }

        [HttpGet]
        public async Task<IActionResult> Product(Guid productId)
        {
            ProductViewModel? product = await _productService.GetDetailsForApprovedProductWithActiveSellOrdersAsync(productId: productId);

            if (product == null)
            { return NotFound(); }


            return View(model: product);
        }



        [HttpGet]
        [Authorize]
        public async Task<IActionResult> MyProducts(int pageIndex)
        {
            IEnumerable<MyProductsViewModel> products = await _productService.GetProductsCreatedByUserAsync(pageIndex: pageIndex, userId: LoggedUserId);
            if (products.Count() == 0)
            { return View(model: null); }

            ViewData["page"] = _productService.ProductPageIndex;

            return View(model: products);
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> MyProduct(Guid productId)
        {
            MyProductViewModel? product = await _productService.GetDetailsForProductAsync(productId: productId);

            if (product == null)
            { return NotFound(); }

            if (LoggedUserUsername != product.CreatorName)
            { return Unauthorized(); }

            int loggedUserActiveSellOrdersCount = await _productService.GetUserActiveSellOrdersCountAsync(userId: LoggedUserId);

            ViewData["currentUserMaxSellOrdersCountReached"] = loggedUserActiveSellOrdersCount >= ApplicationConstants.UserMaxActiveSellOrders ? true : false;
            ViewData["currentProductMaxSellOrdersCountReached"] = product.ActiveSellOrdersCount >= ApplicationConstants.ProductMaxActiveSellOrders ? true : false;

            return View(model: product);
        }


    }
}
