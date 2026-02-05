using AspNetCoreGeneratedDocument;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TradingApp.Data;
using TradingApp.Data.Enums;
using TradingApp.Data.Models;
using TradingApp.InputModels;
using TradingApp.ViewModels.Product;

namespace TradingApp.Controllers
{
    public class ProductController : Controller
    {
        private ApplicationDbContext _context;
        public ProductController(ApplicationDbContext context)
        {
            _context = context;
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
        public async Task<IActionResult> Products()
        {
            List<ProductViewModel> products = await _context.Products
                .AsNoTracking()
                .Where(p => p.Status == ProductStatus.approved)
                .Select(p => new ProductViewModel
                {
                    CreatorName = p.Creator.UserName,
                    Description = p.Description,
                    Price = p.Price,
                    ProductName = p.Name
                })
                .ToListAsync();

            ViewData["isPrivate"] = false;

            return View(model: products);
        }

        [HttpGet]
        public async Task<IActionResult> Product(string creatorName, string productName)
        {
            ProductViewModel? product = await _context.Products
                .AsNoTracking()
                .Where(p => p.Creator.UserName == creatorName && p.Name == productName)
                .Select(p => new ProductViewModel
                {
                    CreatorName = p.Creator.UserName,
                    Description = p.Description,
                    Price = p.Price,
                    ProductName = p.Name
                })
                .FirstOrDefaultAsync();

            if (product == null)
            { return NotFound(); }


            return View(model: product);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> MyProducts()
        {
            //get the Id of the logged user
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            List<ProductViewModel> products = await _context.Products
               .AsNoTracking()
               .Where(p => p.CreatorId == userId)
               .Select(p => new ProductViewModel
               {
                   CreatorName = p.Creator.UserName,
                   Description = p.Description,
                   Price = p.Price,
                   ProductName = p.Name
               })
               .ToListAsync();

            ViewData["isPrivate"] = true;

            return View(viewName: "Products", model: products);
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
            if (ModelState.IsValid == false)
            {
                return View(createdProductModel);
            }

            bool doesCreatorExist = DoesCreatorExist(LoggedUserId);
            bool doesProductCreatedByCreatorExist = DoesProductCreatedByCreatorExist(LoggedUserId, createdProductModel.ProductName);

            if (doesProductCreatedByCreatorExist == true)
            {
                TempData["title"] = "error";
                TempData["message"] = "An error occurred: a product with the same name already exists!\n Consider using unique name for your 3D model file.\n If you want to replace an existing product make sure you delete it first.";
                return RedirectToAction(nameof(Message));
            }

            await SaveProductInFolder(product: createdProductModel, creatorName: LoggedUserUsername, createUser: !doesCreatorExist);
            
            Product product = new Product
            {
                Name = createdProductModel.ProductName,
                Description = createdProductModel.Description,
                Price = createdProductModel.Price,
                CreatorId = LoggedUserId,
                Status = ProductStatus.approved
            };
            await SaveProductInDb(product);

            TempData["title"] = "success";
            TempData["message"] = $"The 3D model {createdProductModel.ProductName} was created succesfully.";
            return RedirectToAction(nameof(Message));

        }
        
        private bool DoesCreatorExist(string userId)
        {
            return _context.Users
                    .AsNoTracking()
                    .Any(u => u.Id == userId);
        }

        private bool DoesProductCreatedByCreatorExist(string userId, string productName)
        {
            return _context.Products
                    .AsNoTracking()
                    .Any(p => p.CreatorId == userId && p.Name == productName);
        }

        private async Task SaveProductInFolder(CreatedProductModel product, string creatorName, bool createUser)
        {
            string creatorPath = Path.Combine("wwwroot", "Creators",creatorName);
            if (createUser == true)
            { Directory.CreateDirectory(creatorPath); }

            string productPath = Path.Combine(creatorPath, product.ProductName);
            Directory.CreateDirectory(productPath);

            Dictionary<string, IFormFile> imageFiles = product.GetDictOfImageFiles();

            foreach ((string imageName, IFormFile imageFile) in imageFiles)
            {
                if(imageFile == null) 
                { continue; }

                string imagePath = Path.Combine(productPath, imageName + ".jpg");
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }                
            }

            string product3DModelPath = Path.Combine(productPath, product.ProductName + ".jpg");
            using (var stream = new FileStream(product3DModelPath, FileMode.Create))
                { await product.File3DModel.CopyToAsync(stream); }
        }

        public async Task SaveProductInDb(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        //code for creating a product>



        public IActionResult Message()
        {
            return View();
        }



        /*

        //this method checks only the image files uploaded by the user; the user may use any name for any of the files but the name which will be used for saving an image file is specified by the key parameter; the uses the key parameter to form a proper error message
        private string CheckImageFiles(Dictionary<string, IFormFile> imageFiles)
        {
            StringBuilder errorMessages = new StringBuilder() ;

            foreach ((string fileName, IFormFile imageFile) in imageFiles)
            {
                if (imageFile == null || imageFile.Length == 0)
                    errorMessages.AppendLine($"Error: the {fileName} image file was missing!");


                var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();

                if (extension != ".jpg" && extension != ".jpeg")
                    errorMessages.AppendLine($"Error: the {fileName} image file was not jpg!");
            }

            return errorMessages.ToString();
        }
        */
    }
}
