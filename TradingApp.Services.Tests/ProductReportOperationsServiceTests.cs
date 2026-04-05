
using Moq;
using TradingApp.Data.Models;
using TradingApp.Data.Repository;
using TradingApp.Data.Repository.Interfaces;
using TradingApp.GCommon;
using TradingApp.GCommon.Enums;
using TradingApp.GCommon.ErrorCodes;
using TradingApp.Services.Core;

namespace TradingApp.Services.Tests;

public class ProductReportOperationsServiceTests
{
    private Mock<IProductReportRepository> _productReportRepositoryMock;
    private Mock<IProductRepository> _productRepositoryMock;
    private Mock<IUserRepository> _userRepositoryMock;
    [SetUp]
    public void Setup()
    {
        _productReportRepositoryMock = new Mock<IProductReportRepository>();
        _productRepositoryMock = new Mock<IProductRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
    }


    //<CreateReportAsync tests

    private ProductReport SetupValidScenario_ForCreateReportAsync()
    {
        _userRepositoryMock
            .Setup(ur => ur.DoesUserExistAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        _productRepositoryMock
           .Setup(pr => pr.DoesProductExistAsync(It.IsAny<Guid>()))
           .ReturnsAsync(true);

        _productRepositoryMock
            .Setup(pr => pr.DoesProductCreatedByUserExistAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(false);


        ProductReport productReport = new ProductReport()
        {
            ReporterId = "reporter Id",
            ReportedProductId = Guid.Parse("8118e3a6-63b4-474e-a62d-5cf549af3568"),
            Title = "title",
            Message = "message",
            CreatedAt = new DateTime(2000, 6, 25),
            Type = ProductReportType.other,
            Status = ProductReportStatus.open
        };

        return productReport;
    }

    [Test]
    public async Task CreateReportAsync_MustReturnErrorCodeUserNotFound_WhenTheUserIdDoesNotExist()
    {
        //Arrange
        ProductReport productReport = SetupValidScenario_ForCreateReportAsync();

        _userRepositoryMock
           .Setup(ur => ur.DoesUserExistAsync(It.IsAny<string>()))
           .ReturnsAsync(false);

        ProductReportOperationsService productReportOperationsService = new ProductReportOperationsService(productReportRepository:_productReportRepositoryMock.Object, productRepository:_productRepositoryMock.Object, userRepository:_userRepositoryMock.Object);

        //Act
        Result result = await productReportOperationsService.CreateReportAsync(reporterId: productReport.ReporterId, reportedProductId: productReport.ReportedProductId, title: productReport.Title, message: productReport.Message, reportType:productReport.Type);

        //Assert
        Assert.That(result.Success, Is.EqualTo(false));
        Assert.That(result.ErrorCode, Is.EqualTo(UserErrorCodes.UserNotFound));
    }

    [Test]
    public async Task CreateReportAsync_MustReturnErrorCodeProductNotFound_WhenTheProductDoesNotExist()
    {
        //Arrange
        ProductReport productReport = SetupValidScenario_ForCreateReportAsync();

        _productRepositoryMock
           .Setup(ur => ur.DoesProductExistAsync(It.IsAny<Guid>()))
           .ReturnsAsync(false);

        ProductReportOperationsService productReportOperationsService = new ProductReportOperationsService(productReportRepository: _productReportRepositoryMock.Object, productRepository: _productRepositoryMock.Object, userRepository: _userRepositoryMock.Object);

        //Act
        Result result = await productReportOperationsService.CreateReportAsync(reporterId: productReport.ReporterId, reportedProductId: productReport.ReportedProductId, title: productReport.Title, message: productReport.Message, reportType: productReport.Type);

        //Assert
        Assert.That(result.Success, Is.EqualTo(false));
        Assert.That(result.ErrorCode, Is.EqualTo(ProductErrorCodes.ProductNotFound));
    }

    [Test]
    public async Task CreateReportAsync_MustReturnErrorCodeProductReportInvalidCreator_WhenTheUserTriesToReportAProductHeOwns()
    {
        //Arrange
        ProductReport productReport = SetupValidScenario_ForCreateReportAsync();

        _productRepositoryMock
           .Setup(ur => ur.DoesProductCreatedByUserExistAsync(It.IsAny<string>(), It.IsAny<Guid>()))
           .ReturnsAsync(true);

        ProductReportOperationsService productReportOperationsService = new ProductReportOperationsService(productReportRepository: _productReportRepositoryMock.Object, productRepository: _productRepositoryMock.Object, userRepository: _userRepositoryMock.Object);

        //Act
        Result result = await productReportOperationsService.CreateReportAsync(reporterId: productReport.ReporterId, reportedProductId: productReport.ReportedProductId, title: productReport.Title, message: productReport.Message, reportType: productReport.Type);

        //Assert
        Assert.That(result.Success, Is.EqualTo(false));
        Assert.That(result.ErrorCode, Is.EqualTo(ProductReportErrorCodes.ProductReportInvalidCreator));
    }


    [Test]
    public async Task CreateReportAsync_MustCreateReport_WhenAllChecksPass()
    {
        //Arrange
        ProductReport productReport = SetupValidScenario_ForCreateReportAsync();
            
        ProductReportOperationsService productReportOperationsService = new ProductReportOperationsService(productReportRepository: _productReportRepositoryMock.Object, productRepository: _productRepositoryMock.Object, userRepository: _userRepositoryMock.Object);

        //Act
        Result result = await productReportOperationsService.CreateReportAsync(reporterId: productReport.ReporterId, reportedProductId: productReport.ReportedProductId, title: productReport.Title, message: productReport.Message, reportType: productReport.Type);

        //Assert
        Assert.That(result.Success, Is.EqualTo(true));
        Assert.That(result.ErrorCode, Is.EqualTo(string.Empty));

        _productReportRepositoryMock
            .Verify(prr => prr.CreateReportAsync(
                It.Is<ProductReport>(pr =>
                pr.Title == productReport.Title && pr.Message == productReport.Message &&
                pr.ReportedProductId == productReport.ReportedProductId && pr.ReporterId == productReport.ReporterId &&
                pr.Type == productReport.Type && pr.Status == productReport.Status
                ))
            , Times.Once);
    }
    //CreateReportAsync tests>



    //<SetReportStatusAsync tests

    private (ProductReport, ProductReportStatus) SetupValidScenario_SetReportStatusAsync()
    {

        ProductReport productReport = new ProductReport()
        {
            Id = Guid.Parse("4e0545d5-d160-4d9b-8efe-6d0606d18335"),
            ReporterId = "reporter Id",
            ReportedProductId = Guid.Parse("8118e3a6-63b4-474e-a62d-5cf549af3568"),
            Title = "title",
            Message = "message",
            CreatedAt = new DateTime(2000, 6, 25),
            Type = ProductReportType.other,
            Status = ProductReportStatus.open
        };
        ProductReportStatus newStatus = ProductReportStatus.in_review;

        _productReportRepositoryMock
           .Setup(pr => pr.GetProductReportByIdAsync(It.IsAny<Guid>()))
           .ReturnsAsync(productReport);

        return (productReport, newStatus);
    }

    [Test]
    public async Task SetReportStatusAsync_MustReturnErrorCodeProductReportNotFound_WhenTheProductReportDoesNotExist()
    {
        //Arrange
        (ProductReport productReport, ProductReportStatus newStatus) = SetupValidScenario_SetReportStatusAsync();

        _productReportRepositoryMock
            .Setup(prr => prr.GetProductReportByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((ProductReport)null);

        ProductReportOperationsService productReportOperationsService = new ProductReportOperationsService(productReportRepository: _productReportRepositoryMock.Object, productRepository: _productRepositoryMock.Object, userRepository: _userRepositoryMock.Object);


        //Act
        Result result = await productReportOperationsService.SetReportStatusAsync(reportId: productReport.Id, newReportStatus: newStatus);


        //Assert
        Assert.That(result.Success, Is.EqualTo(false));
        Assert.That(result.ErrorCode, Is.EqualTo(ProductReportErrorCodes.ProductReportNotFound));

    }


    [Test]
    public async Task SetReportStatusAsync_MustSetReportStatus_WhenAllChecksPass()
    {
        //Arrange
        (ProductReport productReport, ProductReportStatus newStatus) = SetupValidScenario_SetReportStatusAsync();
               
        ProductReportOperationsService productReportOperationsService = new ProductReportOperationsService(productReportRepository: _productReportRepositoryMock.Object, productRepository: _productRepositoryMock.Object, userRepository: _userRepositoryMock.Object);


        //Act
        Result result = await productReportOperationsService.SetReportStatusAsync(reportId: productReport.Id, newReportStatus: newStatus);


        //Assert
        Assert.That(result.Success, Is.EqualTo(true));
        Assert.That(result.ErrorCode, Is.EqualTo(string.Empty));

        _productReportRepositoryMock
            .Verify(prr => prr.SetReportStatusAsync(
            It.Is<ProductReport>(pr => pr.Id == productReport.Id && pr.Status == productReport.Status)
            , newStatus)
            , Times.Once);
    }


    //SetReportStatusAsync tests>
}
