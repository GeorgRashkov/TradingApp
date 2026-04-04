using Moq;
using System.Threading.Tasks;
using TradingApp.Data.Models;
using TradingApp.Data.Repository;
using TradingApp.Data.Repository.Interfaces;
using TradingApp.GCommon;
using TradingApp.GCommon.Enums;
using TradingApp.GCommon.ErrorCodes;
using TradingApp.Services.Core;
using static TradingApp.GCommon.EntityValidation;

namespace TradingApp.Services.Tests;

public class OrderRequestOperationsServiceTests
{
    private Mock<IOrderRequestRepository> _orderRequestRepositoryMock;
    private Mock<IUserRepository> _userRepositoryMock;
    private Mock<IProductRepository> _productRepositoryMock;
    [SetUp]
    public void Setup()
    {
        _orderRequestRepositoryMock = new Mock<IOrderRequestRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _productRepositoryMock = new Mock<IProductRepository>();
    }

    [Test]
    public void Test1()
    {
        Assert.Pass();
    }

    //< CreateSuggestionForOrderRequest tests


    //A product can be suggested only to those order requests which meet the criteria in this method
    private void SetupValidScenario_ForCreateSuggestionForOrderRequest()
    {
        _orderRequestRepositoryMock
            .Setup(orr => orr.DoesOrderRequestExistAsync(It.IsAny<Guid>()))
            .ReturnsAsync(true);

        _userRepositoryMock
            .Setup(ur => ur.GetCreatorIdOfRequestAsync(It.IsAny<Guid>()))
            .ReturnsAsync("different-user");

        _orderRequestRepositoryMock
            .Setup(orr => orr.IsOrderRequestActiveAsync(It.IsAny<Guid>()))
            .ReturnsAsync(true);

        _productRepositoryMock
            .Setup(pr => pr.DoesProductExistAsync(It.IsAny<Guid>()))
            .ReturnsAsync(true);

        _productRepositoryMock
            .Setup(pr => pr.DoesProductCreatedByUserExistAsync(It.IsAny<string>(), It.IsAny<Guid>()))
            .ReturnsAsync(true);

        _productRepositoryMock
            .Setup(pr => pr.IsProductApprovedAsync(It.IsAny<Guid>()))
            .ReturnsAsync(true);

        _productRepositoryMock
            .Setup(pr => pr.DoesProductHaveActiveSaleOrdersAsync(It.IsAny<Guid>()))
            .ReturnsAsync(true);

        _productRepositoryMock
            .Setup(pr => pr.IsProductSuggestedToOrderRequestAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(false);
    }

    //<request validations
    [Test]
    public void CreateSuggestionForOrderRequest_MustReturnErrorCodeRequestNotFound_WhenOrderRequestExistDoesNotExist()
    {
        //Arrange
        Guid requestId = Guid.Parse("8118e3a6-63b4-474e-a62d-5cf549af3568");
        Guid productId = Guid.Parse("4e0545d5-d160-4d9b-8efe-6d0606d18335");
        string suggesterId = "user-id";

        SetupValidScenario_ForCreateSuggestionForOrderRequest();
        _orderRequestRepositoryMock
            .Setup(orr => orr.DoesOrderRequestExistAsync(requestId))
            .ReturnsAsync(false);

        OrderRequestOperationsService service = new OrderRequestOperationsService(
            productRepository: _productRepositoryMock.Object,
            orderRequestRepository: _orderRequestRepositoryMock.Object,
            userRepository: _userRepositoryMock.Object);


        //Act
        Result result = service.CreateSuggestionForOrderRequest(
            productId: productId,
            suggesterId: suggesterId,
            requestId: requestId).Result;


        //Assert
        Assert.That(result.Success, Is.EqualTo(false));
        Assert.That(result.ErrorCode, Is.EqualTo(OrderRequestErrorCodes.RequestNotFound));
    }
    [Test]
    public void CreateSuggestionForOrderRequest_MustReturnErrorCodeRequestSuggestionSameCreator_WhenOrderRequestIsCreatedBySuggester()
    {
        //Arrange
        Guid requestId = Guid.Parse("8118e3a6-63b4-474e-a62d-5cf549af3568");
        Guid productId = Guid.Parse("4e0545d5-d160-4d9b-8efe-6d0606d18335");
        string suggesterId = "user-id";

        SetupValidScenario_ForCreateSuggestionForOrderRequest();
        _userRepositoryMock
            .Setup(ur => ur.GetCreatorIdOfRequestAsync(requestId))
            .ReturnsAsync(suggesterId);

        OrderRequestOperationsService service = new OrderRequestOperationsService(
            productRepository: _productRepositoryMock.Object,
            orderRequestRepository: _orderRequestRepositoryMock.Object,
            userRepository: _userRepositoryMock.Object);


        //Act
        Result result = service.CreateSuggestionForOrderRequest(
            productId: productId,
            suggesterId: suggesterId,
            requestId: requestId).Result;


        //Assert
        Assert.That(result.Success, Is.EqualTo(false));
        Assert.That(result.ErrorCode, Is.EqualTo(OrderRequestErrorCodes.RequestSuggestionSameCreator));
    }
    [Test]
    public void CreateSuggestionForOrderRequest_MustReturnErrorCodeRequestInvalidStatus_WhenOrderRequestHasNonActiveStatus()
    {
        //Arrange
        Guid requestId = Guid.Parse("8118e3a6-63b4-474e-a62d-5cf549af3568");
        Guid productId = Guid.Parse("4e0545d5-d160-4d9b-8efe-6d0606d18335");
        string suggesterId = "user-id";

        SetupValidScenario_ForCreateSuggestionForOrderRequest();
        _orderRequestRepositoryMock
            .Setup(orr => orr.IsOrderRequestActiveAsync(requestId))
            .ReturnsAsync(false);

        OrderRequestOperationsService service = new OrderRequestOperationsService(
            productRepository: _productRepositoryMock.Object,
            orderRequestRepository: _orderRequestRepositoryMock.Object,
            userRepository: _userRepositoryMock.Object);


        //Act
        Result result = service.CreateSuggestionForOrderRequest(
            productId: productId,
            suggesterId: suggesterId,
            requestId: requestId).Result;


        //Assert
        Assert.That(result.Success, Is.EqualTo(false));
        Assert.That(result.ErrorCode, Is.EqualTo(OrderRequestErrorCodes.RequestInvalidStatus));
    }
    //request validations>


    //<product validations
    [Test]
    public void CreateSuggestionForOrderRequest_MustReturnErrorCodeProductNotFound_WhenNonExistingProductIsSuggested()
    {
        //Arrange
        Guid requestId = Guid.Parse("8118e3a6-63b4-474e-a62d-5cf549af3568");
        Guid productId = Guid.Parse("4e0545d5-d160-4d9b-8efe-6d0606d18335");
        string suggesterId = "user-id";

        SetupValidScenario_ForCreateSuggestionForOrderRequest();
        _productRepositoryMock
            .Setup(pr => pr.DoesProductExistAsync(productId))
            .ReturnsAsync(false);

        OrderRequestOperationsService service = new OrderRequestOperationsService(
            productRepository: _productRepositoryMock.Object,
            orderRequestRepository: _orderRequestRepositoryMock.Object,
            userRepository: _userRepositoryMock.Object);


        //Act
        Result result = service.CreateSuggestionForOrderRequest(
            productId: productId,
            suggesterId: suggesterId,
            requestId: requestId).Result;


        //Assert
        Assert.That(result.Success, Is.EqualTo(false));
        Assert.That(result.ErrorCode, Is.EqualTo(ProductErrorCodes.ProductNotFound));
    }

    [Test]
    public void CreateSuggestionForOrderRequest_MustReturnErrorCodeProductInvalidCreator_WhenSuggestedProductIsNotCreatedBySuggester()
    {
        //Arrange
        Guid requestId = Guid.Parse("8118e3a6-63b4-474e-a62d-5cf549af3568");
        Guid productId = Guid.Parse("4e0545d5-d160-4d9b-8efe-6d0606d18335");
        string suggesterId = "user-id";

        SetupValidScenario_ForCreateSuggestionForOrderRequest();
        _productRepositoryMock
            .Setup(pr => pr.DoesProductCreatedByUserExistAsync(suggesterId, productId))
            .ReturnsAsync(false);

        OrderRequestOperationsService service = new OrderRequestOperationsService(
            productRepository: _productRepositoryMock.Object,
            orderRequestRepository: _orderRequestRepositoryMock.Object,
            userRepository: _userRepositoryMock.Object);


        //Act
        Result result = service.CreateSuggestionForOrderRequest(
            productId: productId,
            suggesterId: suggesterId,
            requestId: requestId).Result;


        //Assert
        Assert.That(result.Success, Is.EqualTo(false));
        Assert.That(result.ErrorCode, Is.EqualTo(ProductErrorCodes.ProductInvalidCreator));
    }

    [Test]
    public void CreateSuggestionForOrderRequest_MustReturnErrorCodeProductInvalidStatus_WhenSuggestedProductIsNotApproved()
    {
        //Arrange
        Guid requestId = Guid.Parse("8118e3a6-63b4-474e-a62d-5cf549af3568");
        Guid productId = Guid.Parse("4e0545d5-d160-4d9b-8efe-6d0606d18335");
        string suggesterId = "user-id";

        SetupValidScenario_ForCreateSuggestionForOrderRequest();
        _productRepositoryMock
            .Setup(pr => pr.IsProductApprovedAsync(productId))
            .ReturnsAsync(false);

        OrderRequestOperationsService service = new OrderRequestOperationsService(
            productRepository: _productRepositoryMock.Object,
            orderRequestRepository: _orderRequestRepositoryMock.Object,
            userRepository: _userRepositoryMock.Object);


        //Act
        Result result = service.CreateSuggestionForOrderRequest(
            productId: productId,
            suggesterId: suggesterId,
            requestId: requestId).Result;


        //Assert
        Assert.That(result.Success, Is.EqualTo(false));
        Assert.That(result.ErrorCode, Is.EqualTo(ProductErrorCodes.ProductInvalidStatus));
    }
    [Test]
    public void CreateSuggestionForOrderRequest_MustReturnErrorCodeProductHasNoActiveSaleOrders_WhenSuggestedProductHasNoActiveSaleOrders()
    {
        //Arrange
        Guid requestId = Guid.Parse("8118e3a6-63b4-474e-a62d-5cf549af3568");
        Guid productId = Guid.Parse("4e0545d5-d160-4d9b-8efe-6d0606d18335");
        string suggesterId = "user-id";

        SetupValidScenario_ForCreateSuggestionForOrderRequest();
        _productRepositoryMock
            .Setup(pr => pr.DoesProductHaveActiveSaleOrdersAsync(productId))
            .ReturnsAsync(false);

        OrderRequestOperationsService service = new OrderRequestOperationsService(
            productRepository: _productRepositoryMock.Object,
            orderRequestRepository: _orderRequestRepositoryMock.Object,
            userRepository: _userRepositoryMock.Object);


        //Act
        Result result = service.CreateSuggestionForOrderRequest(
            productId: productId,
            suggesterId: suggesterId,
            requestId: requestId).Result;


        //Assert
        Assert.That(result.Success, Is.EqualTo(false));
        Assert.That(result.ErrorCode, Is.EqualTo(ProductErrorCodes.ProductHasNoActiveSaleOrders));
    }

    [Test]
    public void CreateSuggestionForOrderRequest_MustReturnErrorCodeProductAlreadySuggestedToRequest_WhenSuggestedProductIsAlreadySuggested()
    {
        //Arrange
        Guid requestId = Guid.Parse("8118e3a6-63b4-474e-a62d-5cf549af3568");
        Guid productId = Guid.Parse("4e0545d5-d160-4d9b-8efe-6d0606d18335");
        string suggesterId = "user-id";

        SetupValidScenario_ForCreateSuggestionForOrderRequest();
        _productRepositoryMock
            .Setup(pr => pr.IsProductSuggestedToOrderRequestAsync(productId, requestId))
            .ReturnsAsync(true);

        OrderRequestOperationsService service = new OrderRequestOperationsService(
            productRepository: _productRepositoryMock.Object,
            orderRequestRepository: _orderRequestRepositoryMock.Object,
            userRepository: _userRepositoryMock.Object);


        //Act
        Result result = service.CreateSuggestionForOrderRequest(
            productId: productId,
            suggesterId: suggesterId,
            requestId: requestId).Result;


        //Assert
        Assert.That(result.Success, Is.EqualTo(false));
        Assert.That(result.ErrorCode, Is.EqualTo(ProductErrorCodes.ProductAlreadySuggestedToRequest));
    }
    //product validations>


    [Test]
    public void CreateSuggestionForOrderRequest_MustCreateProductSuggestion_WhenAllChecksPass()
    {
        //Arrange
        Guid requestId = Guid.Parse("8118e3a6-63b4-474e-a62d-5cf549af3568");
        Guid productId = Guid.Parse("4e0545d5-d160-4d9b-8efe-6d0606d18335");
        string suggesterId = "user-id";
        SellOrderSuggestion sellOrderSuggestion = new SellOrderSuggestion
        {
            ProductId = productId,
            OrderRequestId = requestId
        };

        SetupValidScenario_ForCreateSuggestionForOrderRequest();


        OrderRequestOperationsService service = new OrderRequestOperationsService(
            productRepository: _productRepositoryMock.Object,
            orderRequestRepository: _orderRequestRepositoryMock.Object,
            userRepository: _userRepositoryMock.Object);


        //Act
        Result result = service.CreateSuggestionForOrderRequest(
            productId: productId,
            suggesterId: suggesterId,
            requestId: requestId).Result;


        //Assert
        Assert.That(result.Success, Is.EqualTo(true));
        Assert.That(result.ErrorCode, Is.EqualTo(string.Empty));
        _orderRequestRepositoryMock.Verify(or => or.CreateSellOrderSuggestionAsync(
            It.Is<SellOrderSuggestion>(sos => sos.ProductId == productId && sos.OrderRequestId == requestId))
        , Times.Once);
    }

    //CreateSuggestionForOrderRequest tests>


    //< CreateOrderRequest tests
    private void SetupValidScenario_ForCreateOrderRequest()
    {
        _userRepositoryMock
            .Setup(ur => ur.DoesUserExistAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        _orderRequestRepositoryMock
            .Setup(orr => orr.DoesOrderRequestCreatedByUserExistAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(false);
    }

    [Test]
    public async Task CreateOrderRequest_MustReturnErrorCodeUserNotFound_WhenUserDoesNotExist()
    {
        //Arrange
        string title = "title";
        string description = "description";
        decimal maxPrice = 100;
        string requestCreatorId = "user-id";

        SetupValidScenario_ForCreateOrderRequest();
        _userRepositoryMock
            .Setup(ur => ur.DoesUserExistAsync(requestCreatorId))
            .ReturnsAsync(false);

        OrderRequestOperationsService service = new OrderRequestOperationsService(
            productRepository: _productRepositoryMock.Object,
            orderRequestRepository: _orderRequestRepositoryMock.Object,
            userRepository: _userRepositoryMock.Object);


        //Act
        Result result = await service.CreateOrderRequest(title: title, description: description, maxPrice: maxPrice, creatorId: requestCreatorId);


        //Assert
        Assert.That(result.Success, Is.EqualTo(false));
        Assert.That(result.ErrorCode, Is.EqualTo(UserErrorCodes.UserNotFound));
    }


    [Test]
    public async Task CreateOrderRequest_MustReturnErrorCodeRequestWithSameTitleAlreadyExists_WhenUserHasOtherRequestWithSameTitle()
    {
        //Arrange
        string title = "title";
        string description = "description";
        decimal maxPrice = 100;
        string requestCreatorId = "user-id";

        SetupValidScenario_ForCreateOrderRequest();
        _orderRequestRepositoryMock
            .Setup(ur => ur.DoesOrderRequestCreatedByUserExistAsync(requestCreatorId, title))
            .ReturnsAsync(true);

        OrderRequestOperationsService service = new OrderRequestOperationsService(
            productRepository: _productRepositoryMock.Object,
            orderRequestRepository: _orderRequestRepositoryMock.Object,
            userRepository: _userRepositoryMock.Object);


        //Act
        Result result = await service.CreateOrderRequest(title: title, description: description, maxPrice: maxPrice, creatorId: requestCreatorId);


        //Assert
        Assert.That(result.Success, Is.EqualTo(false));
        Assert.That(result.ErrorCode, Is.EqualTo(OrderRequestErrorCodes.RequestWithSameTitleAlreadyExists));
    }


    [Test]
    public async Task CreateOrderRequest_MustCreateRequest_WhenAllChecksPass()
    {
        //Arrange
        string title = "title";
        string description = "description";
        decimal maxPrice = 100;
        string requestCreatorId = "user-id";
        OrderRequestStatus status = ApplicationConstants.CreatedOrderRequestDefaultStatus;

        OrderRequest orderRequest = new OrderRequest
        {
            Title = title,
            Description = description,
            MaxPrice = maxPrice,
            Status = status,
            CreatorId = requestCreatorId
        };

        SetupValidScenario_ForCreateOrderRequest();

        OrderRequestOperationsService service = new OrderRequestOperationsService(
            productRepository: _productRepositoryMock.Object,
            orderRequestRepository: _orderRequestRepositoryMock.Object,
            userRepository: _userRepositoryMock.Object);


        //Act
        Result result = await service.CreateOrderRequest(title: title, description: description, maxPrice: maxPrice, creatorId: requestCreatorId);


        //Assert
        Assert.That(result.Success, Is.EqualTo(true));
        Assert.That(result.ErrorCode, Is.EqualTo(string.Empty));

        _orderRequestRepositoryMock
            .Verify(orr => orr.CreateOrderRequestAsync(
                It.Is<OrderRequest>(or =>
                or.Title == title && or.Description == description &&
                or.MaxPrice == maxPrice &&
                or.Status == status && or.CreatorId == requestCreatorId)
                ), Times.Once);
    }

    //CreateOrderRequest tests>




    //< UpdateOrderRequest tests

    private void SetupValidScenario_ForUpdateOrderRequest(string userId, Guid orderRequestId)
    {
        _userRepositoryMock
            .Setup(ur => ur.GetCreatorIdOfRequestAsync(It.IsAny<Guid>()))
            .ReturnsAsync(userId);

        _orderRequestRepositoryMock
            .Setup(orr => orr.DoesOrderRequestCreatedByUserExistAsync(It.IsAny<string>(), It.IsAny<string>(), new Guid[1] { orderRequestId }))
            .ReturnsAsync(false);
    }

    [Test]
    public async Task UpdateOrderRequest_MustReturnErrorCodeRequestNotFound_WhenTheUserTriesToUpdateNonExistingRequest()
    {
        //Arrange        
        Guid orderRequestId = Guid.Parse("8118e3a6-63b4-474e-a62d-5cf549af3568");
        string title = "title";
        string description = "description";
        decimal maxPrice = 100;
        string requestUpdaterId = "user-id";

        SetupValidScenario_ForUpdateOrderRequest(userId: requestUpdaterId, orderRequestId: orderRequestId);
        _userRepositoryMock
            .Setup(ur => ur.GetCreatorIdOfRequestAsync(orderRequestId))
            .ReturnsAsync((string)null);

        OrderRequestOperationsService service = new OrderRequestOperationsService(
            productRepository: _productRepositoryMock.Object,
            orderRequestRepository: _orderRequestRepositoryMock.Object,
            userRepository: _userRepositoryMock.Object);


        //Act
        Result result = await service.UpdateOrderRequest(id: orderRequestId, title: title, description: description, maxPrice: maxPrice, creatorId: requestUpdaterId);


        //Assert
        Assert.That(result.Success, Is.EqualTo(false));
        Assert.That(result.ErrorCode, Is.EqualTo(OrderRequestErrorCodes.RequestNotFound));
    }

    [Test]
    public async Task UpdateOrderRequest_MustReturnErrorCodeRequestInvalidCreator_WhenTheUserTriesToUpdateRequestCreatedByOtherUser()
    {
        //Arrange        
        Guid orderRequestId = Guid.Parse("8118e3a6-63b4-474e-a62d-5cf549af3568");
        string title = "title";
        string description = "description";
        decimal maxPrice = 100;
        string requestUpdaterId = "user-id";
        string requestCreatorId = "creator-user-id";

        SetupValidScenario_ForUpdateOrderRequest(userId: requestUpdaterId, orderRequestId: orderRequestId);
        _userRepositoryMock
            .Setup(ur => ur.GetCreatorIdOfRequestAsync(orderRequestId))
            .ReturnsAsync(requestCreatorId);

        OrderRequestOperationsService service = new OrderRequestOperationsService(
            productRepository: _productRepositoryMock.Object,
            orderRequestRepository: _orderRequestRepositoryMock.Object,
            userRepository: _userRepositoryMock.Object);


        //Act
        Result result = await service.UpdateOrderRequest(id: orderRequestId, title: title, description: description, maxPrice: maxPrice, creatorId: requestUpdaterId);


        //Assert
        Assert.That(result.Success, Is.EqualTo(false));
        Assert.That(result.ErrorCode, Is.EqualTo(OrderRequestErrorCodes.RequestInvalidCreator));
    }

    [Test]
    public async Task UpdateOrderRequest_MustReturnErrorCodeRequestWithSameTitleAlreadyExists_WhenUserHasOtherRequestWithSameTitle()
    {
        //Arrange        
        Guid orderRequestId = Guid.Parse("8118e3a6-63b4-474e-a62d-5cf549af3568");
        string title = "title";
        string description = "description";
        decimal maxPrice = 100;
        string requestUpdaterId = "user-id";

        SetupValidScenario_ForUpdateOrderRequest(userId: requestUpdaterId, orderRequestId: orderRequestId);
        _orderRequestRepositoryMock
            .Setup(ur => ur.DoesOrderRequestCreatedByUserExistAsync(requestUpdaterId, title, new Guid[1] { orderRequestId }))
            .ReturnsAsync(true);

        OrderRequestOperationsService service = new OrderRequestOperationsService(
            productRepository: _productRepositoryMock.Object,
            orderRequestRepository: _orderRequestRepositoryMock.Object,
            userRepository: _userRepositoryMock.Object);


        //Act
        Result result = await service.UpdateOrderRequest(id: orderRequestId, title: title, description: description, maxPrice: maxPrice, creatorId: requestUpdaterId);


        //Assert
        Assert.That(result.Success, Is.EqualTo(false));
        Assert.That(result.ErrorCode, Is.EqualTo(OrderRequestErrorCodes.RequestWithSameTitleAlreadyExists));
    }

    [Test]
    public async Task UpdateOrderRequest_MustUpdateOrderRequest_WhenAllChecksPass()
    {
        //Arrange        
        Guid orderRequestId = Guid.Parse("8118e3a6-63b4-474e-a62d-5cf549af3568");
        string title = "title";
        string description = "description";
        decimal maxPrice = 100;
        string requestUpdaterId = "user-id";

        OrderRequest orderRequest = new OrderRequest
        {
            Id = orderRequestId,
            Title = "old-title",
            Description = "old-description",
            MaxPrice = 50,
            CreatorId = requestUpdaterId
        };

        SetupValidScenario_ForUpdateOrderRequest(userId: requestUpdaterId, orderRequestId: orderRequestId);
        _orderRequestRepositoryMock
            .Setup(or => or.GetRequestAsync(orderRequestId))
            .ReturnsAsync(orderRequest);

        OrderRequestOperationsService service = new OrderRequestOperationsService(
            productRepository: _productRepositoryMock.Object,
            orderRequestRepository: _orderRequestRepositoryMock.Object,
            userRepository: _userRepositoryMock.Object);


        //Act
        Result result = await service.UpdateOrderRequest(id: orderRequestId, title: title, description: description, maxPrice: maxPrice, creatorId: requestUpdaterId);


        //Assert
        Assert.That(result.Success, Is.EqualTo(true));
        Assert.That(result.ErrorCode, Is.EqualTo(string.Empty));
        _orderRequestRepositoryMock
            .Verify(orr => orr.UpdateOrderRequest(orderRequest, title, description, maxPrice), Times.Once);
    }
    //UpdateOrderRequest tests>


    //< CancelOrderRequestAsync tests

    [Test]
    public async Task CancelOrderRequestAsync_MustReturnErrorCodeRequestNotFound_WhenTheUserTriesToCancelNonExistingRequest()
    {
        //Arrange        
        Guid orderRequestId = Guid.Parse("8118e3a6-63b4-474e-a62d-5cf549af3568");        
        string requestUpdaterId = "user-id";

        _orderRequestRepositoryMock
            .Setup(ur => ur.GetRequestAsync(orderRequestId))
            .ReturnsAsync((OrderRequest)null);

        OrderRequestOperationsService service = new OrderRequestOperationsService(
            productRepository: _productRepositoryMock.Object,
            orderRequestRepository: _orderRequestRepositoryMock.Object,
            userRepository: _userRepositoryMock.Object);


        //Act
        Result result = await service.CancelOrderRequestAsync(id: orderRequestId, userId: requestUpdaterId);


        //Assert
        Assert.That(result.Success, Is.EqualTo(false));
        Assert.That(result.ErrorCode, Is.EqualTo(OrderRequestErrorCodes.RequestNotFound));
    }

    [Test]
    public async Task CancelOrderRequestAsync_MustReturnErrorCodeRequestInvalidCreator_WhenTheUserTriesToCancelRequestCreatedByOtherUser()
    {
        //Arrange        
        Guid orderRequestId = Guid.Parse("8118e3a6-63b4-474e-a62d-5cf549af3568");
        string requestUpdaterId = "user-id";
        string requestCreatorId = "creator-user-id";

        OrderRequest orderRequest = new OrderRequest
        {
            Id = orderRequestId,
            CreatorId = requestCreatorId,
            Status = OrderRequestStatus.active
        };

        _orderRequestRepositoryMock
            .Setup(ur => ur.GetRequestAsync(orderRequestId))
            .ReturnsAsync(orderRequest);

        OrderRequestOperationsService service = new OrderRequestOperationsService(
            productRepository: _productRepositoryMock.Object,
            orderRequestRepository: _orderRequestRepositoryMock.Object,
            userRepository: _userRepositoryMock.Object);


        //Act
        Result result = await service.CancelOrderRequestAsync(id: orderRequestId, userId: requestUpdaterId);


        //Assert
        Assert.That(result.Success, Is.EqualTo(false));
        Assert.That(result.ErrorCode, Is.EqualTo(OrderRequestErrorCodes.RequestInvalidCreator));
    }

    [Test]
    public async Task CancelOrderRequestAsync_MustReturnErrorCodeRequestInvalidStatus_WhenTheUserTriesToCancelNonActiveRequest()
    {
        //Arrange        
        Guid orderRequestId = Guid.Parse("8118e3a6-63b4-474e-a62d-5cf549af3568");
        string requestUpdaterId = "user-id";        

        OrderRequest orderRequest = new OrderRequest
        {
            Id = orderRequestId,
            CreatorId = requestUpdaterId,
            Status = OrderRequestStatus.completed
        };

        _orderRequestRepositoryMock
            .Setup(ur => ur.GetRequestAsync(orderRequestId))
            .ReturnsAsync(orderRequest);

        OrderRequestOperationsService service = new OrderRequestOperationsService(
            productRepository: _productRepositoryMock.Object,
            orderRequestRepository: _orderRequestRepositoryMock.Object,
            userRepository: _userRepositoryMock.Object);


        //Act
        Result result = await service.CancelOrderRequestAsync(id: orderRequestId, userId: requestUpdaterId);


        //Assert
        Assert.That(result.Success, Is.EqualTo(false));
        Assert.That(result.ErrorCode, Is.EqualTo(OrderRequestErrorCodes.RequestInvalidStatus));
    }

    [Test]
    public async Task CancelOrderRequestAsync_MustCancelOrderRequest_WhenAllChecksPass()
    {
        //Arrange        
        Guid orderRequestId = Guid.Parse("8118e3a6-63b4-474e-a62d-5cf549af3568");
        string requestUpdaterId = "user-id";

        OrderRequest orderRequest = new OrderRequest
        {
            Id = orderRequestId,
            CreatorId = requestUpdaterId,
            Status = OrderRequestStatus.active
        };

        _orderRequestRepositoryMock
            .Setup(ur => ur.GetRequestAsync(orderRequestId))
            .ReturnsAsync(orderRequest);

        OrderRequestOperationsService service = new OrderRequestOperationsService(
            productRepository: _productRepositoryMock.Object,
            orderRequestRepository: _orderRequestRepositoryMock.Object,
            userRepository: _userRepositoryMock.Object);


        //Act
        Result result = await service.CancelOrderRequestAsync(id: orderRequestId, userId: requestUpdaterId);


        //Assert
        Assert.That(result.Success, Is.EqualTo(true));
        Assert.That(result.ErrorCode, Is.EqualTo(string.Empty));
        _orderRequestRepositoryMock
            .Verify(orr => orr.UpdateOrderRequestStatusAsync(orderRequest, OrderRequestStatus.cancelled), Times.Once);
    }

    //CancelOrderRequestAsync tests>
}
