//Area `Admin`

using Microsoft.AspNetCore.Mvc;
using TradingApp.Services.Core.Interfaces;
using TradingApp.ViewModels.Product;

namespace TradingApp.Areas.Admin.Controllers
{
    public class ProductController : ControllerBase
    {
        private IProductService _productService;
        private IUserService _userService;
        private IProductFileService _productFileService;
        public ProductController(IProductService productService, IUserService userService, IProductFileService productFileService)
        {
            _productService = productService;
            _userService = userService;
            _productFileService = productFileService;
        }

        [HttpGet]
        public async Task<IActionResult> Products(int pageIndex)
        {
            IEnumerable<ProductsViewModel> products = await _productService.GetProductsAsync(pageIndex: pageIndex);
            if (products.Count() == 0)
            { return View(model: null); }

            ViewData["page"] = _productService.ProductPageIndex;
            ViewData["action"] = "Products";

            return View(model: products);
        }


        [HttpGet]        
        public async Task<IActionResult> Product(Guid productId)
        {
            MyProductViewModel? product = await _productService.GetDetailsForProductAsync(productId: productId);

            if (product == null)
            { return NotFound(); }           

            string userId = (await _userService.GetUserIdAsync(userName:product.CreatorName))!;
            int loggedUserActiveSellOrdersCount = await _userService.GetUserActiveSellOrdersCountAsync(userId: userId);

            return View(model: product);
        }


        [HttpGet]
        public async Task<IActionResult> Download3dModelFile(string productName, string productCreatorName)
        {
            
            byte[] bytes;
            try
            {
                bytes = _productFileService.Get3dModelFileBytes(productCreatorName: productCreatorName, productName: productName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                TempData["title"] = "Error";
                TempData["message"] = "An error occured while attempting to dowload the file!";
                return View(viewName: "Message");
            }

            return File(bytes, "image/jpg", productName + ".jpg");
        }
    }
}
