using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TradingApp.Data.Helpers;
using TradingApp.Data;
using TradingApp.Data.Models;
using TradingApp.ViewModels.InputProduct;
using TradingApp.Services;

namespace TradingApp.Controllers
{
    public class ProductOperationsController : Controller
    {
        private GCommon.Enums.ProductStatus _createdProductDefaultStatus = GCommon.Enums.ProductStatus.approved;
        private const int MaxSellOrdersPerProduct = 2;


        private CrudDb _crudDb;
        private CrudFile _crudFile;
        public ProductOperationsController(ApplicationDbContext context)
        {
            _crudDb = new CrudDb(context);
            _crudFile = new CrudFile();
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
            try
            {
                if (ModelState.IsValid == false)
                {
                    return View(createdProductModel);
                }

                createdProductModel.ProductName = createdProductModel.ProductName.Trim();

                bool doesProductCreatedByCreatorExist = await _crudDb.DoesProductCreatedByCreatorExistAsync(userId: LoggedUserId, productName: createdProductModel.ProductName);

                if (doesProductCreatedByCreatorExist == true)
                {
                    ModelState.AddModelError(string.Empty, "A product with the same name already exists!\n Consider using unique name before creating your 3D model file.");
                    return View(createdProductModel);
                }

                bool doesCreatorExist = await _crudDb.DoesCreatorExistAsync(userId: LoggedUserId);
                await _crudFile.SaveProductInFolder(product: createdProductModel, creatorName: LoggedUserUsername, createUser: !doesCreatorExist);

                Product product = new Product
                {
                    Name = createdProductModel.ProductName,
                    Description = createdProductModel.Description,
                    Price = createdProductModel.Price,
                    CreatorId = LoggedUserId,
                    Status = _createdProductDefaultStatus
                };
                await _crudDb.SaveProductAsync(product);

                TempData["title"] = "Success";
                TempData["message"] = $"The 3D model {createdProductModel.ProductName} was created succesfully.";
                return RedirectToAction(nameof(Message));
            }
            catch (Exception e)
            {
                Console.Write(e.Message.ToString());

                TempData["title"] = "Error";
                TempData["message"] = $"An error occured while attempting to save you product!";
                return RedirectToAction(nameof(Message));
            }

        }








        [HttpGet]
        [Authorize]
        public async Task<IActionResult> UpdateProduct(Guid productId)
        {
            bool doesProductCreatedByCreatorExist = await _crudDb.DoesProductCreatedByCreatorExistAsync(userId: LoggedUserId, productId: productId);

            if (doesProductCreatedByCreatorExist == false)
            {
                return NotFound();
            }


            int currentProductSellOrdersCount = await _crudDb.GetSellOrdersCountAsync(userId: LoggedUserId, productId: productId);

            if (currentProductSellOrdersCount > 0)
            {
                TempData["title"] = "Not allowed";
                TempData["message"] = $"The product you are trying to update has at least one sell order! Make sure you cancel all sell orders of the product before editing it!";
                return RedirectToAction(nameof(Message));
            }

            ProductFilter filter = new ProductFilter
            {
                PorductId = productId,
            };

            UpdatedProductModel product = (await _crudDb.GetProductAsync<UpdatedProductModel>(productFilter: filter,
                selector: p => new UpdatedProductModel()
                {
                    Id = productId,
                    ProductName = p.Name,
                    Description = p.Description,
                    Price = p.Price
                }))!;

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
            bool doesProductCreatedByCreatorExist = await _crudDb.DoesProductCreatedByCreatorExistAsync(userId: LoggedUserId, productId: updatedProductModel.Id);
            if (doesProductCreatedByCreatorExist == false)
            {
                ModelState.AddModelError(string.Empty, "The product you are trying to edit could not be found!");
                return View(updatedProductModel);
            }

            string newProductName = updatedProductModel.ProductName.Trim();
            string oldProductName = (await _crudDb.GetProductAsync(
                   productFilter: new ProductFilter() { PorductId = updatedProductModel.Id },
                   selector: p => p.Name)
                   )!;

            //the product name must change due to browser chaching (Browsers aggressively cache static files (especially images) when the URL does not change and as a result the browser will the old pictures) 
            updatedProductModel.ProductName = (newProductName == oldProductName) ? newProductName + "_" : newProductName;

            //attempting to update the product folder and the 3D model file in it
            try
            {               
                await _crudFile.UpdateProductInFolder(product: updatedProductModel, creatorName: LoggedUserUsername, oldProductName: oldProductName);
            }
            catch (Exception e)
            {
                Console.Write(e.Message.ToString());

                TempData["title"] = "Error";
                TempData["message"] = $"An error occured while attempting to save you product!";
                return RedirectToAction(nameof(Message));
            }

            Product product = new Product
            {
                Id = updatedProductModel.Id,
                Name = updatedProductModel.ProductName,
                Description = updatedProductModel.Description,
                Price = updatedProductModel.Price,
                Status = _createdProductDefaultStatus,
                CreatorId = LoggedUserId
            };

            //attempting to update the product in the database
            try
            {
                await _crudDb.UpdateProductAsync(product);
            }
            catch (Exception e)
            {
                Console.Write(e.Message.ToString());

                TempData["title"] = "Error";
                TempData["message"] = $"An error occured while attempting to save you product!";
                return RedirectToAction(nameof(Message));
            }
            TempData["title"] = "Success";
            TempData["message"] = $"The 3D model {updatedProductModel.ProductName} was updated succesfully.";
            return RedirectToAction(nameof(Message));


        }


        

        [HttpGet]
        public IActionResult Message()
        {
            return View();
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> DeleteProduct(Guid productId)
        {
            //product existance validation
            bool doesProductCreatedByCreatorExist = await _crudDb.DoesProductCreatedByCreatorExistAsync(userId: LoggedUserId, productId: productId);

            if (doesProductCreatedByCreatorExist == false)
            {
                return NotFound();
            }

          
            DeletedProductModel model = new DeletedProductModel() 
            { 
                ProductId = productId, 
                ProductName = (await _crudDb.GetProductAsync(
                    productFilter: new ProductFilter() { PorductId =  productId },
                    selector: p => p.Name))!
            };

            if(string.IsNullOrEmpty(Referer) == false)
            { TempData["ReturnUrl"] = Referer; }

            return View(model);
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeleteProduct_execute(Guid productId)
        {      
            //product existance validation
            bool doesProductCreatedByCreatorExist = await _crudDb.DoesProductCreatedByCreatorExistAsync(userId: LoggedUserId, productId: productId);
            if (doesProductCreatedByCreatorExist == false)
            {
                TempData["title"] = "Not found";
                TempData["message"] = "The product you are trying to delete could not be found!";
                return RedirectToAction(nameof(Message));
            }            

            var product = (await _crudDb.GetProductAsync(
                   productFilter: new ProductFilter() { PorductId = productId },
                   selector: p => new { Name = p.Name, CreatorName = p.Creator.UserName } ))!;

            string productName = product.Name;
            string creatorName = product.CreatorName;
            

            try
            {
                //attempting to delete the product folder and it's content (the 6 images and the 3D model file)
                _crudFile.DeleteProductFolder(creatorName: creatorName, productName: productName);

                //attempting to delete the product in the database
                await _crudDb.DeleteProductAsync(productId);
            }
            catch (Exception e)
            {
                Console.Write(e.Message.ToString());

                TempData["title"] = "Error";
                TempData["message"] = $"An error occured while attempting to delete you product!";
                return RedirectToAction(nameof(Message));
            }

            TempData["title"] = "Success";
            TempData["message"] = $"The 3D model {productName} was deleted succesfully.";
            return RedirectToAction(nameof(Message));
        }
    }
}
