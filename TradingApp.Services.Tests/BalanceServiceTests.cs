using Microsoft.AspNetCore.Identity;
using Moq;
using System.Threading.Tasks;
using TradingApp.Data.Models;
using TradingApp.Data.Repository.Interfaces;
using TradingApp.Services.Core;

namespace TradingApp.Services.Tests
{
    public class Tests
    {
        private Mock<UserManager<User>> userManagerMock;
        private Mock<IBalanceRepository> balanceRepoMock;
        [SetUp]
        public void Setup()
        {
            var store = new Mock<IUserStore<User>>();

            userManagerMock = new Mock<UserManager<User>>(store.Object,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null);

            balanceRepoMock = new Mock<IBalanceRepository>();
        }


        //< CreateBalanceAsync tests
        [Test]
        public void CreateBalanceAsync_ShouldThrow_WhenUserDoesNotExist()
        {
            //Arrange
            string nonExistingUserId = "fakeUserId";

            userManagerMock
                .Setup(um => um.FindByIdAsync(nonExistingUserId))
                .ReturnsAsync((User)null);

            BalanceService balanceService = new BalanceService(balanceRepoMock.Object, userManagerMock.Object);

            //Act, Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await balanceService.CreateBalanceAsync(userId: nonExistingUserId));
        }

        [Test]
        public async Task CreateBalanceAsync_ShouldCreateBalance_WhenUserExists()
        {
            //Arrange
            string existingUserId = "123";

            userManagerMock
                .Setup(um => um.FindByIdAsync(existingUserId))
                .ReturnsAsync(new User() { Id = existingUserId });

            BalanceService balanceService = new BalanceService(balanceRepoMock.Object, userManagerMock.Object);


            //Act
            await balanceService.CreateBalanceAsync(userId: existingUserId);


            //Assert
            balanceRepoMock.Verify(br => br.CreateBalanceAsync(
                It.Is<Balance>(b =>
                b.Id == existingUserId && b.Amount == 0)),
                Times.Once);
        }

        //CreateBalanceAsync tests>


        //<IncreaseUserBalanceAsync tests
        [Test]
        public async Task IncreaseUserBalanceAsync_ShouldIncreaseBalance_WhenAmountPositive()
        {
            //Arrange
            string existingUserId = "123";
            decimal balanceIncrement = 100;

            BalanceService service = new BalanceService(balanceRepoMock.Object, userManagerMock.Object);


            //Act
            await service.IncreaseUserBalanceAsync(userId: existingUserId, increasement: balanceIncrement);


            //Assert
            balanceRepoMock.Verify(br => br.IncreaseBalanceAsync(existingUserId, balanceIncrement), Times.Once);
        }

        [Test]
        public async Task IncreaseUserBalanceAsync_ShouldNotIncreaseBalance_WhenAmountNegative() 
        {
            //Arrange
            string existingUserId = "123";
            decimal balanceIncrement = -100;
            BalanceService service = new BalanceService(balanceRepoMock.Object, userManagerMock.Object);

            //Act
            await service.IncreaseUserBalanceAsync(userId: existingUserId, increasement: balanceIncrement);

            //Assert
            balanceRepoMock.Verify(br => br.IncreaseBalanceAsync(existingUserId, It.IsAny<decimal>()), Times.Never);
        }

        //IncreaseUserBalanceAsync tests>


        //<DecreaseUserBalanceAsync tests
        [Test]
        public async Task DecreaseUserBalanceAsync_ShouldDecreaseBalance_WhenAmountPositive()
        {
            //Arrange
            string existingUserId = "123";
            decimal balanceDecrement = 100;

            BalanceService service = new BalanceService(balanceRepoMock.Object, userManagerMock.Object);


            //Act
            await service.DecreaseUserBalanceAsync(userId: existingUserId, decreasement: balanceDecrement);


            //Assert
            balanceRepoMock.Verify(br => br.DecreaseBalanceAsync(existingUserId, balanceDecrement), Times.Once);
        }

        [Test]
        public async Task DecreaseUserBalanceAsync_ShouldNotDecreaseBalance_WhenAmountNegative()
        {
            //Arrange
            string existingUserId = "123";
            decimal balanceDecrement = -100;
            BalanceService service = new BalanceService(balanceRepoMock.Object, userManagerMock.Object);

            //Act
            await service.DecreaseUserBalanceAsync(userId: existingUserId, decreasement: balanceDecrement);

            //Assert
            balanceRepoMock.Verify(br => br.DecreaseBalanceAsync(existingUserId, It.IsAny<decimal>()), Times.Never);
        }

        //DecreaseUserBalanceAsync tests>
    }
}