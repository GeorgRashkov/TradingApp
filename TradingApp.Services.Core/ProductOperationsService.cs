
using TradingApp.Data.Models;
using TradingApp.Data.Repository.Interfaces;
using TradingApp.GCommon;
using TradingApp.GCommon.Enums;
using TradingApp.GCommon.ErrorCodes;
using TradingApp.Services.Core.Interfaces;

namespace TradingApp.Services.Core
{
    public class ProductOperationsService : IProductOperationsService
    {
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;
        public ProductOperationsService(IUserRepository userRepository, IProductRepository productRepository)
        {
            _userRepository = userRepository;
            _productRepository = productRepository;
        }


        //saves the product in the database
        public async Task<Result> AddProductAsync(string name, string description, decimal price, string creatorId)
        {
            bool doesUserExist = await _userRepository.DoesUserExistAsync(userId: creatorId);
            if (doesUserExist == false)
            {
                return new Result(errorCode: UserErrorCodes.UserNotFound);
            }

            bool doesProductCreatedByUserExist = await _productRepository.DoesProductCreatedByUserExistAsync(userId: creatorId, productName: name);
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

            await _productRepository.CreateProductAsync(product:product);          

            return new Result();
        }

        //updates an existing product in the database
        public async Task<Result> UpdateProductAsync(Guid id, string name, string description, decimal price, string creatorId)
        {
            Product? product = await _productRepository.GetProductByIdAsync(productId: id);

            if (product == null)
            {
                return new Result(errorCode: ProductErrorCodes.ProductNotFound);
            }
            else if (product.CreatorId != creatorId)
            {
                return new Result(errorCode: ProductErrorCodes.ProductInvalidCreator);
            }

            bool doesCreatorHaveOtherProductsWithTheSameName = await _userRepository.DoesCreatorHaveOtherProductsWithTheSameNameAsync(productName: name, productId:id, creatorId:creatorId);
            if(doesCreatorHaveOtherProductsWithTheSameName == true) 
            {
                return new Result(errorCode: ProductErrorCodes.ProductWithSameNameAlreadyExists);
            }

            bool doesProductHaveNonResolvedReports = await _productRepository.DoesProductHaveNonResolvedReports(productId: id);
            if (doesProductHaveNonResolvedReports == true)
            {
                return new Result(errorCode: ProductErrorCodes.ProductHasNonResolvedReports);
            }

            int productActiveSellOrdersCount = await _productRepository.GetProductActiveSellOrdersCountAsync(productId: id);
            if (productActiveSellOrdersCount > 0)
            {
                return new Result(errorCode: ProductErrorCodes.ProductHasActiveSaleOrders);
            }

            await _productRepository.UpdateProductAsync(product: product, newName: name, newDescription: description, newPrice:price);
          
            return new Result();
        }

        //deletes an existing product in the database
        public async Task<Result> DeleteProductAsync(Guid id, string creatorId)
        {
            Product? product = await _productRepository.GetProductByIdAsync(productId: id);

            if (product == null)
            {
                return new Result(errorCode: ProductErrorCodes.ProductNotFound);
            }
            else if (product.CreatorId != creatorId)
            {
                return new Result(errorCode: ProductErrorCodes.ProductInvalidCreator);
            }

            bool doesProductHaveNonResolvedReports = await _productRepository.DoesProductHaveNonResolvedReports(productId: id);

            if (doesProductHaveNonResolvedReports == true)
            {
                return new Result(errorCode: ProductErrorCodes.ProductHasNonResolvedReports);
            }

            await _productRepository.DeleteProductAsync(product: product);

            return new Result();
        }

        //changes the status of an existing product in the database
        public async Task<Result> ChangeProductStatusAsync(Guid id, ProductStatus productStatus)
        {
            Product? product = await _productRepository.GetProductByIdAsync(productId: id);

            if (product == null)
            {
                return new Result(errorCode: ProductErrorCodes.ProductNotFound);
            }
            else if (product.Status == productStatus)
            {
                return new Result(errorCode: ProductErrorCodes.ProductInvalidStatus);
            }

            await _productRepository.ChangeProductStatusAsync(product: product, newStatus: productStatus);

            return new Result();
        }

    }
}
