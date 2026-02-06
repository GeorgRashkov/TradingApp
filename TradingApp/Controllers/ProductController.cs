using AspNetCoreGeneratedDocument;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

namespace TradingApp.Controllers
{
    public class ProductController : Controller
    {
        private ApplicationDbContext _context;
        private int _productsPerPage = 4;
        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

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
            pageIndex = pageIndex<0 ? 0: pageIndex;
            
            ProductFilter filter = new ProductFilter()
            {
                ProductStatus = ProductStatus.approved,
                SellOrderStatus = SellOrderStatus.active,                
            };

            int productsCount = GetProductsCount(filter);            
            if (productsCount == 0)
            { return View(model: null); }
            pageIndex = pageIndex * _productsPerPage >= productsCount ? (int)Math.Ceiling((decimal)productsCount/ (decimal)_productsPerPage)-1 : pageIndex; ;//pageIndex-1 : pageIndex;


            filter.Skip = pageIndex * _productsPerPage;
            filter.Take = _productsPerPage;
            ViewData["page"] = pageIndex;


            List<ProductsViewModel> products = await GetProductsAsync<ProductsViewModel>(productFilter: filter,
            selector: (p) =>
               new ProductsViewModel
               {
                   CreatorName = p.Creator.UserName,                   
                   Price = p.Price.ToString("f2"),
                   ProductName = p.Name
               }
            );          

            return View(model: products);
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

            int productsCount = GetProductsCount(filter);
            if(productsCount == 0) 
            { return View(model: null); }

            pageIndex = pageIndex * _productsPerPage >= productsCount ? (int)Math.Ceiling((decimal)productsCount / (decimal)_productsPerPage) - 1 : pageIndex; ;//pageIndex-1 : pageIndex;


            filter.Skip = pageIndex * _productsPerPage;
            filter.Take = _productsPerPage;
            ViewData["page"] = pageIndex;            

            List<MyProductsViewModel> products = await GetProductsAsync(productFilter: filter,
                selector: (p) =>
                new MyProductsViewModel
                {                                        
                    Price = p.Price.ToString("f2"),
                    ProductName = p.Name,
                    ProductStatus = p.Status.ToString(),                    
                    CreatorName = p.Creator.UserName
                }
                );           

            return View(model: products);
        }       


        //<DB calls

        //this method filters the products based on the parameter values in the filter

        private IQueryable<Product> FilterProducts(ProductFilter productFilter)
        {
            IQueryable<Product> query = _context.Products
               .AsNoTracking();

            if (productFilter.ProductStatus != null)
            {
                //query = query.Where(p => p.Status == productFilter.ProductStatus);
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

        private int GetProductsCount(ProductFilter productFilter)
        {
            int productsCount = FilterProducts(productFilter).Count();
            return productsCount;
        }
        //DB calls> 
    }
}
