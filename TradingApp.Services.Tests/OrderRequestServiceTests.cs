using Moq;
using TradingApp.Data.Dtos.OrderRequest;
using TradingApp.Data.Models;
using TradingApp.Data.Repository.Interfaces;
using TradingApp.GCommon;
using TradingApp.Services.Core;
using TradingApp.ViewModels.InputOrderRequest;
using TradingApp.ViewModels.OrderRequest;

namespace TradingApp.Services.Tests;

public class OrderRequestServiceTests
{
    private const int _requestsPerPage = ApplicationConstants.RequestsPerPage;
    private Mock<IOrderRequestRepository> _orderRequestRepositoryMock;

    [SetUp]
    public void Setup()
    {
        _orderRequestRepositoryMock = new Mock<IOrderRequestRepository>();
    }


    //<GetActiveRequestsAsync

    [Test]
    public async Task GetActiveRequestsAsync_MustReturnEmtpyCollection_WhenThereAreNoActiveRequests()
    {
        //Arrange
        _orderRequestRepositoryMock
            .Setup(orr => orr.GetActiveRequestsCountAsync())
            .ReturnsAsync(0);

        OrderRequestService orderRequestService = new OrderRequestService(orderRequestRepository: _orderRequestRepositoryMock.Object);


        //Act
        IEnumerable<OrderRequestViewModel> orderRequests = await orderRequestService.GetActiveRequestsAsync(pageIndex: 0);


        //Assert        
        Assert.That(orderRequests, Is.Empty);
    }

    [Test]
    public async Task GetActiveRequestsAsync_MustReturnNonEmtpyCollection_WhenThereAreNoActiveRequests()
    {
        //Arrange
        int pageIndex = 5;
        int activeRequestsCount = pageIndex * _requestsPerPage * 2;

        _orderRequestRepositoryMock
            .Setup(orr => orr.GetActiveRequestsCountAsync())
            .ReturnsAsync(activeRequestsCount);

        List<OrderRequestDto> orderRequestDtos = new List<OrderRequestDto>()
        {
            new OrderRequestDto
        {
            Id = Guid.Parse("8118e3a6-63b4-474e-a62d-5cf549af3568"),
            Title = "Title1",
            MaxPrice = 100
        },
            new OrderRequestDto
        {
            Id = Guid.Parse("4e0545d5-d160-4d9b-8efe-6d0606d18335"),
            Title = "Title2",
            MaxPrice = 200
        },
            new OrderRequestDto
            {
                Id = Guid.Parse("d1c8e3a6-63b4-474e-a62d-5cf549af3568"),
                Title = "Title3",
                MaxPrice = 300
            }
        };

        _orderRequestRepositoryMock
            .Setup(orr => orr.GetActiveRequestsAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync((IEnumerable<OrderRequestDto>)orderRequestDtos);

        OrderRequestService orderRequestService = new OrderRequestService(orderRequestRepository: _orderRequestRepositoryMock.Object);


        //Act
        IEnumerable<OrderRequestViewModel> orderRequests = await orderRequestService.GetActiveRequestsAsync(pageIndex: pageIndex);


        //Assert        
        Assert.That(orderRequests, Is.Not.Empty);

        for (int i = 0; i < orderRequests.Count(); i++)
        {
            Assert.That(orderRequests.ElementAt(i).Id, Is.EqualTo(orderRequestDtos[i].Id));
            Assert.That(orderRequests.ElementAt(i).Title, Is.EqualTo(orderRequestDtos[i].Title));
            Assert.That(orderRequests.ElementAt(i).MaxPrice, Is.EqualTo($"{orderRequestDtos[i].MaxPrice:F2}"));
        }

        Assert.That(orderRequests.Count(), Is.EqualTo(orderRequestDtos.Count));
        Assert.That(orderRequestService.RequestPageIndex, Is.EqualTo(pageIndex));
    }

    //GetActiveRequestsAsync>



    //<GetDetailsForActiveRequestAsync tets

    [Test]
    public async Task GetDetailsForActiveRequestAsync_MustReturnNull_WhenTheRequestDoesNotExist()
    {
        //Arrange
        Guid requestId = Guid.Parse("4e0545d5-d160-4d9b-8efe-6d0606d18335");
        _orderRequestRepositoryMock
            .Setup(orr => orr.GetDetailsForActiveRequestAsync(It.IsAny<Guid>()))
            .ReturnsAsync((OrderRequestDetailsDto)null);

        OrderRequestService orderRequestService = new OrderRequestService(orderRequestRepository: _orderRequestRepositoryMock.Object);
        

        //Act
        OrderRequestDetailsViewModel? orderRequestDetails = await orderRequestService.GetDetailsForActiveRequestAsync(requestId: requestId);


        //Assert        
        Assert.That(orderRequestDetails, Is.Null);
    }

    [Test]
    public async Task GetDetailsForActiveRequestAsync_MustReturnDetailsForActiveRequest_WhenTheRequestExists()
    {
        //Arrange
        Guid requestId = Guid.Parse("4e0545d5-d160-4d9b-8efe-6d0606d18335");
        OrderRequestDetailsDto orderRequestDetailsDto = new OrderRequestDetailsDto()
        {
            Id = requestId,
            Title = "Title1",
            Description = "Description1",
            MaxPrice = 100,            
            CreatorUserName = "User1"           
        };

        _orderRequestRepositoryMock
            .Setup(orr => orr.GetDetailsForActiveRequestAsync(It.IsAny<Guid>()))
            .ReturnsAsync(orderRequestDetailsDto);

        OrderRequestService orderRequestService = new OrderRequestService(orderRequestRepository: _orderRequestRepositoryMock.Object);
        

        //Act
        OrderRequestDetailsViewModel? orderRequestDetails = await orderRequestService.GetDetailsForActiveRequestAsync(requestId: requestId);


        //Assert        
        Assert.That(orderRequestDetails, Is.Not.Null);

        Assert.That(orderRequestDetails.Id, Is.EqualTo(orderRequestDetailsDto.Id));
        Assert.That(orderRequestDetails.Title, Is.EqualTo(orderRequestDetailsDto.Title));
        Assert.That(orderRequestDetails.Description, Is.EqualTo(orderRequestDetailsDto.Description));
        Assert.That(orderRequestDetails.MaxPrice, Is.EqualTo(orderRequestDetailsDto.MaxPrice.ToString("f2")));        
        Assert.That(orderRequestDetails.CreatorName, Is.EqualTo(orderRequestDetailsDto.CreatorUserName));

    }

    //GetDetailsForActiveRequestAsync tests>


    //< GetActiveRequestsCreatedByUserAsync tests

    [Test]
    public async Task GetActiveRequestsCreatedByUserAsync_MustReturnEmtpyCollection_WhenTheUserHasNoActiveRequests()
    {
        //Arrange
        string userId = "User1";

        _orderRequestRepositoryMock
            .Setup(orr => orr.GetUserActiveRequestsCountAsync(It.IsAny<string>()))
            .ReturnsAsync(0);

        OrderRequestService orderRequestService = new OrderRequestService(orderRequestRepository: _orderRequestRepositoryMock.Object);


        //Act
        IEnumerable<MyOrderRequestViewModel> orderRequests = await orderRequestService.GetActiveRequestsCreatedByUserAsync(pageIndex: 0, userId: userId);


        //Assert        
        Assert.That(orderRequests, Is.Empty);
    }

    [Test]
    public async Task GetActiveRequestsCreatedByUserAsync_MustReturnNonEmtpyCollection_WhenTheUserHasActiveRequests()
    {
        //Arrange
        string userId = "User1";
        int pageIndex = 5;
        int activeRequestsCount = pageIndex * _requestsPerPage * 2;

        _orderRequestRepositoryMock
            .Setup(orr => orr.GetUserActiveRequestsCountAsync(It.IsAny<string>()))
            .ReturnsAsync(activeRequestsCount);

        List<OrderRequestDto> orderRequestDtos = new List<OrderRequestDto>()
        {
            new OrderRequestDto
        {
            Id = Guid.Parse("8118e3a6-63b4-474e-a62d-5cf549af3568"),
            Title = "Title1",
            MaxPrice = 100
        },
            new OrderRequestDto
        {
            Id = Guid.Parse("4e0545d5-d160-4d9b-8efe-6d0606d18335"),
            Title = "Title2",
            MaxPrice = 200
        },
            new OrderRequestDto
            {
                Id = Guid.Parse("d1c8e3a6-63b4-474e-a62d-5cf549af3568"),
                Title = "Title3",
                MaxPrice = 300
            }
        };

        _orderRequestRepositoryMock
            .Setup(orr => orr.GetActiveRequestsCreatedByUserAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync((IEnumerable<OrderRequestDto>)orderRequestDtos);

        OrderRequestService orderRequestService = new OrderRequestService(orderRequestRepository: _orderRequestRepositoryMock.Object);


        //Act
        IEnumerable<MyOrderRequestViewModel> orderRequests = await orderRequestService.GetActiveRequestsCreatedByUserAsync(pageIndex: pageIndex, userId: userId);


        //Assert        
        Assert.That(orderRequests, Is.Not.Empty);

        for (int i = 0; i < orderRequests.Count(); i++)
        {
            Assert.That(orderRequests.ElementAt(i).Id, Is.EqualTo(orderRequestDtos[i].Id));
            Assert.That(orderRequests.ElementAt(i).Title, Is.EqualTo(orderRequestDtos[i].Title));
            Assert.That(orderRequests.ElementAt(i).MaxPrice, Is.EqualTo($"{orderRequestDtos[i].MaxPrice:F2}"));
        }

        Assert.That(orderRequests.Count(), Is.EqualTo(orderRequestDtos.Count));
        Assert.That(orderRequestService.RequestPageIndex, Is.EqualTo(pageIndex));
    }

    // GetActiveRequestsCreatedByUserAsync tests>



    //<GetDetailsForActiveRequestCreatedByUserAsync tests

    [Test]
    public async Task GetDetailsForActiveRequestCreatedByUserAsync_MustReturnNull_WhenTheRequestDoesNotExist()
    {
        //Arrange
        string userId = "User1";
        Guid requestId = Guid.Parse("4e0545d5-d160-4d9b-8efe-6d0606d18335");

        _orderRequestRepositoryMock
            .Setup(orr => orr.GetDetailsForActiveRequestCreatedByUserAsync(It.IsAny<Guid>(), It.IsAny<string>()))
            .ReturnsAsync((OrderRequestDetailsDto)null);

        OrderRequestService orderRequestService = new OrderRequestService(orderRequestRepository: _orderRequestRepositoryMock.Object);


        //Act
        MyOrderRequestDetailsViewModel? orderRequestDetails = await orderRequestService.GetDetailsForActiveRequestCreatedByUserAsync(requestId: requestId, userId:userId);


        //Assert        
        Assert.That(orderRequestDetails, Is.Null);
    }

    [Test]
    public async Task GetDetailsForActiveRequestCreatedByUserAsync_MustReturnNonEmtpyCollection_WhenTheRequestExists()
    {
        //Arrange
        string userId = "User1";
        Guid requestId = Guid.Parse("4e0545d5-d160-4d9b-8efe-6d0606d18335");
        OrderRequestDetailsDto orderRequestDetailsDto = new OrderRequestDetailsDto()
        {
            Id = requestId,
            Title = "Title1",
            Description = "Description1",
            MaxPrice = 100,            
            CreatorUserName = "User1",
            HasSuggestions = true
        };

        _orderRequestRepositoryMock
            .Setup(orr => orr.GetDetailsForActiveRequestCreatedByUserAsync(It.IsAny<Guid>(), It.IsAny<string>()))
            .ReturnsAsync(orderRequestDetailsDto);

        OrderRequestService orderRequestService = new OrderRequestService(orderRequestRepository: _orderRequestRepositoryMock.Object);


        //Act
        MyOrderRequestDetailsViewModel? orderRequestDetails = await orderRequestService.GetDetailsForActiveRequestCreatedByUserAsync(requestId: requestId, userId: userId);


        //Assert        
        Assert.That(orderRequestDetails, Is.Not.Null);

        Assert.That(orderRequestDetails.Id, Is.EqualTo(orderRequestDetailsDto.Id));
        Assert.That(orderRequestDetails.Title, Is.EqualTo(orderRequestDetailsDto.Title));
        Assert.That(orderRequestDetails.Description, Is.EqualTo(orderRequestDetailsDto.Description));
        Assert.That(orderRequestDetails.MaxPrice, Is.EqualTo(orderRequestDetailsDto.MaxPrice.ToString("f2")));        
        Assert.That(orderRequestDetails.HasSuggestions, Is.EqualTo(orderRequestDetailsDto.HasSuggestions));
    }

    //GetDetailsForActiveRequestCreatedByUserAsync tests>


    //<GetUpdatedOrderRequestModelAsync tests

    [Test]
    public async Task GetUpdatedOrderRequestModelAsync_MustReturnNull_WhenTheRequestDoesNotExist()
    {
        //Arrange        
        Guid requestId = Guid.Parse("4e0545d5-d160-4d9b-8efe-6d0606d18335");

        _orderRequestRepositoryMock
            .Setup(orr => orr.GetRequestAsync(It.IsAny<Guid>()))
            .ReturnsAsync((OrderRequest)null);

        OrderRequestService orderRequestService = new OrderRequestService(orderRequestRepository: _orderRequestRepositoryMock.Object);


        //Act
        UpdatedOrderRequestModel? updatedOrderRequest = await orderRequestService.GetUpdatedOrderRequestModelAsync(orderRequestId: requestId);


        //Assert        
        Assert.That(updatedOrderRequest, Is.Null);
    }

    [Test]
    public async Task GetUpdatedOrderRequestModelAsync_MustReturnUpdatedOrderRequest_WhenTheRequestExists()
    {
        //Arrange        
        Guid requestId = Guid.Parse("4e0545d5-d160-4d9b-8efe-6d0606d18335");
        OrderRequest orderRequest = new OrderRequest()
        {
            Id = requestId,
            Title = "Title1",
            Description = "Description1",
            MaxPrice = 100            
        };

        _orderRequestRepositoryMock
            .Setup(orr => orr.GetRequestAsync(It.IsAny<Guid>()))
            .ReturnsAsync(orderRequest);

        OrderRequestService orderRequestService = new OrderRequestService(orderRequestRepository: _orderRequestRepositoryMock.Object);


        //Act
        UpdatedOrderRequestModel? updatedOrderRequest = await orderRequestService.GetUpdatedOrderRequestModelAsync(orderRequestId: requestId);


        //Assert        
        Assert.That(updatedOrderRequest, Is.Not.Null);


        Assert.That(updatedOrderRequest.Id, Is.EqualTo(orderRequest.Id));
        Assert.That(updatedOrderRequest.Title, Is.EqualTo(orderRequest.Title));
        Assert.That(updatedOrderRequest.Description, Is.EqualTo(orderRequest.Description));
        Assert.That(updatedOrderRequest.MaxPrice, Is.EqualTo(orderRequest.MaxPrice));        
    }

    //GetUpdatedOrderRequestModelAsync tests>


    //<GetUserActiveRequestsCountAsync tests

    [Test]
    public async Task GetUserActiveRequestsCountAsync_MustReturnTheCountOfActiveRequestsCreatedByUser()
    {
        //Arrange
        string userId = "User1";
        int requestsCount = 10;

        _orderRequestRepositoryMock
            .Setup(orr => orr.GetUserActiveRequestsCountAsync(It.IsAny<string>()))
            .ReturnsAsync(requestsCount);

        OrderRequestService orderRequestService = new OrderRequestService(orderRequestRepository: _orderRequestRepositoryMock.Object);


        //Act
        int countOfRequestsCreatedByUser = await orderRequestService.GetUserActiveRequestsCountAsync(userId: userId);


        //Assert
        Assert.That(countOfRequestsCreatedByUser, Is.EqualTo(requestsCount));
    }

    //<GetUserActiveRequestsCountAsync tests
}
