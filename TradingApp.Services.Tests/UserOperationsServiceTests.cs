using Microsoft.AspNetCore.Identity;
using Moq;
using TradingApp.Data.Models;
using TradingApp.Data.Repository.Interfaces;
using TradingApp.GCommon;
using TradingApp.GCommon.ErrorCodes;
using TradingApp.Services.Core;
using TradingApp.ViewModels.InputUser;

namespace TradingApp.Services.Tests;

public class UserOperationsServiceTests
{
    private Mock<RoleManager<IdentityRole>> _roleManagerMock;
    private Mock<UserManager<User>> _userManagerMock;
    private Mock<IUserRepository> _userRepositoryMock;


    [SetUp]
    public void Setup()
    {
        var userStore = new Mock<IUserStore<User>>();
        var roleStore = new Mock<IRoleStore<IdentityRole>>();

        _roleManagerMock = new Mock<RoleManager<IdentityRole>>(roleStore.Object, null, null, null, null);
        _userManagerMock = new Mock<UserManager<User>>(userStore.Object, null, null, null, null, null, null, null, null);
        _userRepositoryMock = new Mock<IUserRepository>();
    }

    //< ManageUserAsync tests
    private (ManagedUserModel, string) SetupValidScenario_ManageUserAsync()
    {
        string managedUserCurrentRole = ApplicationRoles.User;
        string managedUserNewRole = ApplicationRoles.Moderator;

        ManagedUserModel managedUser = new ManagedUserModel()
        {
            UserId = "user Id",
            Role = managedUserCurrentRole,
            DaysToSuspend = 0,
            LockoutMessage = ""
        };


        _userManagerMock
            .Setup(ur => ur.FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new User() { Id = managedUser.UserId });

        _userManagerMock
            .Setup(pr => pr.GetRolesAsync(It.IsAny<User>()))
            .ReturnsAsync(new List<string> { managedUser.Role });

        var roles = new List<IdentityRole>
        {
            new IdentityRole { Name = ApplicationRoles.Admin },
            new IdentityRole { Name = ApplicationRoles.User },
            new IdentityRole { Name = ApplicationRoles.Moderator }
        }.AsQueryable();

        _roleManagerMock
            .Setup(rm => rm.Roles)
            .Returns(roles);



        _userManagerMock
            .Setup(pr => pr.IsInRoleAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(false);

        return (managedUser, managedUserNewRole);
    }


    [Test]
    public async Task ManageUserAsync_MustReturnErrorCodeUserNotFound_WhenTheManagedUserIdDoesNotExist()
    {
        //Arrange
        (ManagedUserModel managedUser, string managedUserNewRole) = SetupValidScenario_ManageUserAsync();

        _userManagerMock
           .Setup(ur => ur.FindByIdAsync(It.IsAny<string>()))
           .ReturnsAsync((User)null);

        UserOperationsService userOperationsService = new UserOperationsService(userRepository: _userRepositoryMock.Object,
            roleManager: _roleManagerMock.Object, userManager: _userManagerMock.Object);


        //Act
        Result result = await userOperationsService.ManageUserAsync(managedUser);


        //Assert
        Assert.That(result.Success, Is.EqualTo(false));
        Assert.That(result.ErrorCode, Is.EqualTo(UserErrorCodes.UserNotFound));
    }


    [Test]
    public async Task ManageUserAsync_MustReturnErrorCodeReadOnlyRoleUpdateAttempt_WhenCurrentRoleOfManagedUserIsAdmin()
    {
        //Arrange
        (ManagedUserModel managedUser, string managedUserNewRole) = SetupValidScenario_ManageUserAsync();

        _userManagerMock
           .Setup(ur => ur.GetRolesAsync(It.IsAny<User>()))
           .ReturnsAsync(new List<string>() { ApplicationRoles.Admin });

        UserOperationsService userOperationsService = new UserOperationsService(userRepository: _userRepositoryMock.Object,
            roleManager: _roleManagerMock.Object, userManager: _userManagerMock.Object);


        //Act
        Result result = await userOperationsService.ManageUserAsync(managedUser);


        //Assert
        Assert.That(result.Success, Is.EqualTo(false));
        Assert.That(result.ErrorCode, Is.EqualTo(RoleErrorCodes.ReadOnlyRole_UpdateAttempt));
    }

    [Test]
    public async Task ManageUserAsync_MustReturnErrorCodeReadOnlyRoleUpdateAttempt_WhenNewRoleOfManagedUserIsAdmin()
    {
        //Arrange
        (ManagedUserModel managedUser, string managedUserNewRole) = SetupValidScenario_ManageUserAsync();

        managedUser.Role = ApplicationRoles.Admin;

        UserOperationsService userOperationsService = new UserOperationsService(userRepository: _userRepositoryMock.Object,
           roleManager: _roleManagerMock.Object, userManager: _userManagerMock.Object);


        //Act
        Result result = await userOperationsService.ManageUserAsync(managedUser);


        //Assert
        Assert.That(result.Success, Is.EqualTo(false));
        Assert.That(result.ErrorCode, Is.EqualTo(RoleErrorCodes.ReadOnlyRole_UpdateAttempt));
    }

    [Test]
    public async Task ManageUserAsync_MustReturnErrorCodeRoleNotFound_WhenNewRoleForUserDoesNotExist()
    {
        //Arrange
        (ManagedUserModel managedUser, string managedUserNewRole) = SetupValidScenario_ManageUserAsync();

        managedUser.Role = "fake role";

        UserOperationsService userOperationsService = new UserOperationsService(userRepository: _userRepositoryMock.Object,
           roleManager: _roleManagerMock.Object, userManager: _userManagerMock.Object);


        //Act
        Result result = await userOperationsService.ManageUserAsync(managedUser);


        //Assert
        Assert.That(result.Success, Is.EqualTo(false));
        Assert.That(result.ErrorCode, Is.EqualTo(RoleErrorCodes.RoleNotFound));
    }

    [Test]
    public async Task ManageUserAsync_MustReturnErrorCodeLockedUserWithoutLockoutMessage_WhenUserIsAboutToBeSuspendedWithoutLockotMessage()
    {
        //Arrange
        (ManagedUserModel managedUser, string managedUserNewRole) = SetupValidScenario_ManageUserAsync();

        managedUser.DaysToSuspend = 1;
        managedUser.LockoutMessage = "";

        UserOperationsService userOperationsService = new UserOperationsService(userRepository: _userRepositoryMock.Object,
           roleManager: _roleManagerMock.Object, userManager: _userManagerMock.Object);


        //Act
        Result result = await userOperationsService.ManageUserAsync(managedUser);


        //Assert
        Assert.That(result.Success, Is.EqualTo(false));
        Assert.That(result.ErrorCode, Is.EqualTo(UserErrorCodes.LockedUserWithoutLockoutMessage));
    }


    [Test]
    public async Task ManageUserAsync_UserMustBeUpdated_WhenAllChecksPass_v1()
    {
        //Arrange
        (ManagedUserModel managedUser, string managedUserNewRole) = SetupValidScenario_ManageUserAsync();
                
        managedUser.DaysToSuspend = 1;
        managedUser.LockoutMessage = "you are suspended";
        DateTimeOffset lockoutEnd = new DateTimeOffset(DateTimeOffset.UtcNow.Date.AddDays(managedUser.DaysToSuspend), TimeSpan.Zero);

        UserOperationsService userOperationsService = new UserOperationsService(userRepository: _userRepositoryMock.Object,
           roleManager: _roleManagerMock.Object, userManager: _userManagerMock.Object);


        //Act
        Result result = await userOperationsService.ManageUserAsync(managedUser);


        //Assert
        Assert.That(result.Success, Is.EqualTo(true));
        Assert.That(result.ErrorCode, Is.EqualTo(string.Empty));

        _userRepositoryMock
            .Verify(ur => ur.ManageUserAsync(It.Is<User>(u => u.Id == managedUser.UserId), managedUser.LockoutMessage, true, lockoutEnd)
            ,Times.Once);
    }

    [Test]
    public async Task ManageUserAsync_UserMustBeUpdated_WhenAllChecksPass_v2()
    {
        //Arrange
        (ManagedUserModel managedUser, string managedUserNewRole) = SetupValidScenario_ManageUserAsync();

        managedUser.DaysToSuspend = 0;
        managedUser.LockoutMessage = "you are suspended";
        DateTimeOffset lockoutEnd = new DateTimeOffset(DateTimeOffset.UtcNow.Date.AddDays(managedUser.DaysToSuspend), TimeSpan.Zero);

        UserOperationsService userOperationsService = new UserOperationsService(userRepository: _userRepositoryMock.Object,
           roleManager: _roleManagerMock.Object, userManager: _userManagerMock.Object);


        //Act
        Result result = await userOperationsService.ManageUserAsync(managedUser);


        //Assert
        Assert.That(result.Success, Is.EqualTo(true));
        Assert.That(result.ErrorCode, Is.EqualTo(string.Empty));

        _userRepositoryMock
            .Verify(ur => ur.ManageUserAsync(It.Is<User>(u => u.Id == managedUser.UserId), null, true, lockoutEnd)
            , Times.Once);
    }

    // ManageUserAsync tests>
}
