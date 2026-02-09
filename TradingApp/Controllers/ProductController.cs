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
        private int _productsMaxActiveSellOrdersPerUser = 3;
        private int _productMaxActiveSellOrdersPerUser = 2;
        private CrudDb _crudDb;

        public ProductController(ApplicationDbContext context)
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
                   Id = p.Id,
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
                    Id = p.Id,
                    ProductName = p.Name,
                    Price = p.Price.ToString("f2"),
                    CreatorName = p.Creator.UserName,
                    Description = p.Description,
                    FirstSellOrderCreationDate = p.SellOrders.Where(so => so.Status == SellOrderStatus.active).Select(so => so.CreatedAt).OrderBy(createdAt => createdAt).SingleOrDefault().ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                    LastSellOrderCreationDate = p.SellOrders.Where(so => so.Status == SellOrderStatus.active).Select(so => so.CreatedAt).OrderByDescending(createdAt => createdAt).SingleOrDefault().ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                    SellOrdersCount = p.SellOrders.Where(so => so.Status == SellOrderStatus.active).Count()
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
                    Id = p.Id,
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
                    Id = p.Id,
                    ProductName = p.Name,
                    Description = p.Description,
                    CreatorName = p.Creator.UserName,
                    Price = p.Price.ToString("f2"),
                    ProductStatus = p.Status.ToString(),
                    ActiveSellOrdersCount = p.SellOrders.Where(so => so.Status == SellOrderStatus.active).Count()
                }

                );

            if (product == null)
            {
                return NotFound();
            }
            else if (LoggedUserUsername != product.CreatorName)
            {
                return Forbid();
            }

            int loggedUserActiveSellOrdersCount = await _crudDb.GetSellOrdersCountAsync(LoggedUserId);
            ViewData["currentUserMaxSellOrdersCountReached"] = loggedUserActiveSellOrdersCount >= _productsMaxActiveSellOrdersPerUser ? true : false;
            ViewData["currentProductMaxSellOrdersCountReached"] = product.ActiveSellOrdersCount >= _productMaxActiveSellOrdersPerUser ? true : false;

            return View(model: product);
        }
    }
}
