using Moq;
using TradingApp.Data.Models;
using TradingApp.Data.Repository.Interfaces;
using TradingApp.GCommon;
using TradingApp.GCommon.Enums;
using TradingApp.GCommon.ErrorCodes;
using TradingApp.Services.Core;


namespace TradingApp.Services.Tests;

public class ProductOperationsServiceTests
{
    private Mock<IProductRepository> _productRepositoryMock;
    private Mock<IUserRepository> _userRepositoryMock;
    [SetUp]
    public void Setup()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
    }

    //<AddProductAsync tests

    private void SetupValidScenario_ForAddProductAsync()
    {
        _userRepositoryMock
            .Setup(ur => ur.DoesUserExistAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        _productRepositoryMock
            .Setup(pr => pr.DoesProductCreatedByUserExistAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(false);
    }


    [Test]
    public async Task AddProductAsync_MustReturnErrorCodeUserNotFound_WhenTheUserIdDoesNotExist()
    {
        //Arrange
        string name = "product name 1";
        string description = "product description 1";
        decimal price = 12;
        string creatorId = "product creator Id 1";

        SetupValidScenario_ForAddProductAsync();
        _userRepositoryMock
          .Setup(ur => ur.DoesUserExistAsync(It.IsAny<string>()))
          .ReturnsAsync(false);

        ProductOperationsService productOperationsService = new ProductOperationsService(userRepository: _userRepositoryMock.Object, productRepository: _productRepositoryMock.Object);


        //Act
        Result result = await productOperationsService.AddProductAsync(name: name, description: description, price: price, creatorId: creatorId);


        //Assert
        Assert.That(result.Success, Is.EqualTo(false));
        Assert.That(result.ErrorCode, Is.EqualTo(UserErrorCodes.UserNotFound));
    }

    [Test]
    public async Task AddProductAsync_MustReturnErrorCodeProductWithSameNameAlreadyExists_WhenTheUserHasCreatedAnotherProductWithTheSameName()
    {
        //Arrange
        string name = "product name 1";
        string description = "product description 1";
        decimal price = 12;
        string creatorId = "product creator Id 1";

        SetupValidScenario_ForAddProductAsync();
        _productRepositoryMock
          .Setup(pr => pr.DoesProductCreatedByUserExistAsync(It.IsAny<string>(), It.IsAny<string>()))
          .ReturnsAsync(true);

        ProductOperationsService productOperationsService = new ProductOperationsService(userRepository: _userRepositoryMock.Object, productRepository: _productRepositoryMock.Object);


        //Act
        Result result = await productOperationsService.AddProductAsync(name: name, description: description, price: price, creatorId: creatorId);


        //Assert
        Assert.That(result.Success, Is.EqualTo(false));
        Assert.That(result.ErrorCode, Is.EqualTo(ProductErrorCodes.ProductWithSameNameAlreadyExists));
    }

    [Test]
    public async Task AddProductAsync_MustCreateProduct_WhenAllChecksPass()
    {
        //Arrange
        string name = "product name 1";
        string description = "product description 1";
        decimal price = 12;
        string creatorId = "product creator Id 1";
        ProductStatus status = ApplicationConstants.CreatedProductDefaultStatus;

        Product product = new Product()
        {
            Name = name,
            Description = description,
            Price = price,
            CreatorId = creatorId,
            Status = status
        };

        SetupValidScenario_ForAddProductAsync();

        ProductOperationsService productOperationsService = new ProductOperationsService(userRepository: _userRepositoryMock.Object, productRepository: _productRepositoryMock.Object);


        //Act
        Result result = await productOperationsService.AddProductAsync(name: name, description: description, price: price, creatorId: creatorId);


        //Assert
        Assert.That(result.Success, Is.EqualTo(true));
        Assert.That(result.ErrorCode, Is.EqualTo(string.Empty));

        _productRepositoryMock
            .Verify(pr => pr.CreateProductAsync(
                It.Is<Product>(p => p.Name == name && p.Description == description
                && p.Price == price && p.CreatorId == creatorId && p.Status == status))
            , Times.Once);
    }

    //AddProductAsync tests>



    //<UpdateProductAsync tests

    private (Product, Product) SetupValidScenario_ForUpdateProductAsync()
    {
        Guid productId = Guid.Parse("8118e3a6-63b4-474e-a62d-5cf549af3568");
        string productCreatorId = "product creator Id";
        string oldProductName = "old product name";
        string oldProductDescription = "old product description";
        decimal oldProductPrice = 13;
        string newProductName = "new product name";
        string newProductDescription = "new product description";
        decimal newProductPrice = 13.5M;

        Product oldProduct = new Product()
        {
            Id = productId,
            CreatorId = productCreatorId,
            Name = oldProductName,
            Description = oldProductDescription,
            Price = oldProductPrice,
        };

        Product newProduct = new Product()
        {
            Id = productId,
            CreatorId = productCreatorId,
            Name = newProductName,
            Description = newProductDescription,
            Price = newProductPrice,
        };

        _productRepositoryMock
            .Setup(pr => pr.GetProductByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(oldProduct);

        _userRepositoryMock
            .Setup(ur => ur.DoesCreatorHaveOtherProductsWithTheSameNameAsync(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<string>()))
            .ReturnsAsync(false);

        _productRepositoryMock
           .Setup(pr => pr.DoesProductHaveNonResolvedReports(It.IsAny<Guid>()))
           .ReturnsAsync(false);

        _productRepositoryMock
          .Setup(pr => pr.GetProductActiveSellOrdersCountAsync(It.IsAny<Guid>()))
          .ReturnsAsync(0);

        return (oldProduct, newProduct);
    }

    [Test]
    public async Task UpdateProductAsync_MustReturnErrorCodeProductNotFound_WhenTheProductDoesNotExist()
    {
        //Arrange
        (Product oldProduct, Product newProduct) = SetupValidScenario_ForUpdateProductAsync();

        _productRepositoryMock
           .Setup(pr => pr.GetProductByIdAsync(It.IsAny<Guid>()))
           .ReturnsAsync((Product)null);

        ProductOperationsService productOperationsService = new ProductOperationsService(userRepository: _userRepositoryMock.Object, productRepository: _productRepositoryMock.Object);


        //Act
        Result result = await productOperationsService.UpdateProductAsync(id: newProduct.Id, name: newProduct.Name, description: newProduct.Description, price: newProduct.Price, creatorId: newProduct.CreatorId);


        //Assert
        Assert.That(result.Success, Is.EqualTo(false));
        Assert.That(result.ErrorCode, Is.EqualTo(ProductErrorCodes.ProductNotFound));
    }

    [Test]
    public async Task UpdateProductAsync_MustReturnErrorCodeProductInvalidCreator_WhenTheProductWasCreatedByAnotherUser()
    {
        //Arrange
        (Product oldProduct, Product newProduct) = SetupValidScenario_ForUpdateProductAsync();
        string userId = "another user Id";

        ProductOperationsService productOperationsService = new ProductOperationsService(userRepository: _userRepositoryMock.Object, productRepository: _productRepositoryMock.Object);


        //Act
        Result result = await productOperationsService.UpdateProductAsync(id: newProduct.Id, name: newProduct.Name, description: newProduct.Description, price: newProduct.Price, creatorId: userId);


        //Assert
        Assert.That(result.Success, Is.EqualTo(false));
        Assert.That(result.ErrorCode, Is.EqualTo(ProductErrorCodes.ProductInvalidCreator));
    }

    [Test]
    public async Task UpdateProductAsync_MustReturnErrorCodeProductWithSameNameAlreadyExists_WhenTheUserHasAnotherProductWithTheSameName()
    {
        //Arrange
        (Product oldProduct, Product newProduct) = SetupValidScenario_ForUpdateProductAsync();

        _userRepositoryMock
           .Setup(ur => ur.DoesCreatorHaveOtherProductsWithTheSameNameAsync(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<string>()))
           .ReturnsAsync(true);

        ProductOperationsService productOperationsService = new ProductOperationsService(userRepository: _userRepositoryMock.Object, productRepository: _productRepositoryMock.Object);


        //Act
        Result result = await productOperationsService.UpdateProductAsync(id: newProduct.Id, name: newProduct.Name, description: newProduct.Description, price: newProduct.Price, creatorId: newProduct.CreatorId);


        //Assert
        Assert.That(result.Success, Is.EqualTo(false));
        Assert.That(result.ErrorCode, Is.EqualTo(ProductErrorCodes.ProductWithSameNameAlreadyExists));
    }

    [Test]
    public async Task UpdateProductAsync_MustReturnErrorCodeProductHasNonResolvedReports_WhenTheProductHasNonResolvedReports()
    {
        //Arrange
        (Product oldProduct, Product newProduct) = SetupValidScenario_ForUpdateProductAsync();

        _productRepositoryMock
           .Setup(pr => pr.DoesProductHaveNonResolvedReports(It.IsAny<Guid>()))
           .ReturnsAsync(true);

        ProductOperationsService productOperationsService = new ProductOperationsService(userRepository: _userRepositoryMock.Object, productRepository: _productRepositoryMock.Object);


        //Act
        Result result = await productOperationsService.UpdateProductAsync(id: newProduct.Id, name: newProduct.Name, description: newProduct.Description, price: newProduct.Price, creatorId: newProduct.CreatorId);


        //Assert
        Assert.That(result.Success, Is.EqualTo(false));
        Assert.That(result.ErrorCode, Is.EqualTo(ProductErrorCodes.ProductHasNonResolvedReports));
    }

    [Test]
    public async Task UpdateProductAsync_MustReturnErrorCodeProductHasActiveSaleOrders_WhenTheProductHasActiveSaleOrders()
    {
        //Arrange
        (Product oldProduct, Product newProduct) = SetupValidScenario_ForUpdateProductAsync();

        _productRepositoryMock
           .Setup(pr => pr.GetProductActiveSellOrdersCountAsync(It.IsAny<Guid>()))
           .ReturnsAsync(1);

        ProductOperationsService productOperationsService = new ProductOperationsService(userRepository: _userRepositoryMock.Object, productRepository: _productRepositoryMock.Object);


        //Act
        Result result = await productOperationsService.UpdateProductAsync(id: newProduct.Id, name: newProduct.Name, description: newProduct.Description, price: newProduct.Price, creatorId: newProduct.CreatorId);


        //Assert
        Assert.That(result.Success, Is.EqualTo(false));
        Assert.That(result.ErrorCode, Is.EqualTo(ProductErrorCodes.ProductHasActiveSaleOrders));
    }

    [Test]
    public async Task UpdateProductAsync_MustUpdateProduct_WhenAllChecksPass()
    {
        //Arrange

        (Product oldProduct, Product newProduct) = SetupValidScenario_ForUpdateProductAsync();

        ProductOperationsService productOperationsService = new ProductOperationsService(userRepository: _userRepositoryMock.Object, productRepository: _productRepositoryMock.Object);


        //Act
        Result result = await productOperationsService.UpdateProductAsync(id: newProduct.Id, name: newProduct.Name, description: newProduct.Description, price: newProduct.Price, creatorId: newProduct.CreatorId);


        //Assert
        Assert.That(result.Success, Is.EqualTo(true));
        Assert.That(result.ErrorCode, Is.EqualTo(string.Empty));

        _productRepositoryMock.Verify(pr => pr.UpdateProductAsync(
            It.Is<Product>(p => p.Name == oldProduct.Name && p.Description == oldProduct.Description && p.Price == oldProduct.Price),
            newProduct.Name, newProduct.Description, newProduct.Price),
            Times.Once);
    }

    //UpdateProductAsync tests>



    //<DeleteProductAsync tests
    private Product SetupValidScenario_ForDeleteProductAsync()
    {
        Guid productId = Guid.Parse("8118e3a6-63b4-474e-a62d-5cf549af3568");
        string productCreatorId = "product creator Id";
        string productName = "product name";
        string productDescription = "product description";
        decimal productPrice = 13;


        Product product = new Product()
        {
            Id = productId,
            CreatorId = productCreatorId,
            Name = productName,
            Description = productDescription,
            Price = productPrice,
        };

        _productRepositoryMock
            .Setup(ur => ur.GetProductByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(product);

        _productRepositoryMock
            .Setup(pr => pr.DoesProductHaveNonResolvedReports(It.IsAny<Guid>()))
            .ReturnsAsync(false);

        return product;
    }

    [Test]
    public async Task DeleteProductAsync_MustReturnErrorCodeProductNotFound_WhenTheProductDoesNotExist()
    {
        //Arrange
        Product product = SetupValidScenario_ForDeleteProductAsync();
        _productRepositoryMock
          .Setup(ur => ur.GetProductByIdAsync(It.IsAny<Guid>()))
          .ReturnsAsync((Product)null);

        ProductOperationsService productOperationsService = new ProductOperationsService(userRepository: _userRepositoryMock.Object, productRepository: _productRepositoryMock.Object);
        
        
        //Act
        Result result = await productOperationsService.DeleteProductAsync(id: product.Id, creatorId: product.CreatorId);


        //Assert
        Assert.That(result.Success, Is.EqualTo(false));
        Assert.That(result.ErrorCode, Is.EqualTo(ProductErrorCodes.ProductNotFound));
    }

    [Test]
    public async Task DeleteProductAsync_MustReturnErrorCodeProductInvalidCreator_WhenTheProductIsCreatedByAnotherCreator()
    {
        //Arrange
        Product product = SetupValidScenario_ForDeleteProductAsync();
        string anotherUserId = "another user id";

        ProductOperationsService productOperationsService = new ProductOperationsService(userRepository: _userRepositoryMock.Object, productRepository: _productRepositoryMock.Object);


        //Act
        Result result = await productOperationsService.DeleteProductAsync(id: product.Id, creatorId: anotherUserId);


        //Assert
        Assert.That(result.Success, Is.EqualTo(false));
        Assert.That(result.ErrorCode, Is.EqualTo(ProductErrorCodes.ProductInvalidCreator));
    }

    [Test]
    public async Task DeleteProductAsync_MustReturnErrorCodeProductHasNonResolvedReports_WhenTheProductHasNonResolvedReports()
    {
        //Arrange
        Product product = SetupValidScenario_ForDeleteProductAsync();
        _productRepositoryMock
           .Setup(ur => ur.DoesProductHaveNonResolvedReports(It.IsAny<Guid>()))
           .ReturnsAsync(true);

        ProductOperationsService productOperationsService = new ProductOperationsService(userRepository: _userRepositoryMock.Object, productRepository: _productRepositoryMock.Object);


        //Act
        Result result = await productOperationsService.DeleteProductAsync(id: product.Id, creatorId: product.CreatorId);


        //Assert
        Assert.That(result.Success, Is.EqualTo(false));
        Assert.That(result.ErrorCode, Is.EqualTo(ProductErrorCodes.ProductHasNonResolvedReports));
    }


    [Test]
    public async Task DeleteProductAsync_MustDeleteProduct_WhenAllChecksPass()
    {
        //Arrange
        Product product = SetupValidScenario_ForDeleteProductAsync();
      
        ProductOperationsService productOperationsService = new ProductOperationsService(userRepository: _userRepositoryMock.Object, productRepository: _productRepositoryMock.Object);


        //Act
        Result result = await productOperationsService.DeleteProductAsync(id: product.Id, creatorId: product.CreatorId);


        //Assert
        Assert.That(result.Success, Is.EqualTo(true));
        Assert.That(result.ErrorCode, Is.EqualTo(string.Empty));
        _productRepositoryMock
            .Verify(pr => pr.DeleteProductAsync(
                It.Is<Product>(p => p.Id == product.Id))
            , Times.Once);
    }

    //DeleteProductAsync tests>



    //<ChangeProductStatusAsync tests
    private (Product, ProductStatus) SetupValidScenario_ForChangeProductStatusAsync()
    {
        Guid productId = Guid.Parse("8118e3a6-63b4-474e-a62d-5cf549af3568");
        ProductStatus oldStatus = ProductStatus.inspection;
        ProductStatus newStatus = ProductStatus.approved;

        Product product = new Product()
        {
            Id = productId,
            Status = oldStatus
        };

        _productRepositoryMock
            .Setup(ur => ur.GetProductByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(product);
          
        return (product, newStatus);
    }

    [Test]
    public async Task ChangeProductStatusAsync_MustReturnErrorCodeProductNotFound_WhenTheProductDoesNotExist()
    {
        //Arrange
        (Product product, ProductStatus newStatus) = SetupValidScenario_ForChangeProductStatusAsync();
        _productRepositoryMock
          .Setup(ur => ur.GetProductByIdAsync(It.IsAny<Guid>()))
          .ReturnsAsync((Product)null);

        ProductOperationsService productOperationsService = new ProductOperationsService(userRepository: _userRepositoryMock.Object, productRepository: _productRepositoryMock.Object);


        //Act
        Result result = await productOperationsService.ChangeProductStatusAsync(id: product.Id, productStatus: newStatus);


        //Assert
        Assert.That(result.Success, Is.EqualTo(false));
        Assert.That(result.ErrorCode, Is.EqualTo(ProductErrorCodes.ProductNotFound));
    }

    [Test]
    public async Task ChangeProductStatusAsync_MustReturnErrorCodeProductInvalidStatus_WhenTheProductStatusIsNotChanged()
    {
        //Arrange
        (Product product, ProductStatus newStatus) = SetupValidScenario_ForChangeProductStatusAsync();
        newStatus = product.Status;

        ProductOperationsService productOperationsService = new ProductOperationsService(userRepository: _userRepositoryMock.Object, productRepository: _productRepositoryMock.Object);


        //Act
        Result result = await productOperationsService.ChangeProductStatusAsync(id: product.Id, productStatus: newStatus);


        //Assert
        Assert.That(result.Success, Is.EqualTo(false));
        Assert.That(result.ErrorCode, Is.EqualTo(ProductErrorCodes.ProductInvalidStatus));
    }


    [Test]
    public async Task ChangeProductStatusAsync_MustChangeProductStatus_WhenAllChecksPass()
    {
        //Arrange
        (Product product, ProductStatus newStatus) = SetupValidScenario_ForChangeProductStatusAsync();
        
        ProductOperationsService productOperationsService = new ProductOperationsService(userRepository: _userRepositoryMock.Object, productRepository: _productRepositoryMock.Object);


        //Act
        Result result = await productOperationsService.ChangeProductStatusAsync(id: product.Id, productStatus: newStatus);


        //Assert
        Assert.That(result.Success, Is.EqualTo(true));
        Assert.That(result.ErrorCode, Is.EqualTo(string.Empty));
        _productRepositoryMock.Verify(pr => pr.ChangeProductStatusAsync(
            It.Is<Product>(p => p.Id == product.Id), newStatus),
            Times.Once);
    }

    //ChangeProductStatusAsync tests>
}
