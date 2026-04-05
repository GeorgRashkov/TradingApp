using Moq;
using TradingApp.Data.Dtos.Product;
using TradingApp.Data.Dtos.User;
using TradingApp.Data.Repository.Interfaces;
using TradingApp.GCommon;
using TradingApp.GCommon.Enums;
using TradingApp.GCommon.ErrorCodes;
using TradingApp.Services.Core;

namespace TradingApp.Services.Tests;

public class OrderServiceTests
{

    private Mock<ISellOrderRepository> _sellOrderRepositoryMock;
    private Mock<IProductRepository> _productRepositoryMock;
    private Mock<IUserRepository> _userRepositoryMock;
    [SetUp]
    public void Setup()
    {
        _sellOrderRepositoryMock = new Mock<ISellOrderRepository>();
        _productRepositoryMock = new Mock<IProductRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
    }


    //< CanUserCreateSellOrdersOfSpecificProductAsync tests

    private (Product_CreateSellOrderEligibilityDto, User_CreateSellOrderEligibilityDto) SetupValidScenario_ForCanUserCreateSellOrdersOfSpecificProductAsync()
    {
        string productCreatorId = "ProductCreatorId";
        int productActiveSellOrdersCount = ApplicationConstants.ProductMaxActiveSellOrders - 1;
        string productName = "productName1";

        Product_CreateSellOrderEligibilityDto productDto = new Product_CreateSellOrderEligibilityDto()
        {
            CreatorId = productCreatorId,
            Name = productName,
            Status = ProductStatus.approved,
            ActiveSellOrdersCount = productActiveSellOrdersCount
        };

        _productRepositoryMock
            .Setup(orr => orr.GetProductForCreateSellOrderAsync(It.IsAny<Guid>()))
            .ReturnsAsync(productDto);

        int userActiveSellOrdersCount = ApplicationConstants.UserMaxActiveSellOrders - 1;
        User_CreateSellOrderEligibilityDto userDto = new User_CreateSellOrderEligibilityDto()
        {
            UserId = productCreatorId,
            ActiveSellOrdersCount = userActiveSellOrdersCount
        };

        _userRepositoryMock
            .Setup(orr => orr.GetUserForCreateSellOrderAsync(It.IsAny<string>()))
            .ReturnsAsync(userDto);

        return (productDto, userDto);
    }


    [Test]
    public async Task CanUserCreateSellOrdersOfSpecificProductAsync_MustReturnErrorCodeProductNotFound_WhenTheProductDoesNotExist()
    {
        //Arrange
        Guid productId = Guid.Parse("8118e3a6-63b4-474e-a62d-5cf549af3568");
        int ordersCount = 5;

        (Product_CreateSellOrderEligibilityDto productDto, User_CreateSellOrderEligibilityDto userDto) = SetupValidScenario_ForCanUserCreateSellOrdersOfSpecificProductAsync();

        _productRepositoryMock
            .Setup(pr => pr.GetProductForCreateSellOrderAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Product_CreateSellOrderEligibilityDto)null);

        OrderService orderService = new OrderService(sellOrderRepository: _sellOrderRepositoryMock.Object, productRepository: _productRepositoryMock.Object, userRepository: _userRepositoryMock.Object);


        //Act
        Result result = await orderService.CanUserCreateSellOrdersOfSpecificProductAsync(productId: productId, userId: userDto.UserId, ordersCount: ordersCount);


        //Assert
        Assert.That(result.ErrorCode, Is.EqualTo(ProductErrorCodes.ProductNotFound));
    }

    [Test]
    public async Task CanUserCreateSellOrdersOfSpecificProductAsync_MustReturnErrorCodeUserNotFound_WhenTheUserIdDoesNotExist()
    {
        //Arrange
        Guid productId = Guid.Parse("8118e3a6-63b4-474e-a62d-5cf549af3568");
        int ordersCount = 5;

        (Product_CreateSellOrderEligibilityDto productDto, User_CreateSellOrderEligibilityDto userDto) = SetupValidScenario_ForCanUserCreateSellOrdersOfSpecificProductAsync();

        _userRepositoryMock
            .Setup(pr => pr.GetUserForCreateSellOrderAsync(It.IsAny<string>()))
            .ReturnsAsync((User_CreateSellOrderEligibilityDto)null);

        OrderService orderService = new OrderService(sellOrderRepository: _sellOrderRepositoryMock.Object, productRepository: _productRepositoryMock.Object, userRepository: _userRepositoryMock.Object);


        //Act
        Result result = await orderService.CanUserCreateSellOrdersOfSpecificProductAsync(productId: productId, userId: userDto.UserId, ordersCount: ordersCount);


        //Assert
        Assert.That(result.ErrorCode, Is.EqualTo(UserErrorCodes.UserNotFound));
    }



    [Test]
    public async Task CanUserCreateSellOrdersOfSpecificProductAsync_MustReturnErrorCodeProductInvalidCreator_WhenTheProductWasCreatedByAnotherUser()
    {
        //Arrange
        Guid productId = Guid.Parse("8118e3a6-63b4-474e-a62d-5cf549af3568");
        int ordersCount = 5;

        (Product_CreateSellOrderEligibilityDto productDto, User_CreateSellOrderEligibilityDto userDto) = SetupValidScenario_ForCanUserCreateSellOrdersOfSpecificProductAsync();
        userDto.UserId = "AnotherUserId";

        _userRepositoryMock
            .Setup(pr => pr.GetUserForCreateSellOrderAsync(It.IsAny<string>()))
            .ReturnsAsync(userDto);

        OrderService orderService = new OrderService(sellOrderRepository: _sellOrderRepositoryMock.Object, productRepository: _productRepositoryMock.Object, userRepository: _userRepositoryMock.Object);


        //Act
        Result result = await orderService.CanUserCreateSellOrdersOfSpecificProductAsync(productId: productId, userId: userDto.UserId, ordersCount: ordersCount);


        //Assert
        Assert.That(result.ErrorCode, Is.EqualTo(ProductErrorCodes.ProductInvalidCreator));

    }

    [Test]
    public async Task CanUserCreateSellOrdersOfSpecificProductAsync_MustReturnErrorCodeProductInvalidStatus_WhenTheProductIsNotApproved()
    {
        //Arrange
        Guid productId = Guid.Parse("8118e3a6-63b4-474e-a62d-5cf549af3568");
        int ordersCount = 5;

        (Product_CreateSellOrderEligibilityDto productDto, User_CreateSellOrderEligibilityDto userDto) = SetupValidScenario_ForCanUserCreateSellOrdersOfSpecificProductAsync();
        productDto.Status = ProductStatus.inspection;

        _productRepositoryMock
            .Setup(pr => pr.GetProductForCreateSellOrderAsync(It.IsAny<Guid>()))
            .ReturnsAsync(productDto);

        OrderService orderService = new OrderService(sellOrderRepository: _sellOrderRepositoryMock.Object, productRepository: _productRepositoryMock.Object, userRepository: _userRepositoryMock.Object);


        //Act
        Result result = await orderService.CanUserCreateSellOrdersOfSpecificProductAsync(productId: productId, userId: userDto.UserId, ordersCount: ordersCount);


        //Assert
        Assert.That(result.ErrorCode, Is.EqualTo(ProductErrorCodes.ProductInvalidStatus));

    }

    [Test]
    public async Task CanUserCreateSellOrdersOfSpecificProductAsync_MustReturnErrorCodeProductMaxActiveSellOrdersReached_WhenTheProductHasReachedTheMaxNumberOfActiveSellOrders()
    {
        //Arrange
        Guid productId = Guid.Parse("8118e3a6-63b4-474e-a62d-5cf549af3568");
        int ordersCount = 5;

        (Product_CreateSellOrderEligibilityDto productDto, User_CreateSellOrderEligibilityDto userDto) = SetupValidScenario_ForCanUserCreateSellOrdersOfSpecificProductAsync();
        productDto.ActiveSellOrdersCount = ApplicationConstants.ProductMaxActiveSellOrders;

        _productRepositoryMock
            .Setup(pr => pr.GetProductForCreateSellOrderAsync(It.IsAny<Guid>()))
            .ReturnsAsync(productDto);

        OrderService orderService = new OrderService(sellOrderRepository: _sellOrderRepositoryMock.Object, productRepository: _productRepositoryMock.Object, userRepository: _userRepositoryMock.Object);


        //Act
        Result result = await orderService.CanUserCreateSellOrdersOfSpecificProductAsync(productId: productId, userId: userDto.UserId, ordersCount: ordersCount);


        //Assert
        Assert.That(result.ErrorCode, Is.EqualTo(ProductErrorCodes.ProductMaxActiveSellOrdersReached));

    }

    [Test]
    public async Task CanUserCreateSellOrdersOfSpecificProductAsync_MustReturnErrorCodeUserMaxActiveSellOrdersReached_WhenTheUserHasReachedTheMaxNumberOfActiveSellOrders()
    {
        //Arrange
        Guid productId = Guid.Parse("8118e3a6-63b4-474e-a62d-5cf549af3568");
        int ordersCount = 5;

        (Product_CreateSellOrderEligibilityDto productDto, User_CreateSellOrderEligibilityDto userDto) = SetupValidScenario_ForCanUserCreateSellOrdersOfSpecificProductAsync();
        userDto.ActiveSellOrdersCount = ApplicationConstants.UserMaxActiveSellOrders;

        _userRepositoryMock
            .Setup(pr => pr.GetUserForCreateSellOrderAsync(It.IsAny<string>()))
            .ReturnsAsync(userDto);

        OrderService orderService = new OrderService(sellOrderRepository: _sellOrderRepositoryMock.Object, productRepository: _productRepositoryMock.Object, userRepository: _userRepositoryMock.Object);


        //Act
        Result result = await orderService.CanUserCreateSellOrdersOfSpecificProductAsync(productId: productId, userId: userDto.UserId, ordersCount: ordersCount);


        //Assert
        Assert.That(result.ErrorCode, Is.EqualTo(UserErrorCodes.UserMaxActiveSellOrdersReached));

    }


    [Test]
    public async Task CanUserCreateSellOrdersOfSpecificProductAsync_MustReturnNumberOfAllowedSellOrdersToCreate_WhenAllChecksPass()
    {
        //Arrange
        Guid productId = Guid.Parse("8118e3a6-63b4-474e-a62d-5cf549af3568");
        int ordersCount = 5;
        int expectedCountOfSellOrdersToCreate = 2;

        (Product_CreateSellOrderEligibilityDto productDto, User_CreateSellOrderEligibilityDto userDto) = SetupValidScenario_ForCanUserCreateSellOrdersOfSpecificProductAsync();

        userDto.ActiveSellOrdersCount = ApplicationConstants.UserMaxActiveSellOrders - expectedCountOfSellOrdersToCreate;
        productDto.ActiveSellOrdersCount = ApplicationConstants.ProductMaxActiveSellOrders - expectedCountOfSellOrdersToCreate;

        _userRepositoryMock
            .Setup(pr => pr.GetUserForCreateSellOrderAsync(It.IsAny<string>()))
            .ReturnsAsync(userDto);

        OrderService orderService = new OrderService(sellOrderRepository: _sellOrderRepositoryMock.Object, productRepository: _productRepositoryMock.Object, userRepository: _userRepositoryMock.Object);


        //Act
        Result result = await orderService.CanUserCreateSellOrdersOfSpecificProductAsync(productId: productId, userId: userDto.UserId, ordersCount: ordersCount);


        //Assert
        Assert.That(result.ErrorCode, Is.EqualTo(string.Empty));

        Assert.That(result.SuccessMessage, Is.EqualTo(expectedCountOfSellOrdersToCreate.ToString()));

    }

    // CanUserCreateSellOrdersOfSpecificProductAsync tests>



    //< CanUserCancelSellOrdersOfSpecificProductAsync tests

    private (Product_CancelSellOrderEligibilityDto, User_CancelSellOrderEligibilityDto) SetupValidScenario_ForCanUserCancelSellOrdersOfSpecificProductAsync()
    {
        string productCreatorId = "ProductCreatorId";
        int productActiveSellOrdersCount = ApplicationConstants.ProductMaxActiveSellOrders;
        string productName = "productName1";

        Product_CancelSellOrderEligibilityDto productDto = new Product_CancelSellOrderEligibilityDto()
        {
            CreatorId = productCreatorId,
            Name = productName,
            Status = ProductStatus.approved,
            ActiveSellOrdersCount = productActiveSellOrdersCount
        };

        _productRepositoryMock
            .Setup(orr => orr.GetProductForCancelSellOrderAsync(It.IsAny<Guid>()))
            .ReturnsAsync(productDto);

        int userActiveSellOrdersCount = ApplicationConstants.UserMaxActiveSellOrders;
        User_CancelSellOrderEligibilityDto userDto = new User_CancelSellOrderEligibilityDto()
        {
            UserId = productCreatorId,
            ActiveSellOrdersCount = userActiveSellOrdersCount
        };

        _userRepositoryMock
            .Setup(orr => orr.GetUserForCancelSellOrderAsync(It.IsAny<string>()))
            .ReturnsAsync(userDto);

        return (productDto, userDto);
    }


    [Test]
    public async Task CanUserCancelSellOrdersOfSpecificProductAsync_MustReturnErrorCodeProductNotFound_WhenTheProductDoesNotExist()
    {
        //Arrange
        Guid productId = Guid.Parse("8118e3a6-63b4-474e-a62d-5cf549af3568");
        int ordersCount = 5;

        (Product_CancelSellOrderEligibilityDto productDto, User_CancelSellOrderEligibilityDto userDto) = SetupValidScenario_ForCanUserCancelSellOrdersOfSpecificProductAsync();

        _productRepositoryMock
            .Setup(pr => pr.GetProductForCancelSellOrderAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Product_CancelSellOrderEligibilityDto)null);

        OrderService orderService = new OrderService(sellOrderRepository: _sellOrderRepositoryMock.Object, productRepository: _productRepositoryMock.Object, userRepository: _userRepositoryMock.Object);


        //Act
        Result result = await orderService.CanUserCancelSellOrdersOfSpecificProductAsync(productId: productId, userId: userDto.UserId, ordersCount: ordersCount);


        //Assert
        Assert.That(result.ErrorCode, Is.EqualTo(ProductErrorCodes.ProductNotFound));
    }

    [Test]
    public async Task CanUserCancelSellOrdersOfSpecificProductAsync_MustReturnErrorCodeUserNotFound_WhenTheUserIdDoesNotExist()
    {
        //Arrange
        Guid productId = Guid.Parse("8118e3a6-63b4-474e-a62d-5cf549af3568");
        int ordersCount = 5;

        (Product_CancelSellOrderEligibilityDto productDto, User_CancelSellOrderEligibilityDto userDto) = SetupValidScenario_ForCanUserCancelSellOrdersOfSpecificProductAsync();

        _userRepositoryMock
            .Setup(pr => pr.GetUserForCancelSellOrderAsync(It.IsAny<string>()))
            .ReturnsAsync((User_CancelSellOrderEligibilityDto)null);

        OrderService orderService = new OrderService(sellOrderRepository: _sellOrderRepositoryMock.Object, productRepository: _productRepositoryMock.Object, userRepository: _userRepositoryMock.Object);


        //Act
        Result result = await orderService.CanUserCancelSellOrdersOfSpecificProductAsync(productId: productId, userId: userDto.UserId, ordersCount: ordersCount);


        //Assert
        Assert.That(result.ErrorCode, Is.EqualTo(UserErrorCodes.UserNotFound));
    }

    [Test]
    public async Task CanUserCancelSellOrdersOfSpecificProductAsync_MustReturnErrorCodeProductInvalidCreator_WhenTheProductWasCreatedByAnotherUser()
    {
        //Arrange
        Guid productId = Guid.Parse("8118e3a6-63b4-474e-a62d-5cf549af3568");
        int ordersCount = 5;

        (Product_CancelSellOrderEligibilityDto productDto, User_CancelSellOrderEligibilityDto userDto) = SetupValidScenario_ForCanUserCancelSellOrdersOfSpecificProductAsync();
        userDto.UserId = "AnotherUserId";

        _userRepositoryMock
            .Setup(pr => pr.GetUserForCancelSellOrderAsync(It.IsAny<string>()))
            .ReturnsAsync(userDto);

        OrderService orderService = new OrderService(sellOrderRepository: _sellOrderRepositoryMock.Object, productRepository: _productRepositoryMock.Object, userRepository: _userRepositoryMock.Object);


        //Act
        Result result = await orderService.CanUserCancelSellOrdersOfSpecificProductAsync(productId: productId, userId: userDto.UserId, ordersCount: ordersCount);


        //Assert
        Assert.That(result.ErrorCode, Is.EqualTo(ProductErrorCodes.ProductInvalidCreator));

    }

    [Test]
    public async Task CanUserCancelSellOrdersOfSpecificProductAsync_MustReturnErrorCodeProductInvalidStatus_WhenTheProductIsNotApproved()
    {
        //Arrange
        Guid productId = Guid.Parse("8118e3a6-63b4-474e-a62d-5cf549af3568");
        int ordersCount = 5;

        (Product_CancelSellOrderEligibilityDto productDto, User_CancelSellOrderEligibilityDto userDto) = SetupValidScenario_ForCanUserCancelSellOrdersOfSpecificProductAsync();
        productDto.Status = ProductStatus.inspection;

        _productRepositoryMock
            .Setup(pr => pr.GetProductForCancelSellOrderAsync(It.IsAny<Guid>()))
            .ReturnsAsync(productDto);

        OrderService orderService = new OrderService(sellOrderRepository: _sellOrderRepositoryMock.Object, productRepository: _productRepositoryMock.Object, userRepository: _userRepositoryMock.Object);


        //Act
        Result result = await orderService.CanUserCancelSellOrdersOfSpecificProductAsync(productId: productId, userId: userDto.UserId, ordersCount: ordersCount);


        //Assert
        Assert.That(result.ErrorCode, Is.EqualTo(ProductErrorCodes.ProductInvalidStatus));

    }

    [Test]
    public async Task CanUserCancelSellOrdersOfSpecificProductAsync_MustReturnErrorCodeProductHasNoActiveSaleOrders_WhenTheProductHasNoActiveSellOrders()
    {
        //Arrange
        Guid productId = Guid.Parse("8118e3a6-63b4-474e-a62d-5cf549af3568");
        int ordersCount = 5;

        (Product_CancelSellOrderEligibilityDto productDto, User_CancelSellOrderEligibilityDto userDto) = SetupValidScenario_ForCanUserCancelSellOrdersOfSpecificProductAsync();
        productDto.ActiveSellOrdersCount = 0;

        _productRepositoryMock
            .Setup(pr => pr.GetProductForCancelSellOrderAsync(It.IsAny<Guid>()))
            .ReturnsAsync(productDto);

        OrderService orderService = new OrderService(sellOrderRepository: _sellOrderRepositoryMock.Object, productRepository: _productRepositoryMock.Object, userRepository: _userRepositoryMock.Object);


        //Act
        Result result = await orderService.CanUserCancelSellOrdersOfSpecificProductAsync(productId: productId, userId: userDto.UserId, ordersCount: ordersCount);


        //Assert
        Assert.That(result.ErrorCode, Is.EqualTo(ProductErrorCodes.ProductHasNoActiveSaleOrders));

    }


    [Test]
    public async Task CanUserCancelSellOrdersOfSpecificProductAsync_MustReturnNumberOfAllowedSellOrdersToCancel_WhenAllChecksPass()
    {
        //Arrange
        Guid productId = Guid.Parse("8118e3a6-63b4-474e-a62d-5cf549af3568");
        int ordersCount = 5;
        int expectedCountOfSellOrdersToCancel = 2;

        (Product_CancelSellOrderEligibilityDto productDto, User_CancelSellOrderEligibilityDto userDto) = SetupValidScenario_ForCanUserCancelSellOrdersOfSpecificProductAsync();

        productDto.ActiveSellOrdersCount = expectedCountOfSellOrdersToCancel;

        _productRepositoryMock
            .Setup(pr => pr.GetProductForCancelSellOrderAsync(It.IsAny<Guid>()))
            .ReturnsAsync(productDto);

        OrderService orderService = new OrderService(sellOrderRepository: _sellOrderRepositoryMock.Object, productRepository: _productRepositoryMock.Object, userRepository: _userRepositoryMock.Object);


        //Act
        Result result = await orderService.CanUserCancelSellOrdersOfSpecificProductAsync(productId: productId, userId: userDto.UserId, ordersCount: ordersCount);


        //Assert
        Assert.That(result.ErrorCode, Is.EqualTo(string.Empty));

        Assert.That(result.SuccessMessage, Is.EqualTo(expectedCountOfSellOrdersToCancel.ToString()));

    }

    //CanUserCancelSellOrdersOfSpecificProductAsync> tests



    //< CanUserCancelSellOrdersOfSpecificProductAsync tests

    private (Product_BuySellOrderEligibilityDto, User_BuySellOrderEligibilityDto) SetupValidScenario_ForCanUserBuySellOrderOfSpecificProductAsync()
    {
        string productCreatorId = "ProductCreatorId";
        string buyerId = "AnotherUserId";
        int productActiveSellOrdersCount = 1;
        string productName = "productName1";
        decimal productPrice = 11;

        Product_BuySellOrderEligibilityDto productDto = new Product_BuySellOrderEligibilityDto()
        {
            CreatorId = productCreatorId,
            Name = productName,
            Status = ProductStatus.approved,
            ActiveSellOrdersCount = productActiveSellOrdersCount,
            Price = productPrice
        };

        _productRepositoryMock
            .Setup(orr => orr.GetProductForBuySellOrderAsync(It.IsAny<Guid>()))
            .ReturnsAsync(productDto);

        User_BuySellOrderEligibilityDto userDto = new User_BuySellOrderEligibilityDto()
        {
            UserId = buyerId,
            Balance = productPrice
        };

        _userRepositoryMock
            .Setup(orr => orr.GetUserForBuySellOrderAsync(It.IsAny<string>()))
            .ReturnsAsync(userDto);

        _userRepositoryMock
           .Setup(pr => pr.DidUserBoughtProductAsync(It.IsAny<Guid>(), It.IsAny<string>()))
           .ReturnsAsync(false);

        return (productDto, userDto);
    }


    [Test]
    public async Task CanUserBuySellOrderOfSpecificProductAsync_MustReturnErrorCodeProductNotFound_WhenTheProductDoesNotExist()
    {
        //Arrange
        Guid productId = Guid.Parse("8118e3a6-63b4-474e-a62d-5cf549af3568");

        (Product_BuySellOrderEligibilityDto productDto, User_BuySellOrderEligibilityDto userDto) = SetupValidScenario_ForCanUserBuySellOrderOfSpecificProductAsync();

        _productRepositoryMock
            .Setup(pr => pr.GetProductForBuySellOrderAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Product_BuySellOrderEligibilityDto)null);

        OrderService orderService = new OrderService(sellOrderRepository: _sellOrderRepositoryMock.Object, productRepository: _productRepositoryMock.Object, userRepository: _userRepositoryMock.Object);


        //Act
        Result result = await orderService.CanUserBuySellOrderOfSpecificProductAsync(productId: productId, userId: userDto.UserId);


        //Assert
        Assert.That(result.ErrorCode, Is.EqualTo(ProductErrorCodes.ProductNotFound));
    }

    [Test]
    public async Task CanUserBuySellOrderOfSpecificProductAsync_MustReturnErrorCodeUserNotFound_WhenTheUserIdDoesNotExist()
    {
        //Arrange
        Guid productId = Guid.Parse("8118e3a6-63b4-474e-a62d-5cf549af3568");

        (Product_BuySellOrderEligibilityDto productDto, User_BuySellOrderEligibilityDto userDto) = SetupValidScenario_ForCanUserBuySellOrderOfSpecificProductAsync();

        _userRepositoryMock
            .Setup(pr => pr.GetUserForBuySellOrderAsync(It.IsAny<string>()))
            .ReturnsAsync((User_BuySellOrderEligibilityDto)null);

        OrderService orderService = new OrderService(sellOrderRepository: _sellOrderRepositoryMock.Object, productRepository: _productRepositoryMock.Object, userRepository: _userRepositoryMock.Object);


        //Act
        Result result = await orderService.CanUserBuySellOrderOfSpecificProductAsync(productId: productId, userId: userDto.UserId);


        //Assert
        Assert.That(result.ErrorCode, Is.EqualTo(UserErrorCodes.UserNotFound));
    }

    [Test]
    public async Task CanUserBuySellOrderOfSpecificProductAsync_MustReturnErrorCodeProductInvalidCreator_WhenTheProductWasCreatedByAnotherUser()
    {
        //Arrange
        Guid productId = Guid.Parse("8118e3a6-63b4-474e-a62d-5cf549af3568");

        (Product_BuySellOrderEligibilityDto productDto, User_BuySellOrderEligibilityDto userDto) = SetupValidScenario_ForCanUserBuySellOrderOfSpecificProductAsync();
        userDto.UserId = productDto.CreatorId;

        _userRepositoryMock
            .Setup(pr => pr.GetUserForBuySellOrderAsync(It.IsAny<string>()))
            .ReturnsAsync(userDto);

        OrderService orderService = new OrderService(sellOrderRepository: _sellOrderRepositoryMock.Object, productRepository: _productRepositoryMock.Object, userRepository: _userRepositoryMock.Object);


        //Act
        Result result = await orderService.CanUserBuySellOrderOfSpecificProductAsync(productId: productId, userId: userDto.UserId);


        //Assert
        Assert.That(result.ErrorCode, Is.EqualTo(ProductErrorCodes.ProductInvalidCreator));

    }

    [Test]
    public async Task CanUserBuySellOrderOfSpecificProductAsync_MustReturnErrorCodeProductInvalidStatus_WhenTheProductIsNotApproved()
    {
        //Arrange
        Guid productId = Guid.Parse("8118e3a6-63b4-474e-a62d-5cf549af3568");

        (Product_BuySellOrderEligibilityDto productDto, User_BuySellOrderEligibilityDto userDto) = SetupValidScenario_ForCanUserBuySellOrderOfSpecificProductAsync();
        productDto.Status = ProductStatus.inspection;

        _productRepositoryMock
            .Setup(pr => pr.GetProductForBuySellOrderAsync(It.IsAny<Guid>()))
            .ReturnsAsync(productDto);

        OrderService orderService = new OrderService(sellOrderRepository: _sellOrderRepositoryMock.Object, productRepository: _productRepositoryMock.Object, userRepository: _userRepositoryMock.Object);


        //Act
        Result result = await orderService.CanUserBuySellOrderOfSpecificProductAsync(productId: productId, userId: userDto.UserId);


        //Assert
        Assert.That(result.ErrorCode, Is.EqualTo(ProductErrorCodes.ProductInvalidStatus));

    }

    [Test]
    public async Task CanUserBuySellOrderOfSpecificProductAsync_MustReturnErrorCodeProductHasNoActiveSaleOrders_WhenTheProductHasNoActiveSellOrders()
    {
        //Arrange
        Guid productId = Guid.Parse("8118e3a6-63b4-474e-a62d-5cf549af3568");

        (Product_BuySellOrderEligibilityDto productDto, User_BuySellOrderEligibilityDto userDto) = SetupValidScenario_ForCanUserBuySellOrderOfSpecificProductAsync();
        productDto.ActiveSellOrdersCount = 0;

        _productRepositoryMock
            .Setup(pr => pr.GetProductForBuySellOrderAsync(It.IsAny<Guid>()))
            .ReturnsAsync(productDto);

        OrderService orderService = new OrderService(sellOrderRepository: _sellOrderRepositoryMock.Object, productRepository: _productRepositoryMock.Object, userRepository: _userRepositoryMock.Object);


        //Act
        Result result = await orderService.CanUserBuySellOrderOfSpecificProductAsync(productId: productId, userId: userDto.UserId);


        //Assert
        Assert.That(result.ErrorCode, Is.EqualTo(ProductErrorCodes.ProductHasNoActiveSaleOrders));

    }

    [Test]
    public async Task CanUserBuySellOrderOfSpecificProductAsync_MustReturnErrorCodeUserInsufficientBalance_WhenTheUserBalanceIsBelowTheProductPrice()
    {
        //Arrange
        Guid productId = Guid.Parse("8118e3a6-63b4-474e-a62d-5cf549af3568");

        (Product_BuySellOrderEligibilityDto productDto, User_BuySellOrderEligibilityDto userDto) = SetupValidScenario_ForCanUserBuySellOrderOfSpecificProductAsync();
        userDto.Balance = productDto.Price - 0.01M;

        _userRepositoryMock
            .Setup(pr => pr.GetUserForBuySellOrderAsync(It.IsAny<string>()))
            .ReturnsAsync(userDto);

        OrderService orderService = new OrderService(sellOrderRepository: _sellOrderRepositoryMock.Object, productRepository: _productRepositoryMock.Object, userRepository: _userRepositoryMock.Object);


        //Act
        Result result = await orderService.CanUserBuySellOrderOfSpecificProductAsync(productId: productId, userId: userDto.UserId);


        //Assert
        Assert.That(result.ErrorCode, Is.EqualTo(UserErrorCodes.UserInsufficientBalance));

    }

    [Test]
    public async Task CanUserBuySellOrderOfSpecificProductAsync_MustReturnErrorProductAlreadyPurchased_WhenTheProductWasAlreadyPurchasedByTheUser()
    {
        //Arrange
        Guid productId = Guid.Parse("8118e3a6-63b4-474e-a62d-5cf549af3568");

        (Product_BuySellOrderEligibilityDto productDto, User_BuySellOrderEligibilityDto userDto) = SetupValidScenario_ForCanUserBuySellOrderOfSpecificProductAsync();

        _userRepositoryMock
            .Setup(pr => pr.DidUserBoughtProductAsync(It.IsAny<Guid>(), It.IsAny<string>()))
            .ReturnsAsync(true);

        OrderService orderService = new OrderService(sellOrderRepository: _sellOrderRepositoryMock.Object, productRepository: _productRepositoryMock.Object, userRepository: _userRepositoryMock.Object);


        //Act
        Result result = await orderService.CanUserBuySellOrderOfSpecificProductAsync(productId: productId, userId: userDto.UserId);


        //Assert
        Assert.That(result.ErrorCode, Is.EqualTo(ProductErrorCodes.ProductAlreadyPurchased));

    }

    [Test]
    public async Task CanUserBuySellOrderOfSpecificProductAsync_MustBuySellOrder_WhenAllChecksPass()
    {
        //Arrange
        Guid productId = Guid.Parse("8118e3a6-63b4-474e-a62d-5cf549af3568");

        (Product_BuySellOrderEligibilityDto productDto, User_BuySellOrderEligibilityDto userDto) = SetupValidScenario_ForCanUserBuySellOrderOfSpecificProductAsync();

        OrderService orderService = new OrderService(sellOrderRepository: _sellOrderRepositoryMock.Object, productRepository: _productRepositoryMock.Object, userRepository: _userRepositoryMock.Object);


        //Act
        Result result = await orderService.CanUserBuySellOrderOfSpecificProductAsync(productId: productId, userId: userDto.UserId);


        //Assert
        Assert.That(result.ErrorCode, Is.EqualTo(string.Empty));

    }
    // CanUserCancelSellOrdersOfSpecificProductAsync tests>
}
