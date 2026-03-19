using Microsoft.AspNetCore.Mvc;
using TradingApp.Services.Core.Interfaces;
using TradingApp.ViewModels.Product;
//Area `Admin`
namespace TradingApp.Areas.Admin.Controllers
{
    public class ProductController : ControllerBase
    {
        IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Products(int pageIndex)
        {
            IEnumerable<ProductsViewModel> products = await _productService.GetProductsAsync(pageIndex: pageIndex);
            if (products.Count() == 0)
            { return View(model: null); }

            ViewData["page"] = _productService.ProductPageIndex;
            ViewData["action"] = "Products";

            return View(model: products);
        }
    }
}
