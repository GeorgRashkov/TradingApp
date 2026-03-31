using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TradingApp.GCommon;
using TradingApp.GCommon.Filters;
using TradingApp.Services.Core.Interfaces;
using TradingApp.ViewModels.Product;


namespace TradingApp.Controllers
{
    public class ProductController : ControllerBase
    {                    
        private IProductService _productService;   
        private IUserService _userService;

        public ProductController(IProductService productService, IUserService userService)
        {          
            _productService = productService;
            _userService = userService;
        }
       
       
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Products(int pageIndex, ProductFilter? productFilter)
        {
            if(ModelState.IsValid == false) 
            { productFilter = null; }

            
            IEnumerable<ProductViewModel> products = await _productService.GetApprovedProductsWithActiveSellOrdersAsync(pageIndex: pageIndex, productFilter);
            if (products.Count() == 0)
            { return View(model: null); }

            ProductsViewModel productsViewModel = new ProductsViewModel
            {
                Products = products,
                ProductFilter = productFilter,
                PageIndex = _productService.ProductPageIndex
            };           

            return View(model: productsViewModel);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Product(Guid productId)
        {
            ProductDetailsViewModel? product = await _productService.GetDetailsForApprovedProductWithActiveSellOrdersAsync(productId: productId);

            if (product == null)
            { return NotFound(); }


            return View(model: product);
        }



        [HttpGet]        
        public async Task<IActionResult> MyProducts(int pageIndex)
        {
            IEnumerable<MyProductViewModel> products = await _productService.GetProductsCreatedByUserAsync(pageIndex: pageIndex, userId: LoggedUserId);
            if (products.Count() == 0)
            { return View(model: null); }

            ViewData["page"] = _productService.ProductPageIndex;

            return View(model: products);
        }


        [HttpGet]        
        public async Task<IActionResult> MyProduct(Guid productId)
        {
            MyProductDetailsViewModel? product = await _productService.GetDetailsForProductAsync(productId: productId);

            if (product == null)
            { return NotFound(); }

            if (LoggedUserUsername != product.CreatorName)
            { return Unauthorized(); }

            int loggedUserActiveSellOrdersCount = await _userService.GetUserActiveSellOrdersCountAsync(userId: LoggedUserId);

            ViewData["currentUserMaxSellOrdersCountReached"] = loggedUserActiveSellOrdersCount >= ApplicationConstants.UserMaxActiveSellOrders ? true : false;
            ViewData["currentProductMaxSellOrdersCountReached"] = product.ActiveSellOrdersCount >= ApplicationConstants.ProductMaxActiveSellOrders ? true : false;

            return View(model: product);
        }






        [HttpGet]
        public async Task<IActionResult> SuggestedProducts_for_OrderRequest(Guid orderRequestId) 
        {
            return RedirectToAction(actionName: nameof(Products), routeValues: new { pageIndex = 0, OrderRequestId = orderRequestId });
        }
    }
}
