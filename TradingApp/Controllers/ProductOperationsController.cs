using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TradingApp.Data;
using TradingApp.Data.Helpers;
using TradingApp.Data.Models;
using TradingApp.GCommon;
using TradingApp.Services;
using TradingApp.Services.Core;
using TradingApp.Services.Core.Interfaces;
using TradingApp.ViewModels.InputProduct;


namespace TradingApp.Controllers
{
    public class ProductOperationsController : Controller
    {
        private IProductBoolsService _productBoolsService; 
        private IProductOperationsService _productOperationsService;
        private IProductFileService _productFileService;
        private IProductService _productService;


        
        public ProductOperationsController(ApplicationDbContext context, IProductBoolsService productBoolsService, IProductOperationsService productOperationsService, IProductFileService productFileService, IProductService productService)
        {
            _productBoolsService = productBoolsService;
            _productOperationsService = productOperationsService;
            _productFileService = productFileService;
            _productService = productService;

        }
        private string LoggedUserId
        {
            get { return User.FindFirst(ClaimTypes.NameIdentifier)?.Value; }
        }

        private string LoggedUserUsername
        {
            get { return User.Identity?.Name; }
        }

        private string Referer
        {
            get { return Request.Headers["Referer"].ToString(); }
        }


        [HttpGet]
        [Authorize]
        public IActionResult CreateProduct()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateProduct([FromForm] CreatedProductModel createdProductModel)
        {

            if (ModelState.IsValid == false)
            {
                return View(createdProductModel);
            }
            createdProductModel.ProductName = createdProductModel.ProductName.Trim();
            
            //check whether the user has a product with the same name
            bool doesProductCreatedByCreatorExist = await _productBoolsService.DoesProductCreatedByUserExistAsync(userId: LoggedUserId, productName: createdProductModel.ProductName);
            if (doesProductCreatedByCreatorExist == true)
            {
                ModelState.AddModelError(string.Empty, "A product with the same name already exists!\n Consider using unique name before creating your 3D model file.");
                return View(createdProductModel);
            }
            
            try
            {                
                await _productOperationsService.AddProductAsync(name:createdProductModel.ProductName,description:createdProductModel.Description, price:createdProductModel.Price, creatorId: LoggedUserId);
                await _productFileService.SaveProductInFolderAsync(product: createdProductModel, creatorName: LoggedUserUsername);
            }
            catch (Exception e)
            {
                Console.Write(e.Message.ToString());

                TempData["title"] = "Error";
                TempData["message"] = $"An error occured while attempting to save you product!";
                return RedirectToAction(nameof(Message));
            }

            TempData["title"] = "Success";
            TempData["message"] = $"The 3D model {createdProductModel.ProductName} was created succesfully.";
            return RedirectToAction(nameof(Message));

        }







        [HttpGet]
        [Authorize]
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
                TempData["message"] = $"The product you are trying to update has at least one sell order! Make sure you cancel all sell orders of the product before editing it!";
                return RedirectToAction(nameof(Message));
            }
            UpdatedProductModel product = await _productService.GetUpdatedProductModelAsync(productId);

            return View(product);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateProduct([FromForm] UpdatedProductModel updatedProductModel)
        {
            //model state validation
            if (ModelState.IsValid == false)
            {
                return View(updatedProductModel);
            }

            //product existance validation
            bool doesProductCreatedByCreatorExist = await _productBoolsService.DoesProductCreatedByUserExistAsync(userId: LoggedUserId, productId: updatedProductModel.Id);
            if (doesProductCreatedByCreatorExist == false)
            {
                ModelState.AddModelError(string.Empty, "The product you are trying to edit could not be found!");
                return View(updatedProductModel);
            }

            string newProductName = updatedProductModel.ProductName.Trim();
            string oldProductName = await _productService.GetProductNameAsync(productId: updatedProductModel.Id);
              
            //the product name must change due to browser chaching (Browsers aggressively cache static files (especially images) when the URL does not change and as a result the browser will the old pictures) 
            updatedProductModel.ProductName = (newProductName == oldProductName) ? newProductName + "_" : newProductName;

            try
            {
                await _productOperationsService.UpdateProductAsync(id: updatedProductModel.Id, name: updatedProductModel.ProductName, description: updatedProductModel.Description, price: updatedProductModel.Price, creatorId: LoggedUserId);
                await _productFileService.UpdateProductInFolderAsync(product: updatedProductModel, creatorName: LoggedUserUsername, oldProductName: oldProductName);               
            }
            catch (Exception e)
            {
                Console.Write(e.Message.ToString());

                TempData["title"] = "Error";
                TempData["message"] = $"An error occured while attempting to apply the product changes!";
                return RedirectToAction(nameof(Message));
            }

            TempData["title"] = "Success";
            TempData["message"] = $"The 3D model {updatedProductModel.ProductName} was updated succesfully.";
            return RedirectToAction(nameof(Message));
        }

       

        
               

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> DeleteProduct(Guid productId)
        {
            //product existance validation
            bool doesProductCreatedByCreatorExist = await _productBoolsService.DoesProductCreatedByUserExistAsync(userId: LoggedUserId, productId: productId);

            if (doesProductCreatedByCreatorExist == false)
            {
                return NotFound();
            }

            DeletedProductModel deletedProductModel = await _productService.GetDeletedProductModelAsync(productId:productId);
                      

            if (string.IsNullOrEmpty(Referer) == false)
            { TempData["ReturnUrl"] = Referer; }

            return View(deletedProductModel);
        }
        
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeleteProduct_execute(Guid productId)
        {
            //product existance validation
            bool doesProductCreatedByCreatorExist = await _productBoolsService.DoesProductCreatedByUserExistAsync(userId: LoggedUserId, productId: productId);
            if (doesProductCreatedByCreatorExist == false)
            {
                TempData["title"] = "Not found";
                TempData["message"] = "The product you are trying to delete could not be found!";
                return RedirectToAction(nameof(Message));
            }

            string productName = await _productService.GetProductNameAsync(productId: productId);
            string creatorName = await _productService.GetCreatorNameOfProductAsync(productId: productId);      

            try
            {
                //attempting to delete the product in the database
                await _productOperationsService.DeleteProductAsync(productId);

                //attempting to delete the product folder and it's content (the 6 images and the 3D model file)
                _productFileService.DeleteProductFolder(creatorName: creatorName, productName: productName);
                                
            }
            catch (Exception e)
            {
                Console.Write(e.Message.ToString());

                TempData["title"] = "Error";
                TempData["message"] = $"An error occured while attempting to delete you product!";
                return RedirectToAction(nameof(Message));
            }

            TempData["title"] = "Success";
            TempData["message"] = $"The 3D model {productName} was deleted successfully.";
            return RedirectToAction(nameof(Message));
        }





        [HttpGet]
        public IActionResult Message()
        {
            return View();
        }


    }
}
