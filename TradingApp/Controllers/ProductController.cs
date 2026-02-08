using AspNetCoreGeneratedDocument;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TradingApp.Common;
using TradingApp.Data;
using TradingApp.Data.Enums;
using TradingApp.Data.Models;
using TradingApp.InputModels;
using TradingApp.ViewModels.Product;
using TradingApp.Services;

namespace TradingApp.Controllers
{
    public class ProductController : Controller
    {
        
        private int _productsPerPage = 4;
        private int _maxActiveSellOrdersPerUser = 3;
        private CrudDb _crudDb;

        public  ProductController(ApplicationDbContext context)
        {
            _crudDb = new CrudDb(context);
        }
        /*
        private ApplicationDbContext _context;
        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }
        */

        //this is the Id of the currently logged user; if the user is not logged the value will be null 
        private string LoggedUserId
        {
            get { return User.FindFirst(ClaimTypes.NameIdentifier)?.Value; }
        }

        //this is the username of the currently logged user; if the user is not logged the value will be null 
        private string LoggedUserUsername
        {
            get { return User.Identity?.Name; }
        }



        [HttpGet]
        public async Task<IActionResult> Products(int pageIndex)
        {
            pageIndex = pageIndex < 0 ? 0 : pageIndex;

            ProductFilter filter = new ProductFilter()
            {
                ProductStatus = ProductStatus.approved,
                SellOrderStatus = SellOrderStatus.active,
            };

            int productsCount = await _crudDb.GetProductsCountAsync(filter);
            if (productsCount == 0)
            { return View(model: null); }
            pageIndex = pageIndex * _productsPerPage >= productsCount ? (int)Math.Ceiling((decimal)productsCount / (decimal)_productsPerPage) - 1 : pageIndex; ;//pageIndex-1 : pageIndex;


            filter.Skip = pageIndex * _productsPerPage;
            filter.Take = _productsPerPage;
            ViewData["page"] = pageIndex;


            List<ProductsViewModel> products = await _crudDb.GetProductsAsync<ProductsViewModel>(productFilter: filter,
            selector: (p) =>
               new ProductsViewModel
               {
                   Id = p.Id.ToString(),
                   CreatorName = p.Creator.UserName,
                   Price = p.Price.ToString("f2"),
                   ProductName = p.Name
               }
            );

            return View(model: products);
        }

        [HttpGet]
        public async Task<IActionResult> Product(Guid productId)
        {
            ProductFilter filter = new ProductFilter()
            {
                PorductId = productId,
                ProductStatus = ProductStatus.approved,
                SellOrderStatus = SellOrderStatus.active,
            };


            ProductViewModel? product = await _crudDb.GetProductAsync<ProductViewModel>(productFilter: filter, selector:
                p => new ProductViewModel
                {
                    ProductName = p.Name,
                    Description = p.Description,
                    Price = p.Price.ToString("f2"),
                    CreatorName = p.Creator.UserName,
                    SellOrderCreationDate = p.SellOrders.SingleOrDefault().CreatedAt.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)
                }

                );

            if (product == null)
            { return NotFound(); }


            return View(model: product);
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> MyProducts(int pageIndex)
        {
            pageIndex = pageIndex < 0 ? 0 : pageIndex;

            ProductFilter filter = new ProductFilter()
            {
                Username = LoggedUserUsername,
            };

            int productsCount = await _crudDb.GetProductsCountAsync(filter);
            if (productsCount == 0)
            { return View(model: null); }

            pageIndex = pageIndex * _productsPerPage >= productsCount ? (int)Math.Ceiling((decimal)productsCount / (decimal)_productsPerPage) - 1 : pageIndex; ;//pageIndex-1 : pageIndex;


            filter.Skip = pageIndex * _productsPerPage;
            filter.Take = _productsPerPage;
            ViewData["page"] = pageIndex;

            List<MyProductsViewModel> products = await _crudDb.GetProductsAsync(productFilter: filter,
                selector: (p) =>
                new MyProductsViewModel
                {
                    Id = p.Id.ToString(),
                    Price = p.Price.ToString("f2"),
                    ProductName = p.Name,
                    ProductStatus = p.Status.ToString(),
                    CreatorName = p.Creator.UserName
                }
                );

            return View(model: products);
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> MyProduct(Guid productId)
        {
            ProductFilter filter = new ProductFilter()
            {
                PorductId = productId
            };

            MyProductViewModel? product = await _crudDb.GetProductAsync<MyProductViewModel>(productFilter: filter, selector:
                p => new MyProductViewModel
                {
                    Id = p.Id.ToString(),
                    ProductName = p.Name,
                    Description = p.Description,
                    CreatorName = p.Creator.UserName,
                    Price = p.Price.ToString("f2"),
                    ProductStatus = p.Status.ToString(),
                    HasActiveSellOrder = p.SellOrders.Any(so => so.Status == SellOrderStatus.active)
                }

                );

            if(product == null)
            {
                return NotFound();
            }
            else if (LoggedUserUsername != product.CreatorName)
            {
                return Forbid();
            }         

            int activeSellOrdersCount = await _crudDb.GetSellOrdersCountAsync(LoggedUserId);
            ViewData["maxSellOrdersCountReached"] = activeSellOrdersCount >= _maxActiveSellOrdersPerUser ? true:false;

            return View(model: product);
        }

        /*
        //<DB calls

        //this method filters the products based on the parameter values in the filter

        private IQueryable<Product> FilterProducts(ProductFilter productFilter)
        {
            IQueryable<Product> query = _context.Products
               .AsNoTracking();

            if (productFilter.PorductId != null)
            {
                query = query.Where(p => p.Id == productFilter.PorductId);
            }

            if (productFilter.UserId != null)
            {
                query = query.Where(p => p.CreatorId == productFilter.UserId);
            }

            if (productFilter.ProductStatus != null)
            {
                query = query.Where(p => p.Status == productFilter.ProductStatus);
            }

            if (productFilter.SellOrderStatus != null)
            {
                query = query.Where(p => p.SellOrders.Any(so => so.Status == productFilter.SellOrderStatus));//must check if this line works correctly !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            }


            if (productFilter.Username != null)
            {
                if (productFilter.UsernameContains == true)
                { query = query.Where(p => p.Creator.UserName.Contains(productFilter.Username)); }
                else
                { query = query.Where(p => p.Creator.UserName == productFilter.Username); }
            }

            if (productFilter.ProductName != null)
            {
                if (productFilter.ProductNameContains == true)
                { query = query.Where(p => p.Name.Contains(productFilter.ProductName)); }
                else
                { query = query.Where(p => p.Name == productFilter.ProductName); }
            }

            if (productFilter.Skip != null)
            {
                query = query.Skip((int)productFilter.Skip);
            }

            if (productFilter.Take != null)
            {
                query = query.Take((int)productFilter.Take);
            }

            return query;
        }

        private async Task<List<PVM>> GetProductsAsync<PVM>(ProductFilter productFilter, Expression<Func<Product, PVM>> selector)
        {
            List<PVM> productViewModels = await FilterProducts(productFilter)
                .Select(selector)
                .ToListAsync();

            return productViewModels;
        }

        private async Task<PVM?> GetProductAsync<PVM>(ProductFilter productFilter, Expression<Func<Product, PVM>> selector)
        {
            PVM? productViewModel = await FilterProducts(productFilter)
                .Select(selector)
                .FirstOrDefaultAsync();

            return productViewModel;
        }
                
        private async Task<int> GetProductsCountAsync(ProductFilter productFilter)
        {
            int productsCount = await FilterProducts(productFilter).CountAsync();
            return productsCount;
        }


        private async Task<int> GetSellOrdersCountAsync(string userId)
        {
            int sellOrdersCount = await _context.SellOrders
                .AsNoTracking()
                .Where(so => so.CreatorId == userId)
                .CountAsync();
            return sellOrdersCount;
        }
        //DB calls> 
        */
    }
}
