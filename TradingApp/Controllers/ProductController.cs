using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TradingApp.Data;
using TradingApp.GCommon;
using TradingApp.Services.Core.Interfaces;
using TradingApp.ViewModels.Product;


namespace TradingApp.Controllers
{
    public class ProductController : ControllerBase
    {                    
        private IProductService _productService;

        public ProductController(ApplicationDbContext context, IProductService productService)
        {          
            _productService = productService;
        }
       
       
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Products(int pageIndex)
        {
            IEnumerable<ProductsViewModel> products = await _productService.GetApprovedProductsWithActiveSellOrdersAsync(pageIndex: pageIndex);
            if (products.Count() == 0)
            { return View(model: null); }

            ViewData["page"] = _productService.ProductPageIndex;

            return View(model: products);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Product(Guid productId)
        {
            ProductViewModel? product = await _productService.GetDetailsForApprovedProductWithActiveSellOrdersAsync(productId: productId);

            if (product == null)
            { return NotFound(); }


            return View(model: product);
        }



        [HttpGet]        
        public async Task<IActionResult> MyProducts(int pageIndex)
        {
            IEnumerable<MyProductsViewModel> products = await _productService.GetProductsCreatedByUserAsync(pageIndex: pageIndex, userId: LoggedUserId);
            if (products.Count() == 0)
            { return View(model: null); }

            ViewData["page"] = _productService.ProductPageIndex;

            return View(model: products);
        }


        [HttpGet]        
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
