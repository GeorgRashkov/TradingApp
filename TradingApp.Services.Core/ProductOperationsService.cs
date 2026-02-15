
using Microsoft.EntityFrameworkCore;
using TradingApp.Data;
using TradingApp.Data.Models;
using TradingApp.GCommon;
using TradingApp.Services.Core.Interfaces;

namespace TradingApp.Services.Core
{
    public class ProductOperationsService: IProductOperationsService
    {
        private ApplicationDbContext _context;
        private IProductBoolsService _productBoolsService;
        public ProductOperationsService(ApplicationDbContext context, IProductBoolsService productBoolsService) 
        {
            _context = context;
            _productBoolsService = productBoolsService;
        }
        
        //saves the product in the database
        public async Task AddProductAsync(string name, string description, decimal price, string creatorId)
        {
            bool doesCreatorExist = await _productBoolsService.DoesUserExistAsync(userId: creatorId);
            if (doesCreatorExist == false)
            {
                throw new InvalidOperationException(message: "I product cannot be created because the creator ID was not found in the DB!");
            }

            bool doesProductCreatedByCreatorExist = await _productBoolsService.DoesProductCreatedByUserExistAsync(userId: creatorId, productName: name);
            if (doesProductCreatedByCreatorExist == true)
            {
                throw new InvalidOperationException(message:"Creators are not allowed to create 2 or more products with the same name!");
            }

           
            Product product = new Product()
            {
                Name = name,
                Description = description,
                Price = price,
                CreatorId = creatorId,
                Status = ApplicationConstants.CreatedProductDefaultStatus
            };
            
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProductAsync(Guid id, string name, string description, decimal price, string creatorId)
        {
            Product? product = await _context.Products
               .FindAsync(id);

            if (product == null)
            {
                throw new InvalidOperationException("Product could not be updated in the DB because it's ID was not in the DB!");
            }

            if (product.CreatorId != creatorId)
            {
                throw new UnauthorizedAccessException("The product cannot be updated by anyone except it's creator.");
            }

            //update the product
            product.Name = name;
            product.Description = description;
            product.Price = price;
            product.Status = ApplicationConstants.CreatedProductDefaultStatus;

            //apply change to DB
            await _context.SaveChangesAsync();
        }


        public async Task DeleteProductAsync(Guid productId)
        {
            Product product = (await _context.Products.FindAsync(productId))!;

            if (product == null)
            {
                throw new InvalidOperationException("Product could not be deleted from the DB because it's ID was not in the DB!");
            }

            await DeleteSellOrdersOfProductAsync(productId);
            await SetFkToNullForCompletedOrdersOfProductAsync(productId);

            _context.Products.Attach(product);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        private async Task DeleteSellOrdersOfProductAsync(Guid productId)
        {
            IEnumerable<SellOrder> sellOrders = await _context
                .SellOrders
                .Include(so => so.Product)
                .Where(so => so.Product.Id == productId)
                .ToListAsync();

            _context.SellOrders.RemoveRange(sellOrders);            
        }

        private async Task SetFkToNullForCompletedOrdersOfProductAsync(Guid productId)
        {
            IEnumerable<CompletedOrder> completedOrders = await _context
                .CompletedOrders
                .Include(so => so.Product)
                .Where(so => so.Product.Id == productId)
                .ToListAsync();

            foreach (CompletedOrder completedOrder in completedOrders)
            {
                completedOrder.Product = null;
            }
        }
    }
}
