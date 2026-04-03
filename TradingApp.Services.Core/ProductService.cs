using Microsoft.EntityFrameworkCore;
using System.Globalization;
using TradingApp.Data.Dtos.Product;
using TradingApp.Data.Repository.Interfaces;
using TradingApp.GCommon;
using TradingApp.GCommon.Filters;
using TradingApp.Services.Core.Interfaces;
using TradingApp.ViewModels.InputProduct;
using TradingApp.ViewModels.Product;

namespace TradingApp.Services.Core
{
    public class ProductService : IProductService
    {
        private const int _productsPerPage = ApplicationConstants.ProductsPerPage;
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public int ProductPageIndex { get; private set; }


        public async Task<bool> DoesProductCreatedByUserExistAsync(string userId, Guid productId)
        {
            return await _productRepository.DoesProductCreatedByUserExistAsync(userId: userId, productId: productId);
        }

        public async Task<IEnumerable<ProductViewModel>> GetApprovedProductsWithActiveSellOrdersAsync(int pageIndex, ProductFilter? productFilter)
        {
            int productsCount = await _productRepository.GetCountOf_ApprovedProductsWithActiveSellOrdersAsync(productFilter: productFilter);

            if (productsCount == 0)
            { return new List<ProductViewModel>(); }

            SetProductPage(pageIndex, productsCount);

            IEnumerable<ProductDto> productsDtos = await _productRepository.GetDtosOf_ApprovedProductsWithActiveSellOrdersAsync(productFilter: productFilter, skipCount: ProductPageIndex * _productsPerPage, takeCount: _productsPerPage);

            List<ProductViewModel> products = productsDtos.Select(p => new ProductViewModel
            {
                Id = p.Id,
                CreatorName = p.CreatorName,
                Price = p.Price.ToString("f2"),
                ProductName = p.ProductName
            }).ToList();

            return products;
        }

        public async Task<IEnumerable<ProductViewModel>> GetProductsAsync(int pageIndex)
        {
            int productsCount = await _productRepository.GetProductsCountAsync();

            if (productsCount == 0)
            { return new List<ProductViewModel>(); }

            SetProductPage(pageIndex, productsCount);

            IEnumerable<ProductDto> productsDtos = await _productRepository.GetDtosOf_ProductsAsync(skipCount: ProductPageIndex * _productsPerPage, takeCount: _productsPerPage);

            List<ProductViewModel> products = productsDtos.Select(p => new ProductViewModel
            {
                Id = p.Id,
                CreatorName = p.CreatorName,
                Price = p.Price.ToString("f2"),
                ProductName = p.ProductName,
                Status = p.Status.ToString()
            }).ToList();

            return products;
        }

        public async Task<Dictionary<string, string>> GetIdsAndNamesOfApprovedProductsWithActiveSaleOrdersCreatedByUserAsync(string userId)
        {
            IEnumerable<ProductListItemDto> productListItemDtos = await _productRepository.GetProductListItemDtosOf_ApprovedProductsWithActiveSaleOrdersCreatedByUserAsync(userId: userId);

            Dictionary<string, string> productIdsAndNamesDict = productListItemDtos
                .Select(p => new { Id = p.Id.ToString(), Name = p.ProductName })
                .ToDictionary(p => p.Id, p => p.Name);

            return productIdsAndNamesDict;
        }

        public async Task<ProductDetailsViewModel?> GetDetailsForApprovedProductWithActiveSellOrdersAsync(Guid productId)
        {
            ProductDetailsDto? productDetailsDto = await _productRepository.GetProductDetailsDtoOf_ApprovedProductWithActiveSellOrdersAsync(productId: productId);
            if (productDetailsDto == null)
            { return null; }

            ProductDetailsViewModel product = new ProductDetailsViewModel
            {
                Id = productDetailsDto.Id,
                ProductName = productDetailsDto.ProductName,
                Price = productDetailsDto.Price.ToString("f2"),
                CreatorName = productDetailsDto.CreatorName,
                Description = productDetailsDto.Description,
                FirstSellOrderCreationDate = productDetailsDto.FirstSellOrderCreationDate.ToString(ApplicationConstants.DateFormat, CultureInfo.InvariantCulture),
                LastSellOrderCreationDate = productDetailsDto.LastSellOrderCreationDate.ToString(ApplicationConstants.DateFormat, CultureInfo.InvariantCulture),
                SellOrdersCount = productDetailsDto.ActiveSellOrdersCount
            };

            return product;
        }



        public async Task<IEnumerable<MyProductViewModel>> GetProductsCreatedByUserAsync(int pageIndex, string userId)
        {
            int productsCount = await _productRepository.GetProductsCountCreatedByUserAsync(userId: userId);

            if (productsCount == 0)
            { return new List<MyProductViewModel>(); }

            SetProductPage(pageIndex, productsCount);

            IEnumerable<ProductDto> productDtos = await _productRepository.GetDtosOf_ProductsCreatedByUserAsync(userId: userId, skipCount: ProductPageIndex * _productsPerPage, takeCount: _productsPerPage);
            List<MyProductViewModel> products = productDtos.Select(p => new MyProductViewModel
            {
                Id = p.Id,
                Price = p.Price.ToString("f2"),
                ProductName = p.ProductName,
                ProductStatus = p.Status.ToString(),
                CreatorName = p.CreatorName
            }).ToList();

            return products;
        }



        public async Task<MyProductDetailsViewModel?> GetDetailsForProductAsync(Guid productId)
        {
            ProductDetailsDto? productDetailsDto = await _productRepository.GetProductDetailsDtoAsync(productId: productId);
            if (productDetailsDto == null)
            { return null; }

            MyProductDetailsViewModel product = new MyProductDetailsViewModel
            {
                Id = productDetailsDto.Id,
                ProductName = productDetailsDto.ProductName,
                Description = productDetailsDto.Description,
                CreatorName = productDetailsDto.CreatorName,
                Price = productDetailsDto.Price.ToString("f2"),
                ProductStatus = productDetailsDto.Status.ToString(),
                ActiveSellOrdersCount = productDetailsDto.ActiveSellOrdersCount
            };

            return product;           
        }

        private void SetProductPage(int pageIndex, int productsCount)
        {
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageIndex = pageIndex * _productsPerPage >= productsCount ? (int)Math.Ceiling((decimal)productsCount / (decimal)_productsPerPage) - 1 : pageIndex;
            ProductPageIndex = pageIndex;
        }

        public async Task<int> GetProductActiveSellOrdersCountAsync(Guid productId)
        {
            int sellOrdersCount = await _productRepository.GetProductActiveSellOrdersCountAsync(productId: productId);
            return sellOrdersCount;
        }

        public async Task<UpdatedProductModel?> GetUpdatedProductModelAsync(Guid productId)
        {
            Product_UpdateProductDto? productDto = await _productRepository.GetProductToUpdateAsync(productId: productId);
            if(productDto == null) 
            { return null; }

            UpdatedProductModel? product = new UpdatedProductModel()
            {
                Id = productDto.Id,
                ProductName = productDto.Name,
                Description = productDto.Description,
                Price = decimal.Parse(productDto.Price.ToString("f2"))
            };

            return product;
        }

        public async Task<DeletedProductModel?> GetDeletedProductModelAsync(Guid productId)
        {
            Product_DeleteProductDto? productDto = await _productRepository.GetProductToDeleteAsync(productId: productId);
            if (productDto == null)
            { return null; }

            DeletedProductModel? product = new DeletedProductModel()
            {
                ProductId = productDto.Id,
                ProductName = productDto.Name,
            };

            return product;
        }

        public async Task<ManagedProductModel?> GetManagedProductModelAsync(Guid productId)
        {
            Product_ManageProductDto? productDto = await _productRepository.GetProductToManageAsync(productId: productId);
            if (productDto == null)
            { return null; }

            ManagedProductModel product = new ManagedProductModel()
            {
                Id = productDto.Id,
                Name = productDto.Name,
                Status = productDto.Status
            };

            return product;
        }


        public async Task<string?> GetProductNameAsync(Guid productId)
        {
            string? productName = await _productRepository.GetProductNameAsync(productId: productId);
            return productName;
        }
    }
}
