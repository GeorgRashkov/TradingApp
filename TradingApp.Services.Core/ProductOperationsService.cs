
using Microsoft.EntityFrameworkCore;
using TradingApp.Data;
using TradingApp.Data.Models;
using TradingApp.GCommon;
using TradingApp.GCommon.ErrorCodes;
using TradingApp.Services.Core.Interfaces;

namespace TradingApp.Services.Core
{
    public class ProductOperationsService : IProductOperationsService
    {
        private ApplicationDbContext _context;
        private IProductBoolsService _productBoolsService;
        private IProductService _productService;
        public ProductOperationsService(ApplicationDbContext context, IProductBoolsService productBoolsService, IProductService productService)
        {
            _context = context;
            _productBoolsService = productBoolsService;
            _productService = productService;
        }

        //saves the product in the database
        public async Task<Result> AddProductAsync(string name, string description, decimal price, string creatorId)
        {
            bool doesUserExist = await _productBoolsService.DoesUserExistAsync(userId: creatorId);
            if (doesUserExist == false)
            {
                return new Result(errorCode: UserErrorCodes.UserNotFound);
            }

            bool doesProductCreatedByUserExist = await _productBoolsService.DoesProductCreatedByUserExistAsync(userId: creatorId, productName: name);
            if (doesProductCreatedByUserExist == true)
            {
                return new Result(errorCode: ProductErrorCodes.ProductWithSameNameAlreadyExists);
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

            return new Result();
        }

        //updates an existing product in the database
        public async Task<Result> UpdateProductAsync(Guid id, string name, string description, decimal price, string creatorId)
        {
            Product? product = await _context
                .Products                            
                .Where(p => p.Id == id)
                .SingleOrDefaultAsync();

            if (product == null)
            {
                return new Result(errorCode: ProductErrorCodes.ProductNotFound);
            }
            else if (product.CreatorId != creatorId)
            {
                return new Result(errorCode: ProductErrorCodes.ProductInvalidCreator);
            }

            int productActiveSellOrdersCount = await _productService.GetProductActiveSellOrdersCountAsync(productId: id);

            if (productActiveSellOrdersCount > 0)
            {
                return new Result(errorCode: ProductErrorCodes.ProductHasActiveSaleOrders);
            }

            //update the product
            product.Name = name;
            product.Description = description;
            product.Price = price;
            product.Status = ApplicationConstants.CreatedProductDefaultStatus;

            //apply change to DB
            await _context.SaveChangesAsync();
            return new Result();
        }

        //deletes an existing product in the database
        public async Task<Result> DeleteProductAsync(Guid id, string creatorId)
        {
            Product product = (await _context.Products.FindAsync(id))!;

            if (product == null)
            {
                return new Result(errorCode: ProductErrorCodes.ProductNotFound);
            }
            else if (product.CreatorId != creatorId)
            {
                return new Result(errorCode: ProductErrorCodes.ProductInvalidCreator);
            }

            await DeleteSellOrdersOfProductAsync(id);
            await SetFkToNullForCompletedOrdersOfProductAsync(id);

            _context.Products.Attach(product);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return new Result();
        }

        private async Task DeleteSellOrdersOfProductAsync(Guid productId)
        {
            IEnumerable<SellOrder> sellOrders = await _context
                .SellOrders
                .Include(so => so.Product)
                .Where(so => so.Product.Id == productId)
                .ToListAsync();

            await DeleteSellOrderSuggestionsAsync(sellOrderIds: sellOrders.Select(so => so.Id));

            _context.SellOrders.RemoveRange(sellOrders);
        }

        private async Task DeleteSellOrderSuggestionsAsync(IEnumerable<Guid>sellOrderIds)
        {
            IEnumerable<SellOrderSuggestion> sellOrderSuggestions = await _context.
                SellOrderSuggestions
                .Where(sos => sellOrderIds.Contains(sos.SellOrderId))
                .ToListAsync();

            _context.SellOrderSuggestions.RemoveRange(sellOrderSuggestions);
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
