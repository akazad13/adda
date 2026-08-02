using Adda.API.Dtos;
using Adda.API.Helpers;
using Adda.API.Models;
using Adda.API.Repositories.UserRepository;
using Adda.API.Security.CurrentUserProvider;
using Adda.API.Services.UserService;
using Adda.API.Tests.Fixtures;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace Adda.API.Tests.Unit.Services;

/// <summary>
/// Unit tests for UserService
/// </summary>
public class UserServiceTests
{
    private readonly Mock<UserManager<User>> _mockUserManager;
    private readonly Mock<ICurrentUserProvider> _mockCurrentUserProvider;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _mockUserManager = new Mock<UserManager<User>>(
            new Mock<IUserStore<User>>().Object, null, null, null, null, null, null, null, null);
        _mockCurrentUserProvider = new Mock<ICurrentUserProvider>();
        _mockUserRepository = new Mock<IUserRepository>();

        _userService = new UserService(_mockUserManager.Object, _mockCurrentUserProvider.Object, _mockUserRepository.Object);
    }

    #region RegistrationAsync - Happy Path

    [Fact]
    public async Task RegistrationAsync_WithValidRequest_CreatesUserAndAssignsMemberRole()
    {
        // Arrange
        var request = new RegistrationRequest 
        { 
            UserName = "newuser", 
            Password = "Password123!"
        };

        var identityResult = IdentityResult.Success;
        User capturedUser = null!;

        _mockUserManager
            .Setup(um => um.FindByNameAsync(request.UserName))
            .ReturnsAsync((User)null!);

        _mockUserManager
            .Setup(um => um.CreateAsync(It.IsAny<User>(), request.Password))
            .Callback<User, string>((user, password) => 
            {
                capturedUser = user;
                user.UserName = request.UserName; // Simulate UserManager setting the username
            })
            .ReturnsAsync(identityResult);

        _mockUserManager
            .Setup(um => um.AddToRolesAsync(It.IsAny<User>(), It.IsAny<IEnumerable<string>>()))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _userService.RegistrationAsync(request);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.UserName.Should().Be(request.UserName);
        _mockUserManager.Verify(um => um.CreateAsync(It.IsAny<User>(), request.Password), Times.Once);
        _mockUserManager.Verify(um => um.AddToRolesAsync(It.IsAny<User>(), It.Is<IEnumerable<string>>(r => r.Contains("Member"))), Times.Once);
    }

    #endregion

    #region RegistrationAsync - Error Cases

    [Fact]
    public async Task RegistrationAsync_WithExistingUsername_ReturnsValidationError()
    {
        // Arrange
        var existingUser = TestDataFactory.CreateUser(1, "existinguser");
        var request = new RegistrationRequest 
        { 
            UserName = "existinguser", 
            Password = "Password123!" 
        };

        _mockUserManager
            .Setup(um => um.FindByNameAsync(request.UserName))
            .ReturnsAsync(existingUser);

        // Act
        var result = await _userService.RegistrationAsync(request);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Description.Should().Contain("UserName already exists");
    }

    [Fact]
    public async Task RegistrationAsync_WhenCreateAsyncFails_ReturnsFailureError()
    {
        // Arrange
        var request = new RegistrationRequest 
        { 
            UserName = "newuser", 
            Password = "Password123!" 
        };

        _mockUserManager
            .Setup(um => um.FindByNameAsync(request.UserName))
            .ReturnsAsync((User)null!);

        var identityErrors = new[] { new IdentityError { Description = "Password too weak" } };
        var failureResult = IdentityResult.Failed(identityErrors);

        _mockUserManager
            .Setup(um => um.CreateAsync(It.IsAny<User>(), request.Password))
            .ReturnsAsync(failureResult);

        // Act
        var result = await _userService.RegistrationAsync(request);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Description.Should().Contain("Couldn't create user");
    }

    [Fact]
    public async Task RegistrationAsync_WhenExceptionThrown_ReturnsFailureError()
    {
        // Arrange
        var request = new RegistrationRequest 
        { 
            UserName = "newuser", 
            Password = "Password123!" 
        };

        var exceptionMessage = "Database connection failed";

        _mockUserManager
            .Setup(um => um.FindByNameAsync(request.UserName))
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act
        var result = await _userService.RegistrationAsync(request);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Description.Should().Contain(exceptionMessage);
    }

    #endregion

    #region GetAsync(int id) - Happy Path

    [Fact]
    public async Task GetAsync_WithValidUserId_ReturnsUser()
    {
        // Arrange
        var userId = 1;
        var expectedUser = TestDataFactory.CreateUser(userId);

        _mockCurrentUserProvider
            .Setup(cup => cup.UserId)
            .Returns(userId);

        _mockUserRepository
            .Setup(ur => ur.GetAsync(userId, true))
            .ReturnsAsync(expectedUser);

        // Act
        var result = await _userService.GetAsync(userId);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Id.Should().Be(userId);
    }

    [Fact]
    public async Task GetAsync_WithDifferentUserId_FetchesAsNonCurrentUser()
    {
        // Arrange
        var currentUserId = 1;
        var requestedUserId = 2;
        var expectedUser = TestDataFactory.CreateUser(requestedUserId);

        _mockCurrentUserProvider
            .Setup(cup => cup.UserId)
            .Returns(currentUserId);

        _mockUserRepository
            .Setup(ur => ur.GetAsync(requestedUserId, false))
            .ReturnsAsync(expectedUser);

        // Act
        var result = await _userService.GetAsync(requestedUserId);

        // Assert
        _mockUserRepository.Verify(ur => ur.GetAsync(requestedUserId, false), Times.Once);
    }

    #endregion

    #region GetAsync(int id) - Error Cases

    [Fact]
    public async Task GetAsync_WithNonexistentUserId_ReturnsSuccessWithNullValue()
    {
        // Arrange
        var userId = 999;

        _mockCurrentUserProvider
            .Setup(cup => cup.UserId)
            .Returns(1);

        _mockUserRepository
            .Setup(ur => ur.GetAsync(userId, It.IsAny<bool>()))
            .ReturnsAsync((User)null!);

        // Act
        var result = await _userService.GetAsync(userId);

        // Assert
        // ErrorOr automatically wraps nullable results
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeNull();
    }

    #endregion

    #region GetAsync(UserParams) - Happy Path

    [Fact]
    public async Task GetAsync_WithFilterOptions_ReturnsPageList()
    {
        // Arrange
        var currentUserId = 1;
        var filterOptions = new UserParams { PageNumber = 1, PageSize = 10 };
        var users = TestDataFactory.CreateUserList(5);
        var pageList = new PageList<User>(users, users.Count, 1, 10);

        _mockCurrentUserProvider
            .Setup(cup => cup.UserId)
            .Returns(currentUserId);

        _mockUserRepository
            .Setup(ur => ur.GetAsync(It.IsAny<UserParams>()))
            .ReturnsAsync(pageList);

        // Act
        var result = await _userService.GetAsync(filterOptions);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(5);
        filterOptions.UserId.Should().Be(currentUserId);
    }

    #endregion

    #region UpdateAsync - Happy Path

    [Fact]
    public async Task UpdateAsync_WithValidData_UpdatesUserSuccessfully()
    {
        // Arrange
        var userId = 1;
        var user = TestDataFactory.CreateUser(userId);
        var updateRequest = new UserUpdateRequest 
        { 
            Introduction = "Updated Introduction",
            LookingFor = "Looking for something",
            Interests = "Testing",
            City = "Test City",
            Country = "Test Country"
        };

        _mockUserRepository
            .Setup(ur => ur.GetAsync(userId, true))
            .ReturnsAsync(user);

        _mockUserRepository
            .Setup(ur => ur.SaveAllAsync())
            .ReturnsAsync(true);

        // Act
        var result = await _userService.UpdateAsync(userId, updateRequest);

        // Assert
        result.IsError.Should().BeFalse();
        _mockUserRepository.Verify(ur => ur.SaveAllAsync(), Times.Once);
    }

    #endregion

    #region UpdateAsync - Error Cases

    [Fact]
    public async Task UpdateAsync_WithNonexistentUser_ReturnsFailureError()
    {
        // Arrange
        var userId = 999;
        var updateRequest = new UserUpdateRequest { City = "Updated" };

        _mockUserRepository
            .Setup(ur => ur.GetAsync(userId, true))
            .ReturnsAsync((User)null!);

        // Act
        var result = await _userService.UpdateAsync(userId, updateRequest);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Description.Should().Contain("User not found");
    }

    [Fact]
    public async Task UpdateAsync_WhenSaveFails_ReturnsFailureError()
    {
        // Arrange
        var userId = 1;
        var user = TestDataFactory.CreateUser(userId);
        var updateRequest = new UserUpdateRequest { City = "Updated" };

        _mockUserRepository
            .Setup(ur => ur.GetAsync(userId, true))
            .ReturnsAsync(user);

        _mockUserRepository
            .Setup(ur => ur.SaveAllAsync())
            .ReturnsAsync(false);

        // Act
        var result = await _userService.UpdateAsync(userId, updateRequest);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Description.Should().Contain("Couldn't update user");
    }

    #endregion

    #region BookmarkAsync - Happy Path

    [Fact]
    public async Task BookmarkAsync_WithValidUsers_CreatesBookmarkSuccessfully()
    {
        // Arrange
        var userId = 1;
        var recipientId = 2;

        _mockUserRepository
            .Setup(ur => ur.GetBookmarkAsync(userId, recipientId))
            .ReturnsAsync((Bookmark)null!);

        _mockUserRepository
            .Setup(ur => ur.GetAsync(recipientId, false))
            .ReturnsAsync(TestDataFactory.CreateUser(recipientId));

        _mockUserRepository
            .Setup(ur => ur.AddAsync(It.IsAny<Bookmark>()))
            .Returns(Task.CompletedTask);

        _mockUserRepository
            .Setup(ur => ur.SaveAllAsync())
            .ReturnsAsync(true);

        // Act
        var result = await _userService.BookmakAsync(userId, recipientId);

        // Assert
        result.IsError.Should().BeFalse();
        _mockUserRepository.Verify(ur => ur.AddAsync(It.IsAny<Bookmark>()), Times.Once);
    }

    #endregion

    #region BookmarkAsync - Error Cases

    [Fact]
    public async Task BookmarkAsync_WhenAlreadyBookmarked_ReturnsConflictError()
    {
        // Arrange
        var userId = 1;
        var recipientId = 2;
        var existingBookmark = new Bookmark { BookmarkerId = userId, BookmarkedId = recipientId };

        _mockUserRepository
            .Setup(ur => ur.GetBookmarkAsync(userId, recipientId))
            .ReturnsAsync(existingBookmark);

        // Act
        var result = await _userService.BookmakAsync(userId, recipientId);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Description.Should().Contain("already bookmark");
    }

    [Fact]
    public async Task BookmarkAsync_WithNonexistentRecipient_ReturnsValidationError()
    {
        // Arrange
        var userId = 1;
        var recipientId = 999;

        _mockUserRepository
            .Setup(ur => ur.GetBookmarkAsync(userId, recipientId))
            .ReturnsAsync((Bookmark)null!);

        _mockUserRepository
            .Setup(ur => ur.GetAsync(recipientId, false))
            .ReturnsAsync((User)null!);

        // Act
        var result = await _userService.BookmakAsync(userId, recipientId);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Description.Should().Contain("valid user");
    }

    [Fact]
    public async Task BookmarkAsync_WhenSaveFails_ReturnsValidationError()
    {
        // Arrange
        var userId = 1;
        var recipientId = 2;

        _mockUserRepository
            .Setup(ur => ur.GetBookmarkAsync(userId, recipientId))
            .ReturnsAsync((Bookmark)null!);

        _mockUserRepository
            .Setup(ur => ur.GetAsync(recipientId, false))
            .ReturnsAsync(TestDataFactory.CreateUser(recipientId));

        _mockUserRepository
            .Setup(ur => ur.AddAsync(It.IsAny<Bookmark>()))
            .Returns(Task.CompletedTask);

        _mockUserRepository
            .Setup(ur => ur.SaveAllAsync())
            .ReturnsAsync(false);

        // Act
        var result = await _userService.BookmakAsync(userId, recipientId);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Description.Should().Contain("Unable to perform the operation");
    }

    #endregion

    #region EditRolesAsync - Happy Path

    [Fact]
    public async Task EditRolesAsync_WithValidRoles_UpdatesUserRolesSuccessfully()
    {
        // Arrange
        var userName = "testuser";
        var user = TestDataFactory.CreateUser(1, userName);
        var request = new EditRoleRequest { RoleName = new[] { "Member", "Admin" } };
        var currentRoles = new List<string> { "Member" };
        var newRoles = new List<string> { "Member", "Admin" };

        _mockUserManager
            .Setup(um => um.FindByNameAsync(userName))
            .ReturnsAsync(user);

        _mockUserManager
            .Setup(um => um.GetRolesAsync(user))
            .ReturnsAsync(currentRoles);

        _mockUserManager
            .Setup(um => um.AddToRolesAsync(user, It.IsAny<IEnumerable<string>>()))
            .ReturnsAsync(IdentityResult.Success);

        _mockUserManager
            .Setup(um => um.RemoveFromRolesAsync(user, It.IsAny<IEnumerable<string>>()))
            .ReturnsAsync(IdentityResult.Success);

        var finalRolesSetup = _mockUserManager
            .Setup(um => um.GetRolesAsync(user));

        finalRolesSetup.ReturnsAsync(newRoles);

        // Act
        var result = await _userService.EditRolesAsync(userName, request);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().Contain("Admin");
    }

    #endregion

    #region EditRolesAsync - Error Cases

    [Fact]
    public async Task EditRolesAsync_WithNonexistentUser_ReturnsFailureError()
    {
        // Arrange
        var userName = "nonexistent";
        var request = new EditRoleRequest { RoleName = new[] { "Admin" } };

        _mockUserManager
            .Setup(um => um.FindByNameAsync(userName))
            .ReturnsAsync((User)null!);

        // Act
        var result = await _userService.EditRolesAsync(userName, request);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Description.Should().Contain("User not found");
    }

    [Fact]
    public async Task EditRolesAsync_WhenAddToRolesFails_ReturnsFailureError()
    {
        // Arrange
        var userName = "testuser";
        var user = TestDataFactory.CreateUser(1, userName);
        var request = new EditRoleRequest { RoleName = new[] { "Admin" } };

        _mockUserManager
            .Setup(um => um.FindByNameAsync(userName))
            .ReturnsAsync(user);

        _mockUserManager
            .Setup(um => um.GetRolesAsync(user))
            .ReturnsAsync(new List<string>());

        var addError = new IdentityError { Description = "Failed to add role" };
        _mockUserManager
            .Setup(um => um.AddToRolesAsync(user, It.IsAny<IEnumerable<string>>()))
            .ReturnsAsync(IdentityResult.Failed(addError));

        // Act
        var result = await _userService.EditRolesAsync(userName, request);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Description.Should().Contain("Failed to add to roles");
    }

    #endregion

    #region GetUsersWithRolesAsync

    [Fact]
    public async Task GetUsersWithRolesAsync_ReturnsUsersWithRoles()
    {
        // Arrange
        var usersWithRoles = new List<object> { new { UserId = 1, UserName = "user1" } };

        _mockUserRepository
            .Setup(ur => ur.GetUsersWithRolesAsync())
            .ReturnsAsync(usersWithRoles);

        // Act
        var result = await _userService.GetUsersWithRolesAsync();

        // Assert
        result.Should().NotBeEmpty();
        _mockUserRepository.Verify(ur => ur.GetUsersWithRolesAsync(), Times.Once);
    }

    #endregion
}
