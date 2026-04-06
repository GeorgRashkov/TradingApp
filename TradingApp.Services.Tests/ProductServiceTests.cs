using Moq;
using TradingApp.Data.Dtos.Product;
using TradingApp.Data.Repository;
using TradingApp.Data.Repository.Interfaces;
using TradingApp.GCommon;
using TradingApp.GCommon.Enums;
using TradingApp.Services.Core;
using TradingApp.Services.Core.Interfaces;
using TradingApp.ViewModels.InputProduct;
using TradingApp.ViewModels.Product;

namespace TradingApp.Services.Tests;

public class ProductServiceTests
{
    private Mock<IProductRepository> _productRepositoryMock;
    [SetUp]
    public void Setup()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
    }


    //<DoesProductCreatedByUserExistAsync tests
    [Test]
    public async Task DoesProductCreatedByUserExistAsync_MustReturnTrue_WhenTheProductWasCreatedByTheUser()
    {
        //Arrange
        string userId = "product creator id";
        Guid productId = Guid.Parse("8118e3a6-63b4-474e-a62d-5cf549af3568");

        _productRepositoryMock
            .Setup(pr => pr.DoesProductCreatedByUserExistAsync(userId, productId))
            .ReturnsAsync(true);

        ProductService productService = new ProductService(_productRepositoryMock.Object);

        //Act
        bool isProductCreatedByUser = await productService.DoesProductCreatedByUserExistAsync(userId: userId, productId: productId);

        //Assert
        Assert.That(isProductCreatedByUser, Is.EqualTo(true));
    }

    [Test]
    public async Task DoesProductCreatedByUserExistAsync_MustReturnFalse_WhenTheProductWasNotCreatedByTheUser()
    {
        //Arrange
        string userId = "another user id";
        Guid productId = Guid.Parse("8118e3a6-63b4-474e-a62d-5cf549af3568");

        _productRepositoryMock
            .Setup(pr => pr.DoesProductCreatedByUserExistAsync(userId, productId))
            .ReturnsAsync(false);

        ProductService productService = new ProductService(_productRepositoryMock.Object);

        //Act
        bool isProductCreatedByUser = await productService.DoesProductCreatedByUserExistAsync(userId: userId, productId: productId);

        //Assert
        Assert.That(isProductCreatedByUser, Is.EqualTo(false));
    }
    //DoesProductCreatedByUserExistAsync tests>



    //<GetApprovedProductsWithActiveSellOrdersAsync tests
    [Test]
    public async Task GetApprovedProductsWithActiveSellOrdersAsync_MustReturnEmptyCollection_WhenThereAreNoSuchProducts()
    {
        //Arrange
        int pageIndex = 5;
        int productsCount = 0;

        _productRepositoryMock
            .Setup(pr => pr.GetCountOf_ApprovedProductsWithActiveSellOrdersAsync(null))
            .ReturnsAsync(productsCount);

        ProductService productService = new ProductService(_productRepositoryMock.Object);


        //Act
        IEnumerable<ProductViewModel> products = await productService.GetApprovedProductsWithActiveSellOrdersAsync(pageIndex: pageIndex, productFilter: null);


        //Assert
        Assert.That(products, Is.Empty);
    }


    [Test]
    public async Task GetApprovedProductsWithActiveSellOrdersAsync_MustReturnNonEmptyCollection_WhenThereAreSuchProducts()
    {
        //Arrange
        int productsPerPage = ApplicationConstants.ProductsPerPage;
        int pageIndex = 5;
        int productsCount = productsPerPage * pageIndex * 2;

        List<ProductDto> productsDtos = new List<ProductDto>()
        {
            new ProductDto()
            {
                Id = Guid.Parse("8118e3a6-63b4-474e-a62d-5cf549af3568"),
                ProductName = "product name 1",
                Price = 11,
                CreatorName = "creator name 1",
                Status = ProductStatus.approved,
            },
             new ProductDto()
            {
                Id = Guid.Parse("4e0545d5-d160-4d9b-8efe-6d0606d18335"),
                ProductName = "product name 2",
                Price = 13,
                CreatorName = "creator name 2",
                Status = ProductStatus.approved,
            },
              new ProductDto()
            {
                Id = Guid.Parse("6dfe1dc8-daec-401b-a691-e6ad85f949cf"),
                ProductName = "product name 3",
                Price = 15,
                CreatorName = "creator name 3",
                Status = ProductStatus.approved,
            }
        };

        _productRepositoryMock
          .Setup(pr => pr.GetCountOf_ApprovedProductsWithActiveSellOrdersAsync(null))
          .ReturnsAsync(productsCount);

        _productRepositoryMock
            .Setup(pr => pr.GetDtosOf_ApprovedProductsWithActiveSellOrdersAsync(null, productsPerPage * pageIndex, productsPerPage))
            .ReturnsAsync(productsDtos);

        ProductService productService = new ProductService(_productRepositoryMock.Object);


        //Act
        List<ProductViewModel> products = (await productService.GetApprovedProductsWithActiveSellOrdersAsync(pageIndex: pageIndex, productFilter: null)).ToList();


        //Assert
        Assert.That(products, Is.Not.Empty);

        for (int i = 0; i < products.Count; i++)
        {
            Assert.That(products[i].Id, Is.EqualTo(productsDtos[i].Id));
            Assert.That(products[i].ProductName, Is.EqualTo(productsDtos[i].ProductName));
            Assert.That(products[i].Price, Is.EqualTo(productsDtos[i].Price.ToString("f2")));
            Assert.That(products[i].CreatorName, Is.EqualTo(productsDtos[i].CreatorName));
        }

        Assert.That(productService.ProductPageIndex, Is.EqualTo(pageIndex));
    }
    //GetApprovedProductsWithActiveSellOrdersAsync tests>



    //<GetProductsAsync tests
    [Test]
    public async Task GetProductsAsync_MustReturnEmptyCollection_WhenThereAreNoProducts()
    {
        //Arrange
        int pageIndex = 5;
        int productsCount = 0;

        _productRepositoryMock
            .Setup(pr => pr.GetProductsCountAsync())
            .ReturnsAsync(productsCount);

        ProductService productService = new ProductService(_productRepositoryMock.Object);


        //Act
        IEnumerable<ProductViewModel> products = await productService.GetApprovedProductsWithActiveSellOrdersAsync(pageIndex: pageIndex, productFilter: null);


        //Assert
        Assert.That(products, Is.Empty);
    }

    [Test]
    public async Task GetProductsAsync_MustReturnNonEmptyCollection_WhenThereAreProducts()
    {
        //Arrange
        int productsPerPage = ApplicationConstants.ProductsPerPage;
        int pageIndex = 5;
        int productsCount = productsPerPage * pageIndex * 2;

        List<ProductDto> productsDtos = new List<ProductDto>()
        {
            new ProductDto()
            {
                Id = Guid.Parse("8118e3a6-63b4-474e-a62d-5cf549af3568"),
                ProductName = "product name 1",
                Price = 11,
                CreatorName = "creator name 1",
                Status = ProductStatus.approved,
            },
             new ProductDto()
            {
                Id = Guid.Parse("4e0545d5-d160-4d9b-8efe-6d0606d18335"),
                ProductName = "product name 2",
                Price = 13,
                CreatorName = "creator name 2",
                Status = ProductStatus.approved,
            },
              new ProductDto()
            {
                Id = Guid.Parse("6dfe1dc8-daec-401b-a691-e6ad85f949cf"),
                ProductName = "product name 3",
                Price = 15,
                CreatorName = "creator name 3",
                Status = ProductStatus.approved,
            }
        };

        _productRepositoryMock
          .Setup(pr => pr.GetProductsCountAsync())
          .ReturnsAsync(productsCount);

        _productRepositoryMock
            .Setup(pr => pr.GetDtosOf_ProductsAsync(productsPerPage * pageIndex, productsPerPage))
            .ReturnsAsync(productsDtos);

        ProductService productService = new ProductService(_productRepositoryMock.Object);


        //Act
        List<ProductViewModel> products = (await productService.GetProductsAsync(pageIndex: pageIndex)).ToList();


        //Assert
        Assert.That(products, Is.Not.Empty);

        for (int i = 0; i < products.Count; i++)
        {
            Assert.That(products[i].Id, Is.EqualTo(productsDtos[i].Id));
            Assert.That(products[i].ProductName, Is.EqualTo(productsDtos[i].ProductName));
            Assert.That(products[i].Price, Is.EqualTo(productsDtos[i].Price.ToString("f2")));
            Assert.That(products[i].CreatorName, Is.EqualTo(productsDtos[i].CreatorName));
        }

        Assert.That(productService.ProductPageIndex, Is.EqualTo(pageIndex));
    }

    //GetProductsAsync tests>



    //<GetIdsAndNamesOfApprovedProductsWithActiveSaleOrdersCreatedByUserAsync tests

    [Test]
    public async Task GetIdsAndNamesOfApprovedProductsWithActiveSaleOrdersCreatedByUserAsync_MustReturnAStringNameDictionary_WhenThereAreSuchProducts()
    {
        //Arrange
        string userId = "products creator id";

        List<ProductListItemDto> productListItemDtos = new List<ProductListItemDto>()
        {
            new ProductListItemDto()
            {
                Id = Guid.Parse("8118e3a6-63b4-474e-a62d-5cf549af3568"),
                ProductName = "product name 1"
            },
             new ProductListItemDto()
            {
                Id = Guid.Parse("4e0545d5-d160-4d9b-8efe-6d0606d18335"),
                ProductName = "product name 2"
            },
              new ProductListItemDto()
            {
                Id = Guid.Parse("6dfe1dc8-daec-401b-a691-e6ad85f949cf"),
                ProductName = "product name 3"
            }
        };

        _productRepositoryMock
            .Setup(pr => pr.GetProductListItemDtosOf_ApprovedProductsWithActiveSaleOrdersCreatedByUserAsync(userId))
            .ReturnsAsync(productListItemDtos);

        ProductService productService = new ProductService(_productRepositoryMock.Object);


        //Act
        Dictionary<string, string> productsIdsAndNames = await productService.GetIdsAndNamesOfApprovedProductsWithActiveSaleOrdersCreatedByUserAsync(userId: userId);


        //Assert
        Assert.That(productsIdsAndNames, Is.Not.Empty);

        for (int i = 0; i < productsIdsAndNames.Count; i++)
        {
            Guid id = productListItemDtos[i].Id;
            string name = productListItemDtos[i].ProductName;
            Assert.That(productsIdsAndNames[id.ToString()], Is.EqualTo(name));
        }
    }
    //GetIdsAndNamesOfApprovedProductsWithActiveSaleOrdersCreatedByUserAsync tests>



    //<GetDetailsForApprovedProductWithActiveSellOrdersAsync tests

    private ProductDetailsDto GetApprovedProductWithActiveSaleOrders()
    {
        ProductDetailsDto productDto = new ProductDetailsDto()
        {
            Id = Guid.Parse("8118e3a6-63b4-474e-a62d-5cf549af3568"),
            ProductName = "product name",
            Price = 11,
            CreatorName = "creator name",
            Status = ProductStatus.approved,

            Description = "description",
            FirstSellOrderCreationDate = new DateTime(2000, 6, 20),
            LastSellOrderCreationDate = new DateTime(2001, 7, 22),
            ActiveSellOrdersCount = ApplicationConstants.ProductMaxActiveSellOrders
        };

        return productDto;
    }

    [Test]
    public async Task GetDetailsForApprovedProductWithActiveSellOrdersAsync_MustReturnNull_WhenThereIsNoSuchProduct()
    {
        //Arrange  
        ProductDetailsDto productDto = GetApprovedProductWithActiveSaleOrders();

        _productRepositoryMock
            .Setup(pr => pr.GetProductDetailsDtoOf_ApprovedProductWithActiveSellOrdersAsync(productDto.Id))
            .ReturnsAsync((ProductDetailsDto)null);

        ProductService productService = new ProductService(productRepository: _productRepositoryMock.Object);


        //Act
        ProductDetailsViewModel? product = await productService.GetDetailsForApprovedProductWithActiveSellOrdersAsync(productId: productDto.Id);


        //Assert
        Assert.That(product, Is.Null);

    }


    [Test]
    public async Task GetDetailsForApprovedProductWithActiveSellOrdersAsync_MustReturnProductDetails_WhenSuchProductExists()
    {
        //Arrange  
        ProductDetailsDto productDto = GetApprovedProductWithActiveSaleOrders();

        _productRepositoryMock
            .Setup(pr => pr.GetProductDetailsDtoOf_ApprovedProductWithActiveSellOrdersAsync(productDto.Id))
            .ReturnsAsync(productDto);

        ProductService productService = new ProductService(productRepository: _productRepositoryMock.Object);


        //Act
        ProductDetailsViewModel? product = await productService.GetDetailsForApprovedProductWithActiveSellOrdersAsync(productId: productDto.Id);


        //Assert
        Assert.That(product, Is.Not.Null);

        Assert.That(productDto.Id, Is.EqualTo(product.Id));
        Assert.That(productDto.ProductName, Is.EqualTo(product.ProductName));
        Assert.That(productDto.Price.ToString("f2"), Is.EqualTo(product.Price));
        Assert.That(productDto.CreatorName, Is.EqualTo(product.CreatorName));        
        Assert.That(productDto.Description, Is.EqualTo(product.Description));
        Assert.That(productDto.FirstSellOrderCreationDate, Is.EqualTo(DateTime.Parse(product.FirstSellOrderCreationDate)));
        Assert.That(productDto.LastSellOrderCreationDate, Is.EqualTo(DateTime.Parse(product.LastSellOrderCreationDate)));
        Assert.That(productDto.ActiveSellOrdersCount, Is.EqualTo(product.SellOrdersCount));
    }

    //GetDetailsForApprovedProductWithActiveSellOrdersAsync tests>



    //< GetProductsCreatedByUserAsync tests

    [Test]
    public async Task GetProductsCreatedByUserAsync_MustReturnEmptyCollection_WhenTheUserHasNoProducts()
    {
        //Arrange
        int pageIndex = 5;
        int productsCount = 0;
        string userId = "id of user who has no products";

        _productRepositoryMock
            .Setup(pr => pr.GetProductsCountCreatedByUserAsync(userId))
            .ReturnsAsync(productsCount);

        ProductService productService = new ProductService(_productRepositoryMock.Object);


        //Act
        IEnumerable<MyProductViewModel> products = await productService.GetProductsCreatedByUserAsync(pageIndex: pageIndex, userId: userId);


        //Assert
        Assert.That(products, Is.Empty);
    }



    [Test]
    public async Task GetProductsCreatedByUserAsync_MustReturnNonEmptyCollection_WhenTheUserHasProducts()
    {
        //Arrange
        int productsPerPage = ApplicationConstants.ProductsPerPage;
        int pageIndex = 5;
        int productsCount = productsPerPage * pageIndex * 2;
        string userId = "id of user who has products";

        List<ProductDto> productsDtos = new List<ProductDto>()
        {
            new ProductDto()
            {
                Id = Guid.Parse("8118e3a6-63b4-474e-a62d-5cf549af3568"),
                ProductName = "product name 1",
                Price = 11,
                CreatorName = "creator name 1",
                Status = ProductStatus.approved,
            },
             new ProductDto()
            {
                Id = Guid.Parse("4e0545d5-d160-4d9b-8efe-6d0606d18335"),
                ProductName = "product name 2",
                Price = 13,
                CreatorName = "creator name 2",
                Status = ProductStatus.approved,
            },
              new ProductDto()
            {
                Id = Guid.Parse("6dfe1dc8-daec-401b-a691-e6ad85f949cf"),
                ProductName = "product name 3",
                Price = 15,
                CreatorName = "creator name 3",
                Status = ProductStatus.approved,
            }
        };

        _productRepositoryMock
          .Setup(pr => pr.GetProductsCountCreatedByUserAsync(userId))
          .ReturnsAsync(productsCount);

        _productRepositoryMock
            .Setup(pr => pr.GetDtosOf_ProductsCreatedByUserAsync(userId, productsPerPage * pageIndex, productsPerPage))
            .ReturnsAsync(productsDtos);

        ProductService productService = new ProductService(_productRepositoryMock.Object);


        //Act
        List<MyProductViewModel> products = (await productService.GetProductsCreatedByUserAsync(pageIndex: pageIndex, userId: userId)).ToList();


        //Assert
        Assert.That(products, Is.Not.Empty);

        for (int i = 0; i < products.Count; i++)
        {
            Assert.That(products[i].Id, Is.EqualTo(productsDtos[i].Id));
            Assert.That(products[i].ProductName, Is.EqualTo(productsDtos[i].ProductName));
            Assert.That(products[i].Price, Is.EqualTo(productsDtos[i].Price.ToString("f2")));
            Assert.That(products[i].ProductStatus, Is.EqualTo(productsDtos[i].Status.ToString()));
            Assert.That(products[i].CreatorName, Is.EqualTo(productsDtos[i].CreatorName));
        }

        Assert.That(productService.ProductPageIndex, Is.EqualTo(pageIndex));
    }

    //GetProductsCreatedByUserAsync tests>



    //<GetDetailsForProductAsync tests

    [Test]
    public async Task GetDetailsForProductAsync_MustReturnNull_WhenThereIsNoSuchProductId()
    {
        //Arrange  
        ProductDetailsDto productDto = GetApprovedProductWithActiveSaleOrders();

        _productRepositoryMock
            .Setup(pr => pr.GetProductDetailsDtoAsync(productDto.Id))
            .ReturnsAsync((ProductDetailsDto)null);

        ProductService productService = new ProductService(productRepository: _productRepositoryMock.Object);


        //Act
        MyProductDetailsViewModel? product = await productService.GetDetailsForProductAsync(productId: productDto.Id);


        //Assert
        Assert.That(product, Is.Null);

    }


    [Test]
    public async Task GetDetailsForProductAsync_MustReturnProductDetails_WhenSuchProductIdExists()
    {
        //Arrange  
        ProductDetailsDto productDto = GetApprovedProductWithActiveSaleOrders();

        _productRepositoryMock
            .Setup(pr => pr.GetProductDetailsDtoAsync(productDto.Id))
            .ReturnsAsync(productDto);

        ProductService productService = new ProductService(productRepository: _productRepositoryMock.Object);


        //Act
        MyProductDetailsViewModel? product = await productService.GetDetailsForProductAsync(productId: productDto.Id);


        //Assert
        Assert.That(product, Is.Not.Null);

        Assert.That(productDto.Id, Is.EqualTo(product.Id));
        Assert.That(productDto.ProductName, Is.EqualTo(product.ProductName));
        Assert.That(productDto.Price.ToString("f2"), Is.EqualTo(product.Price));
        Assert.That(productDto.CreatorName, Is.EqualTo(product.CreatorName));
        Assert.That(productDto.Description, Is.EqualTo(product.Description));      
        Assert.That(productDto.ActiveSellOrdersCount, Is.EqualTo(product.ActiveSellOrdersCount));
        Assert.That(productDto.Status.ToString(), Is.EqualTo(product.ProductStatus));
    }
    //GetDetailsForProductAsync tests>



    //<GetProductActiveSellOrdersCountAsync tests
    [Test]
    public async Task GetProductActiveSellOrdersCountAsync_MustReturnTheCountOfActiveSellOrdersForTheProduct()
    {
        //Arrange          
        Guid productId = Guid.Parse("8118e3a6-63b4-474e-a62d-5cf549af3568");
        int productActiveSaleOrdersCount = ApplicationConstants.ProductMaxActiveSellOrders;

        _productRepositoryMock
            .Setup(pr => pr.GetProductActiveSellOrdersCountAsync(productId))
            .ReturnsAsync(productActiveSaleOrdersCount);

        ProductService productService = new ProductService(productRepository: _productRepositoryMock.Object);


        //Act
        int productActiveSaleOrdersCountResult = await productService.GetProductActiveSellOrdersCountAsync(productId: productId);


        //Assert
        Assert.That(productActiveSaleOrdersCountResult, Is.EqualTo(productActiveSaleOrdersCount));
    }
    //GetProductActiveSellOrdersCountAsync tests>



    //<GetUpdatedProductModelAsync tests

    [Test]
    public async Task GetUpdatedProductModelAsync_MustReturnNull_WhenTheProductDoesNotExist()
    {
        //Arrange          
        Guid productId = Guid.Parse("8118e3a6-63b4-474e-a62d-5cf549af3568");

        _productRepositoryMock
            .Setup(pr => pr.GetProductToUpdateAsync(productId))
            .ReturnsAsync((Product_UpdateProductDto)null);

        ProductService productService = new ProductService(productRepository: _productRepositoryMock.Object);


        //Act
        UpdatedProductModel? updatedProduct = await productService.GetUpdatedProductModelAsync(productId: productId);


        //Assert
        Assert.That(updatedProduct, Is.Null);
    }

    [Test]
    public async Task GetUpdatedProductModelAsync_MustReturnModel_WhenTheProductExists()
    {
        //Arrange       
        Product_UpdateProductDto updatedProductDto = new Product_UpdateProductDto()
        {
            Id = Guid.Parse("4e0545d5-d160-4d9b-8efe-6d0606d18335"),
            Name = "product name",
            Price = 13,
            Description = "description"
        };

        _productRepositoryMock
            .Setup(pr => pr.GetProductToUpdateAsync(updatedProductDto.Id))
            .ReturnsAsync(updatedProductDto);

        ProductService productService = new ProductService(productRepository: _productRepositoryMock.Object);


        //Act
        UpdatedProductModel? updatedProduct = await productService.GetUpdatedProductModelAsync(productId: updatedProductDto.Id);


        //Assert
        Assert.That(updatedProduct, Is.Not.Null);

        Assert.That(updatedProductDto.Id, Is.EqualTo(updatedProduct.Id));
        Assert.That(updatedProductDto.Name, Is.EqualTo(updatedProduct.ProductName));
        Assert.That(updatedProductDto.Price, Is.EqualTo(updatedProduct.Price));
        Assert.That(updatedProductDto.Description, Is.EqualTo(updatedProduct.Description));
    }
    //GetUpdatedProductModelAsync tests>


    //<GetDeletedProductModelAsync tests

    [Test]
    public async Task GetDeletedProductModelAsync_MustReturnNull_WhenTheProductDoesNotExist()
    {
        //Arrange          
        Guid productId = Guid.Parse("8118e3a6-63b4-474e-a62d-5cf549af3568");

        _productRepositoryMock
            .Setup(pr => pr.GetProductToDeleteAsync(productId))
            .ReturnsAsync((Product_DeleteProductDto)null);

        ProductService productService = new ProductService(productRepository: _productRepositoryMock.Object);


        //Act
        DeletedProductModel? deletedProduct = await productService.GetDeletedProductModelAsync(productId: productId);


        //Assert
        Assert.That(deletedProduct, Is.Null);
    }

    [Test]
    public async Task GetDeletedProductModelAsync_MustReturnModel_WhenTheProductExists()
    {
        //Arrange       
        Product_DeleteProductDto deletedProductDto = new Product_DeleteProductDto()
        {
            Id = Guid.Parse("4e0545d5-d160-4d9b-8efe-6d0606d18335"),
            Name = "product name"           
        };

        _productRepositoryMock
            .Setup(pr => pr.GetProductToDeleteAsync(deletedProductDto.Id))
            .ReturnsAsync(deletedProductDto);

        ProductService productService = new ProductService(productRepository: _productRepositoryMock.Object);


        //Act
        DeletedProductModel? deletedProduct = await productService.GetDeletedProductModelAsync(productId: deletedProductDto.Id);


        //Assert
        Assert.That(deletedProduct, Is.Not.Null);

        Assert.That(deletedProductDto.Id, Is.EqualTo(deletedProduct.ProductId));
        Assert.That(deletedProductDto.Name, Is.EqualTo(deletedProduct.ProductName));       
    }
    //GetDeletedProductModelAsync tests>



    //<GetManagedProductModelAsync tests

    [Test]
    public async Task GetManagedProductModelAsync_MustReturnNull_WhenTheProductDoesNotExist()
    {
        //Arrange          
        Guid productId = Guid.Parse("8118e3a6-63b4-474e-a62d-5cf549af3568");

        _productRepositoryMock
            .Setup(pr => pr.GetProductToManageAsync(productId))
            .ReturnsAsync((Product_ManageProductDto)null);

        ProductService productService = new ProductService(productRepository: _productRepositoryMock.Object);


        //Act
        ManagedProductModel? managedProduct = await productService.GetManagedProductModelAsync(productId: productId);


        //Assert
        Assert.That(managedProduct, Is.Null);
    }

    [Test]
    public async Task GetManagedProductModelAsync_MustReturnModel_WhenTheProductExists()
    {
        //Arrange       
        Product_ManageProductDto managedProductDto = new Product_ManageProductDto()
        {
            Id = Guid.Parse("4e0545d5-d160-4d9b-8efe-6d0606d18335"),
            Name = "product name",
            Status = ProductStatus.inspection
        };

        _productRepositoryMock
            .Setup(pr => pr.GetProductToManageAsync(managedProductDto.Id))
            .ReturnsAsync(managedProductDto);

        ProductService productService = new ProductService(productRepository: _productRepositoryMock.Object);


        //Act
        ManagedProductModel? managedProduct = await productService.GetManagedProductModelAsync(productId: managedProductDto.Id);


        //Assert
        Assert.That(managedProduct, Is.Not.Null);

        Assert.That(managedProductDto.Id, Is.EqualTo(managedProduct.Id));
        Assert.That(managedProductDto.Name, Is.EqualTo(managedProduct.Name));
        Assert.That(managedProductDto.Status, Is.EqualTo(managedProduct.Status));
    }
    //GetManagedProductModelAsync tests>



    //<GetProductNameAsync tests
    [Test]
    public async Task GetProductNameAsync_MustReturnNull_WhenTheProductDoesNotExist()
    {
        //Arrange
        Guid productId = Guid.Parse("4e0545d5-d160-4d9b-8efe-6d0606d18335");        

        _productRepositoryMock
            .Setup(pr => pr.GetProductNameAsync(productId))
            .ReturnsAsync((string)null);

        ProductService productService = new ProductService(productRepository: _productRepositoryMock.Object);


        //Act
        string? productName = await productService.GetProductNameAsync(productId: productId);


        //Assert
        Assert.That(productName, Is.Null);
    }

    [Test]
    public async Task GetProductNameAsync_MustReturnProductName_WhenTheProductExists()
    {
        //Arrange
        Guid productId = Guid.Parse("4e0545d5-d160-4d9b-8efe-6d0606d18335");
        string productName = "product name";

        _productRepositoryMock
            .Setup(pr => pr.GetProductNameAsync(productId))
            .ReturnsAsync(productName);

        ProductService productService = new ProductService(productRepository: _productRepositoryMock.Object);


        //Act
        string? productNameResult = await productService.GetProductNameAsync(productId: productId);


        //Assert
        Assert.That(productNameResult, Is.Not.Null);
        Assert.That(productNameResult, Is.EqualTo(productName));
    }

    //GetProductNameAsync tests>
}
