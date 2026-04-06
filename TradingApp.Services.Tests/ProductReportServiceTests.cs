
using Moq;
using TradingApp.Data.Dtos.ProductReport;
using TradingApp.Data.Repository.Interfaces;
using TradingApp.GCommon;
using TradingApp.GCommon.Enums;
using TradingApp.Services.Core;
using TradingApp.ViewModels.ProductReport;

namespace TradingApp.Services.Tests;

public class ProductReportServiceTests
{
    private Mock<IProductReportRepository> _productReportRepositoryMock;

    [SetUp]
    public void Setup()
    {
        _productReportRepositoryMock = new Mock<IProductReportRepository>();
    }


    //<GetReportsAsync tests

    [Test]
    public async Task GetReportsAsync_MustReturnEmptyCollection_WhenThereAreNoProductReports()
    {
        //Arrange
        int pageIndex = 5;
        int reportsCount = 0;

        _productReportRepositoryMock
            .Setup(prr => prr.GetReportsCountAsync())
            .ReturnsAsync(reportsCount);

        ProductReportService productReportService = new ProductReportService(productReportRepository: _productReportRepositoryMock.Object);


        //Act
        List<ProductReportViewModel> reports = await productReportService.GetReportsAsync(pageIndex: pageIndex);


        //Assert
        Assert.That(reports, Is.Empty);
    }


    [Test]
    public async Task GetReportsAsync_MustReturnNonEmptyCollection_WhenThereAreProductReports()
    {
        //Arrange
        int reportsPerPage = ApplicationConstants.ProductReportsPerPage;
        int pageIndex = 5;
        int reportsCount = reportsPerPage * pageIndex * 2;

        List<ProductReportDto> reportsDtos = new List<ProductReportDto>()
        {
            new ProductReportDto()
            {
                ReportId = Guid.Parse("8118e3a6-63b4-474e-a62d-5cf549af3568"),
                Title = "title 1",
                CreatedAt = new DateTime(2000,6,20),
                Type = ProductReportType.spam,
                Status = ProductReportStatus.open,
            },
             new ProductReportDto()
            {
                ReportId = Guid.Parse("4e0545d5-d160-4d9b-8efe-6d0606d18335"),
                Title = "title 2",
                CreatedAt = new DateTime(2000,6,21),
                Type = ProductReportType.misleading,
                Status = ProductReportStatus.resolved,
            },
              new ProductReportDto()
            {
                ReportId = Guid.Parse("6dfe1dc8-daec-401b-a691-e6ad85f949cf"),
                Title = "title 3",
                CreatedAt = new DateTime(2000,6,22),
                Type = ProductReportType.other,
                Status = ProductReportStatus.in_review,
            }
        };

        _productReportRepositoryMock
            .Setup(prr => prr.GetReportsCountAsync())
            .ReturnsAsync(reportsCount);

        _productReportRepositoryMock
            .Setup(prr => prr.GetProductReportsAsync(reportsPerPage * pageIndex, reportsPerPage))
            .ReturnsAsync(reportsDtos);

        ProductReportService productReportService = new ProductReportService(productReportRepository: _productReportRepositoryMock.Object);


        //Act
        List<ProductReportViewModel> reports = await productReportService.GetReportsAsync(pageIndex: pageIndex);


        //Assert
        Assert.That(reports, Is.Not.Empty);

        for (int i = 0; i < reports.Count; i++)
        {
            Assert.That(reports[i].ReportId, Is.EqualTo(reportsDtos[i].ReportId));
            Assert.That(reports[i].Title, Is.EqualTo(reportsDtos[i].Title));
            Assert.That(reports[i].Type, Is.EqualTo(reportsDtos[i].Type.ToString()));
            Assert.That(reports[i].Status, Is.EqualTo(reportsDtos[i].Status.ToString()));
        }

        Assert.That(productReportService.ProductReportPageIndex, Is.EqualTo(pageIndex));
    }

    //<GetReportsAsync tests




    //<GetReportsForProductAsync tests

    [Test]
    public async Task GetReportsForProductAsync_MustReturnEmptyCollection_WhenTheProductHasNoReports()
    {
        //Arrange
        int pageIndex = 7;
        int reportsCount = 0;
        Guid productId = Guid.Parse("55ae9bc2-986b-446d-9203-391daf739022");

        _productReportRepositoryMock
            .Setup(prr => prr.GetReportsCountForProductAsync(productId))
            .ReturnsAsync(reportsCount);

        ProductReportService productReportService = new ProductReportService(productReportRepository: _productReportRepositoryMock.Object);


        //Act
        List<ProductReportViewModel> reports = await productReportService.GetReportsForProductAsync(pageIndex: pageIndex, reportedProductId:productId);


        //Assert
        Assert.That(reports, Is.Empty);
                
    }

    [Test]
    public async Task GetReportsForProductAsync_MustReturnNonEmptyCollection_WhenTheProductHasReports()
    {
        //Arrange
        int reportsPerPage = ApplicationConstants.ProductReportsPerPage;
        int pageIndex = 7;
        int reportsCount = reportsPerPage * pageIndex * 2;
        Guid productId = Guid.Parse("55ae9bc2-986b-446d-9203-391daf739022");

        List<ProductReportDto> reportsDtos = new List<ProductReportDto>()
        {
            new ProductReportDto()
            {
                ReportId = Guid.Parse("8118e3a6-63b4-474e-a62d-5cf549af3568"),
                Title = "title 1",
                CreatedAt = new DateTime(2000,6,20),
                Type = ProductReportType.spam,
                Status = ProductReportStatus.open,
            },
             new ProductReportDto()
            {
                ReportId = Guid.Parse("4e0545d5-d160-4d9b-8efe-6d0606d18335"),
                Title = "title 2",
                CreatedAt = new DateTime(2000,6,21),
                Type = ProductReportType.misleading,
                Status = ProductReportStatus.resolved,
            },
              new ProductReportDto()
            {
                ReportId = Guid.Parse("6dfe1dc8-daec-401b-a691-e6ad85f949cf"),
                Title = "title 3",
                CreatedAt = new DateTime(2000,6,22),
                Type = ProductReportType.other,
                Status = ProductReportStatus.in_review,
            }
        };

        _productReportRepositoryMock
            .Setup(prr => prr.GetReportsCountForProductAsync(productId))
            .ReturnsAsync(reportsCount);

        _productReportRepositoryMock
            .Setup(prr => prr.GetReportsForProductAsync(reportsPerPage * pageIndex, reportsPerPage, productId))
            .ReturnsAsync(reportsDtos);

        ProductReportService productReportService = new ProductReportService(productReportRepository: _productReportRepositoryMock.Object);


        //Act
        List<ProductReportViewModel> reports = await productReportService.GetReportsForProductAsync(pageIndex: pageIndex, reportedProductId: productId);


        //Assert
        Assert.That(reports, Is.Not.Empty);

        for (int i = 0; i < reports.Count; i++)
        {
            Assert.That(reports[i].ReportId, Is.EqualTo(reportsDtos[i].ReportId));
            Assert.That(reports[i].Title, Is.EqualTo(reportsDtos[i].Title));
            Assert.That(reports[i].Type, Is.EqualTo(reportsDtos[i].Type.ToString()));
            Assert.That(reports[i].Status, Is.EqualTo(reportsDtos[i].Status.ToString()));
        }

        Assert.That(productReportService.ProductReportPageIndex, Is.EqualTo(pageIndex));
    }

    //GetReportsForProductAsync tests>



    //<GetProductReportAsync tests

    [Test]
    public async Task GetProductReportAsync_MustReturnNull_WhenTheReportDoesNotExist()
    {
        //Arrange        
        Guid productId = Guid.Parse("55ae9bc2-986b-446d-9203-391daf739022");

        ProductReportDetailsDto reportDto = new ProductReportDetailsDto()
        {
            ReportId = Guid.Parse("8118e3a6-63b4-474e-a62d-5cf549af3568"),
            Title = "title",
            CreatedAt = new DateTime(2000, 6, 20),
            Type = ProductReportType.spam,
            Status = ProductReportStatus.open,

            ReportedProductId = productId,
            ReporterName = "reporter name",
            Message = "message",
        };

        _productReportRepositoryMock
            .Setup(prr => prr.GetProductReportAsync(productId))
            .ReturnsAsync((ProductReportDetailsDto)null);

        ProductReportService productReportService = new ProductReportService(productReportRepository: _productReportRepositoryMock.Object);


        //Act
        ProductReportViewModel? report = await productReportService.GetProductReportAsync(reportId: reportDto.ReportId);


        //Assert
        Assert.That(report, Is.Null);

    }


    [Test]
    public async Task GetProductReportAsync_MustReturnProductReport_WhenTheReportExists()
    {
        //Arrange        
        Guid productId = Guid.Parse("55ae9bc2-986b-446d-9203-391daf739022");

        ProductReportDetailsDto reportDto = new ProductReportDetailsDto()
        {
            ReportId = Guid.Parse("8118e3a6-63b4-474e-a62d-5cf549af3568"),
            Title = "title",
            CreatedAt = new DateTime(2000, 6, 20),
            Type = ProductReportType.spam,
            Status = ProductReportStatus.open,

            ReportedProductId = productId,
            ReporterName = "reporter name",
            Message = "message",
        };

        _productReportRepositoryMock
            .Setup(prr => prr.GetProductReportAsync(reportDto.ReportId))
            .ReturnsAsync(reportDto);

        ProductReportService productReportService = new ProductReportService(productReportRepository: _productReportRepositoryMock.Object);


        //Act
        ProductReportDetailsViewModel? report = await productReportService.GetProductReportAsync(reportId: reportDto.ReportId);


        //Assert
        Assert.That(report, Is.Not.Null);

        Assert.That(report.ReportId, Is.EqualTo(reportDto.ReportId));
        Assert.That(report.Title, Is.EqualTo(reportDto.Title));
        Assert.That(DateTime.Parse(report.CreatedAt), Is.EqualTo(reportDto.CreatedAt));
        Assert.That(report.Type, Is.EqualTo(reportDto.Type.ToString()));
        Assert.That(report.Status, Is.EqualTo(reportDto.Status.ToString()));

        Assert.That(report.ReportedProductId, Is.EqualTo(reportDto.ReportedProductId));
        Assert.That(report.ReporterName, Is.EqualTo(reportDto.ReporterName));
        Assert.That(report.Message, Is.EqualTo(reportDto.Message));
    }

    //GetProductReportAsync tests>
}