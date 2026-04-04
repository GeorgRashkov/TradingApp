using Moq;
using System.Threading.Tasks;
using TradingApp.Data.Dtos.CompletedOrder;
using TradingApp.Data.Models;
using TradingApp.Data.Repository.Interfaces;
using TradingApp.GCommon;
using TradingApp.Services.Core;
using TradingApp.Services.Core.Interfaces;
using TradingApp.ViewModels.Invoice;

namespace TradingApp.Services.Tests;

public class InvoiceServiceTests
{
    private Mock<ICompletedOrderRepository> _completedOrderRepositoryMock;   
    [SetUp]
    public void Setup()
    {
        _completedOrderRepositoryMock = new Mock<ICompletedOrderRepository>();
    }

    //< GetCompletedOrdersAsync tests
    [Test]
    public async Task GetCompletedOrdersAsync_MustReturnEmptyCollectionInvoiceViewModel_WhenUserHasZeroCompletedOrders()
    {
        //Arrange
        string userId = "123";
        int pageIndex = 1;

        _completedOrderRepositoryMock.Setup(cor => cor.GetCompletedOrdersCountAsync(userId)).ReturnsAsync(0);

        IInvoiceService invoiceService = new InvoiceService(completedOrderRepository: _completedOrderRepositoryMock.Object);


        //Act
        IEnumerable<InvoiceViewModel> invoices = await invoiceService.GetCompletedOrdersAsync(userId: userId, pageIndex: pageIndex);

        //Assert
        Assert.That(invoices, Is.Empty);
        
    }


    [Test]
    public async Task GetCompletedOrdersAsync_MustReturnNonEmptyCollectionInvoiceViewModel_WhenUserHasOneOrMoreCompletedOrders()
    {
        //Arrange
        string userId = "123";
        int pageIndex = 5;        
        int takeCount = ApplicationConstants.InvoicesPerPage;
        int skipCount = pageIndex* takeCount;
        int userAllCompletedOrdersCount = skipCount*2;

        List<CompletedOrderDto> userCompletedOrdersDtos = new List<CompletedOrderDto>
        {
            new CompletedOrderDto
            {
                Id = Guid.Parse("8118e3a6-63b4-474e-a62d-5cf549af3568"),
                BuyerId = userId,
                TitleForBuyer = "BuyerTitle1",
                TitleForSeller = "SellerTitle1",
                CompletedAt = new DateTime(2000, 6, 25)
            },
             new CompletedOrderDto
            {
                Id = Guid.Parse("4e0545d5-d160-4d9b-8efe-6d0606d18335"),
                BuyerId = "buyerId1",
                TitleForBuyer = "BuyerTitle2",
                TitleForSeller = "SellerTitle2",
                CompletedAt = new DateTime(2010, 6, 25)
            },
              new CompletedOrderDto
            {
                Id = Guid.Parse("6dfe1dc8-daec-401b-a691-e6ad85f949cf"),
                BuyerId = "buyerId2",
                TitleForBuyer = "BuyerTitle3",
                TitleForSeller = "SellerTitle3",
                CompletedAt = new DateTime(2020, 6, 25)
            }
        };
        
        _completedOrderRepositoryMock.Setup(cor => cor.GetCompletedOrdersCountAsync(userId)).ReturnsAsync(userAllCompletedOrdersCount);
        _completedOrderRepositoryMock.Setup(cor => cor.GetCompletedOrdersAsync(userId, skipCount, takeCount)).ReturnsAsync(userCompletedOrdersDtos);

        IInvoiceService invoiceService = new InvoiceService(completedOrderRepository: _completedOrderRepositoryMock.Object);


        //Act
        List<InvoiceViewModel> userCompletedOrders = (await invoiceService.GetCompletedOrdersAsync(userId: userId, pageIndex: pageIndex)).ToList();


        //Assert 
        Assert.That(invoiceService.InvoicePageIndex, Is.EqualTo(pageIndex));

        for (int i = 0; i < userCompletedOrders.Count(); i++)
        {
            Assert.That(userCompletedOrders[i].Id, Is.EqualTo(userCompletedOrdersDtos[i].Id));
        }

        Assert.That(userCompletedOrders.Count(), Is.EqualTo(userCompletedOrdersDtos.Count()));
        
    }
    // GetCompletedOrdersAsync tests>


    //< GetCompletedOrderAsync tests
    [Test]
    public async Task GetCompletedOrderAsync_MustReturnNull_WhenCompletedOrderDoesNotExist()
    {
        //Arrange
        string userId = "123";
        Guid nonExistingCompletedOrderId = Guid.Parse("8118e3a6-63b4-474e-a62d-5cf549af3568");

        _completedOrderRepositoryMock.Setup(cor => cor.GetCompletedOrderAsync(nonExistingCompletedOrderId)).ReturnsAsync((CompletedOrder)null);
        IInvoiceService invoiceService = new InvoiceService(completedOrderRepository: _completedOrderRepositoryMock.Object);


        //Act
        InvoiceDetailsViewModel? invoiceDetails = await invoiceService.GetCompletedOrderAsync(userId: userId, completedOrderId: nonExistingCompletedOrderId);

        //Assert
        Assert.That(invoiceDetails, Is.Null);
    }


    [Test]
    public async Task GetCompletedOrderAsync_MustReturnNull_WhenCompletedOrderExistAndDoesNotBelongToUser()
    {
        //Arrange
        string userId = "123";
        Guid completedOrderId = Guid.Parse("8118e3a6-63b4-474e-a62d-5cf549af3568");
        CompletedOrder completedOrder = new CompletedOrder
        {
            BuyerId = "buyerId1",
            SellerId = "sellerId1",           
        };

        _completedOrderRepositoryMock.Setup(cor => cor.GetCompletedOrderAsync(completedOrderId)).ReturnsAsync(completedOrder);
        IInvoiceService invoiceService = new InvoiceService(completedOrderRepository: _completedOrderRepositoryMock.Object);


        //Act
        InvoiceDetailsViewModel? invoiceDetails = await invoiceService.GetCompletedOrderAsync(userId: userId, completedOrderId: completedOrderId);

        //Assert
        Assert.That(invoiceDetails, Is.Null);
    }


    [Test]
    public async Task GetCompletedOrderAsync_MustReturnInvoceDetails_WhenCompletedOrderExistAndBelongsToBuyer()
    {
        //Arrange
        string userId = "123";
        Guid completedOrderId = Guid.Parse("8118e3a6-63b4-474e-a62d-5cf549af3568");
        CompletedOrder completedOrder = new CompletedOrder
        {
            BuyerId = userId,
            SellerId = "sellerId1",
            PricePaid = 100,
            PlatformFee = 10,
            SellerRevenue = 90,
            TitleForBuyer = "BuyerTitle1",
            TitleForSeller = "SellerTitle1"
        };

        _completedOrderRepositoryMock.Setup(cor => cor.GetCompletedOrderAsync(completedOrderId)).ReturnsAsync(completedOrder);
        IInvoiceService invoiceService = new InvoiceService(completedOrderRepository: _completedOrderRepositoryMock.Object);


        //Act
        InvoiceDetailsViewModel? invoiceDetails = await invoiceService.GetCompletedOrderAsync(userId: userId, completedOrderId: completedOrderId);

        //Assert
        Assert.That(invoiceDetails, Is.Not.Null);
        Assert.That(invoiceDetails.IsUserTheBuyer, Is.EqualTo(true));
        Assert.That(invoiceDetails.Price, Is.EqualTo(completedOrder.PricePaid.ToString("f2")));
        Assert.That(invoiceDetails.Title, Is.EqualTo(completedOrder.TitleForBuyer));
    }


    [Test]
    public async Task GetCompletedOrderAsync_MustReturnInvoceDetails_WhenCompletedOrderExistAndBelongsToSeller()
    {
        //Arrange
        string userId = "123";
        Guid completedOrderId = Guid.Parse("8118e3a6-63b4-474e-a62d-5cf549af3568");
        CompletedOrder completedOrder = new CompletedOrder
        {
            BuyerId = "buyerId1",
            SellerId = userId,
            PricePaid = 100,
            PlatformFee = 10,
            SellerRevenue = 90,
            TitleForBuyer = "BuyerTitle1",
            TitleForSeller = "SellerTitle1"
        };

        _completedOrderRepositoryMock.Setup(cor => cor.GetCompletedOrderAsync(completedOrderId)).ReturnsAsync(completedOrder);
        IInvoiceService invoiceService = new InvoiceService(completedOrderRepository: _completedOrderRepositoryMock.Object);


        //Act
        InvoiceDetailsViewModel? invoiceDetails = await invoiceService.GetCompletedOrderAsync(userId: userId, completedOrderId: completedOrderId);

        //Assert
        Assert.That(invoiceDetails, Is.Not.Null);
        Assert.That(invoiceDetails.IsUserTheBuyer, Is.EqualTo(false));
        Assert.That(invoiceDetails.Price, Is.EqualTo(completedOrder.SellerRevenue.ToString("f2")));
        Assert.That(invoiceDetails.Title, Is.EqualTo(completedOrder.TitleForSeller));
    }
    //GetCompletedOrderAsync tests>
}
