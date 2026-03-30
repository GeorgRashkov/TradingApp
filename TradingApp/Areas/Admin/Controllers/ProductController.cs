//Area `Admin`

using Microsoft.AspNetCore.Mvc;
using TradingApp.GCommon;
using TradingApp.GCommon.ErrorCodes;
using TradingApp.Services.Core.Interfaces;
using TradingApp.ViewModels.InputProduct;
using TradingApp.ViewModels.Product;

namespace TradingApp.Areas.Admin.Controllers
{
    public class ProductController : ControllerBase
    {
        private IUserService _userService;
        private IProductService _productService;
        private IProductOperationsService _productOperationsService;
        private IProductFileService _productFileService;

        private ILogger<ProductController> _logger;
        public ProductController(IUserService userService, IProductService productService, IProductOperationsService productOperationsService, IProductFileService productFileService, ILogger <ProductController> logger)
        {
            _userService = userService;
            _productService = productService;
            _productOperationsService = productOperationsService;
            _productFileService = productFileService;
            
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Products(int pageIndex)
        {
            IEnumerable<ProductViewModel> products = await _productService.GetProductsAsync(pageIndex: pageIndex);
            if (products.Count() == 0)
            { return View(model: null); }

            ViewData["page"] = _productService.ProductPageIndex;
            ViewData["action"] = "Products";

            return View(model: products);
        }


        [HttpGet]        
        public async Task<IActionResult> Product(Guid productId)
        {
            MyProductDetailsViewModel? product = await _productService.GetDetailsForProductAsync(productId: productId);

            if (product == null)
            { return NotFound(); }           

            string userId = (await _userService.GetUserIdAsync(userName:product.CreatorName))!;
            int loggedUserActiveSellOrdersCount = await _userService.GetUserActiveSellOrdersCountAsync(userId: userId);

            return View(model: product);
        }


        [HttpGet]
        public IActionResult Download3dModelFile(string productName, string productCreatorName)
        {
            
            byte[] bytes;
            try
            {
                bytes = _productFileService.Get3dModelFileBytes(productCreatorName: productCreatorName, productName: productName);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occured while attemping to download a 3D model file!");

                TempData["title"] = "Error";
                TempData["message"] = "An error occured while attempting to download the file!";
                return View(viewName: "Message");
            }

            return File(bytes, "image/jpg", productName + ".jpg");
        }



        [HttpGet]
        public async Task<IActionResult> ManageProduct(Guid productId) 
        {
            ManagedProductModel? managedProductModel = await _productService.GetManagedProductModelAsync(productId: productId);
            
            if(managedProductModel == null) 
            { return NotFound(); }

            return View(model: managedProductModel);
        }

        //this method is formed based on the logic of the service method for changing the status of a product 
        //its purpose is to provide a proper error message which will be shown to the user based on the error code
        private string Get_ManageProduct_ErrorMessage(string errorCode)
        {

            string errorMessage = errorCode switch
            {
                string code when code == ProductErrorCodes.ProductNotFound =>
                    "The product you are trying to manage could not be found!",

                string code when code == ProductErrorCodes.ProductInvalidStatus =>
                    "The product status was not changed!",

                _ => "Something went wrong."
            };

            return errorMessage;
        }

        [HttpPost]
        public async Task<IActionResult> ManageProduct_execute([FromForm]ManagedProductModel managedProductModel) 
        {
            //model state validation
            if (ModelState.IsValid == false)
            {
                return View(viewName:nameof(ManageProduct), model: managedProductModel);
            }

            Result result;
            try 
            {
                result = await _productOperationsService.ChangeProductStatusAsync(id: managedProductModel.Id, productStatus: managedProductModel.Status);
            }
            catch(Exception e) 
            {
                _logger.LogError(e, "An error occured while attempting to change the product status!");

                TempData["title"] = "Error";
                TempData["message"] = "An error occured while attempting to apply the product changes! Please try again later.";
                return RedirectToAction(nameof(Message));
            }

            if (result.Success == false)
            {
                string errorMessage = Get_ManageProduct_ErrorMessage(result.ErrorCode);
                TempData["title"] = "Error";
                TempData["message"] = errorMessage;
                return RedirectToAction(nameof(Message));
            }

            TempData["title"] = "Success";
            TempData["message"] = $"The product status was updated succesfully.";
            return RedirectToAction(nameof(Message));
        }
    }
}
