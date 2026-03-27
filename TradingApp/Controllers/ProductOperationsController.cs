using Microsoft.AspNetCore.Mvc;
using TradingApp.Data;
using TradingApp.GCommon;
using TradingApp.GCommon.ErrorCodes;
using TradingApp.Services.Core.Interfaces;
using TradingApp.ViewModels.InputProduct;


namespace TradingApp.Controllers
{
    public class ProductOperationsController : ControllerBase
    {
        private IProductBoolsService _productBoolsService; 
        private IProductOperationsService _productOperationsService;
        private IProductFileService _productFileService;
        private IProductService _productService;
        private IUserService _userService;

        private ILogger<InvoiceController> _logger;
        public ProductOperationsController(ApplicationDbContext context, IProductBoolsService productBoolsService, IProductOperationsService productOperationsService, IProductFileService productFileService, IProductService productService, IUserService userService, ILogger<InvoiceController> logger)
        {
            _productBoolsService = productBoolsService;
            _productOperationsService = productOperationsService;
            _productFileService = productFileService;
            _productService = productService;
            _userService = userService;

            _logger = logger;
        }       

       

        [HttpGet]
        public IActionResult CreateProduct()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromForm] CreatedProductModel createdProductModel)
        {

            if (ModelState.IsValid == false)
            {
                return View(createdProductModel);
            }
            createdProductModel.ProductName = createdProductModel.ProductName.Trim();
            
            try
            {                
                Result result = await _productOperationsService.AddProductAsync(name:createdProductModel.ProductName,description:createdProductModel.Description, price:createdProductModel.Price, creatorId: LoggedUserId);
                if (result.Success == false)
                {
                    string errorMessage = GetCreateProductErrorMessage(result.ErrorCode);
                    ModelState.AddModelError(key:string.Empty, errorMessage: errorMessage);
                    return View(createdProductModel);
                }
                
                await _productFileService.SaveProductInFolderAsync(product: createdProductModel, creatorName: LoggedUserUsername);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occured while attempting to create a product!");

                TempData["title"] = "Error";
                TempData["message"] = "An error occured while attempting to save you product! Please try again later.";
                return RedirectToAction(nameof(Message));
            }

            TempData["title"] = "Success";
            TempData["message"] = $"The 3D model {createdProductModel.ProductName} was created succesfully.";
            return RedirectToAction(nameof(Message));

        }

        //this method is formed based on the logic of the service method for creating a product 
        //it's purpose is to provide a proper error message which will be shown to the user based on the error code
        private string GetCreateProductErrorMessage(string errorCode)
        {
            
            string errorMessage = errorCode switch
            {
                string code when code == UserErrorCodes.UserNotFound =>
                    "You have to login in order to create a product.",

                string code when code == ProductErrorCodes.ProductWithSameNameAlreadyExists =>
                    "A product with the same name already exists!\n Consider using unique name before creating the product.",

                _ => "Something went wrong."
            };

            return errorMessage;
        }






        [HttpGet]
        public async Task<IActionResult> UpdateProduct(Guid productId)
        {
            bool doesProductCreatedByCreatorExist = await _productBoolsService.DoesProductCreatedByUserExistAsync(userId: LoggedUserId, productId: productId);

            if (doesProductCreatedByCreatorExist == false)
            {
                return NotFound();
            }

            int productActiveSellOrdersCount = await _productService.GetProductActiveSellOrdersCountAsync(productId: productId);

            if (productActiveSellOrdersCount > 0)
            {
                TempData["title"] = "Not allowed";
                TempData["message"] = "The product you are trying to update has at least one sell order! Make sure you cancel all sell orders of the product before editing it!";
                return RedirectToAction(nameof(Message));
            }
            UpdatedProductModel product = await _productService.GetUpdatedProductModelAsync(productId);

            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProduct([FromForm] UpdatedProductModel updatedProductModel)
        {
            //model state validation
            if (ModelState.IsValid == false)
            {
                return View(updatedProductModel);
            }

            
            string newProductName = updatedProductModel.ProductName.Trim();
            string oldProductName = await _productService.GetProductNameAsync(productId: updatedProductModel.Id);
              
            //the product name must change due to browser chaching (Browsers aggressively cache static files (especially images) when the URL does not change and as a result the browser will get the old pictures) 
            updatedProductModel.ProductName = (newProductName == oldProductName) ? newProductName + "_" : newProductName;

            try
            {
                Result result = await _productOperationsService.UpdateProductAsync(id: updatedProductModel.Id, name: updatedProductModel.ProductName, description: updatedProductModel.Description, price: updatedProductModel.Price, creatorId: LoggedUserId);
                if(result.Success == false)
                {
                    string errorMessage = GetUpdateProductErrorMessage(result.ErrorCode);
                    ModelState.AddModelError(key:string.Empty, errorMessage: errorMessage);
                    return View(updatedProductModel);
                }
                await _productFileService.UpdateProductInFolderAsync(product: updatedProductModel, creatorName: LoggedUserUsername, oldProductName: oldProductName);               
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occured while attempting to update a product!");

                TempData["title"] = "Error";
                TempData["message"] = "An error occured while attempting to apply the product changes! Please try again later.";
                return RedirectToAction(nameof(Message));
            }

            TempData["title"] = "Success";
            TempData["message"] = $"The 3D model {updatedProductModel.ProductName} was updated succesfully.";
            return RedirectToAction(nameof(Message));
        }

        //this method is formed based on the logic of the service method for updating a product 
        //it's purpose is to provide a proper error message which will be shown to the user based on the error code
        private string GetUpdateProductErrorMessage(string errorCode)
        {

            string errorMessage = errorCode switch
            {
                string code when code == ProductErrorCodes.ProductNotFound =>
                    "The product you are trying to edit could not be found!",

                string code when code == ProductErrorCodes.ProductInvalidCreator =>
                "You are not allowed to edit products created by other users!",

                string code when code == ProductErrorCodes.ProductHasActiveSaleOrders =>
                "The product you are trying to update has at least one sell order! Make sure you cancel all sell orders of the product before editing it!",

                _ => "Something went wrong."
            };

            return errorMessage;
        }





        [HttpGet]
        public async Task<IActionResult> DeleteProduct(Guid productId)
        {
            //product existance validation
            bool doesProductCreatedByUserExist = await _productBoolsService.DoesProductCreatedByUserExistAsync(userId: LoggedUserId, productId: productId);

            if (doesProductCreatedByUserExist == false)
            {
                return NotFound();
            }

            DeletedProductModel deletedProductModel = await _productService.GetDeletedProductModelAsync(productId:productId);
                      

            if (string.IsNullOrEmpty(Referer) == false)
            { TempData["ReturnUrl"] = Referer; }

            return View(deletedProductModel);
        }
        
        [HttpPost]
        public async Task<IActionResult> DeleteProduct_execute(Guid productId)
        {
            
            string productName = await _productService.GetProductNameAsync(productId: productId);
            string creatorName = await _userService.GetCreatorNameOfProductAsync(productId: productId);
            try
            {
                //attempting to delete the product in the database
                Result result = await _productOperationsService.DeleteProductAsync(id:productId, creatorId:LoggedUserId);

                if (result.Success == false)
                {                                       
                    TempData["title"] = "Error";
                    TempData["message"] = GetDeleteProductErrorMessage(result.ErrorCode);
                    return View(viewName: nameof(Message));
                }    
                //attempting to delete the product folder and it's content (the 6 images and the 3D model file)
                _productFileService.DeleteProductFolder(creatorName: creatorName, productName: productName);
                                
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occured while attempting to delete a product!");

                TempData["title"] = "Error";
                TempData["message"] = $"An error occured while attempting to delete your product! Please try again later.";
                return RedirectToAction(nameof(Message));
            }

            TempData["title"] = "Success";
            TempData["message"] = $"The 3D model {productName} was deleted successfully.";
            return RedirectToAction(nameof(Message));
        }

        //this method is formed based on the logic of the service method for deleting a product 
        //it's purpose is to provide a proper error message which will be shown to the user based on the error code
        private string GetDeleteProductErrorMessage(string errorCode)
        {

            string errorMessage = errorCode switch
            {
                string code when code == ProductErrorCodes.ProductNotFound =>
                    "The product you are trying to delete could not be found!",

                string code when code == ProductErrorCodes.ProductInvalidCreator =>
                "You are not allowed to delete products created by other users!",

                _ => "Something went wrong."
            };

            return errorMessage;
        }
    }
}
