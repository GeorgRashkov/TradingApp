using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TradingApp.GCommon;
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
        public async Task<IActionResult> Products(int pageIndex)
        {
            IEnumerable<ProductViewModel> products = await _productService.GetApprovedProductsWithActiveSellOrdersAsync(pageIndex: pageIndex);
            if (products.Count() == 0)
            { return View(model: null); }

            ViewData["page"] = _productService.ProductPageIndex;
            ViewData["action"] = "Products";

            return View(model: products);
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
        public async Task<IActionResult> SuggestedProducts_for_OrderRequest(int pageIndex, string? orderRequestId)
        {
            Guid? lastUsed_OrderRequestId = Get_LastUsed_OrderRequestId(orderRequestId: orderRequestId);
            if(lastUsed_OrderRequestId == null) 
            { return View(viewName: nameof(Products), model: null); }
            
            IEnumerable<ProductViewModel> products = await _productService.Get_SuggestedApprovedProductsWithActiveSellOrders_for_OrderRequest_Async(pageIndex: pageIndex, orderRequestId: (Guid)lastUsed_OrderRequestId);
            if (products.Count() == 0)
            { return View(viewName: nameof(Products), model: null); }

            ViewData["page"] = _productService.ProductPageIndex;
            ViewData["action"] = "SuggestedProducts_for_OrderRequest";

            return View(viewName: nameof(Products), model: products);
        }

        private Guid? Get_LastUsed_OrderRequestId(string? orderRequestId)
        {
            if (orderRequestId.IsNullOrEmpty() == false)
            {
                bool is_OrderRequestId_validGuid = Guid.TryParse(orderRequestId, out Guid orderRequestIdAsGuid);
                if (is_OrderRequestId_validGuid == true)
                { TempData["_lastUsed_OrderRequestId"] = orderRequestIdAsGuid; }
            }
            return (Guid?)TempData.Peek("_lastUsed_OrderRequestId");
        }
    }
}
