using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TradingApp.Data;
using TradingApp.Data.Enums;
using TradingApp.Data.Models;
using TradingApp.InputModels;
using TradingApp.Services;

namespace TradingApp.Controllers
{
    public class ProductOperationsController : Controller
    {
        private ProductStatus _createdProductDefaultStatus = ProductStatus.approved;


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



        [HttpGet]
        [Authorize]
        public IActionResult CreateProduct()
        {
            return View();
        }
      
        //<code for creating a product
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

                bool doesProductCreatedByCreatorExist = await _crudDb.DoesProductCreatedByCreatorExistAsync(userId: LoggedUserId, productName: createdProductModel.ProductName);

                if (doesProductCreatedByCreatorExist == true)
                {
                    //TempData["title"] = "error";
                    //TempData["message"] = "An error occurred: a product with the same name already exists!\n Consider using unique name for your 3D model file.\n If you want to replace an existing product make sure you delete it first.";
                    //return RedirectToAction(nameof(Message));
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

        public IActionResult Message()
        {
            return View();
        }
    }
}
