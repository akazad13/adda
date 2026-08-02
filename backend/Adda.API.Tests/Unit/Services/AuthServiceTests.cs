using Adda.API.Dtos;
using Adda.API.Models;
using Adda.API.Security.TokenGenerator;
using Adda.API.Services.AuthService;
using Adda.API.Tests.Fixtures;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace Adda.API.Tests.Unit.Services;

/// <summary>
/// Unit tests for AuthService
/// Note: LoginAsync tests that require complex EF Core Include() + SingleOrDefaultAsync() mocking
/// are covered by integration tests in ControllerIntegrationTests.cs instead.
/// Unit tests here focus on simpler scenarios that can be properly mocked.
/// </summary>
public class AuthServiceTests
{
    private readonly Mock<UserManager<User>> _mockUserManager;
    private readonly Mock<SignInManager<User>> _mockSignInManager;
    private readonly Mock<IJwtTokenGenerator> _mockJwtTokenGenerator;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        // Setup UserManager mock
        var userStore = new Mock<IUserStore<User>>();
        _mockUserManager = new Mock<UserManager<User>>(
            userStore.Object, null, null, null, null, null, null, null, null
        );

        // Setup SignInManager mock
        var contextAccessor = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
        var userClaimsPrincipalFactory = new Mock<Microsoft.AspNetCore.Identity.IUserClaimsPrincipalFactory<User>>();
        var options = new Mock<Microsoft.Extensions.Options.IOptions<Microsoft.AspNetCore.Identity.IdentityOptions>>();
        var logger = new Mock<Microsoft.Extensions.Logging.ILogger<SignInManager<User>>>();

        _mockSignInManager = new Mock<SignInManager<User>>(
            _mockUserManager.Object,
            contextAccessor.Object,
            userClaimsPrincipalFactory.Object,
            options.Object,
            logger.Object,
            null,
            null
        );

        _mockJwtTokenGenerator = new Mock<IJwtTokenGenerator>();

        _authService = new AuthService(_mockJwtTokenGenerator.Object, _mockUserManager.Object, _mockSignInManager.Object);
    }

    #region LoginAsync - Error Cases

    [Fact]
    public async Task LoginAsync_WithNonexistentUser_ReturnsValidationError()
    {
        // Arrange
        var request = new AuthRequest { UserName = "nonexistent", Password = "password123" };

        _mockUserManager
            .Setup(um => um.FindByNameAsync(request.UserName))
            .ReturnsAsync((User)null!);

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Description.Should().Contain("Invalid userName or password");
    }

    [Fact]
    public async Task LoginAsync_WithIncorrectPassword_ReturnsValidationError()
    {
        // Arrange
        var user = TestDataFactory.CreateUser(1, "testuser");
        var request = new AuthRequest { UserName = "testuser", Password = "wrongpassword" };

        _mockUserManager
            .Setup(um => um.FindByNameAsync(request.UserName))
            .ReturnsAsync(user);

        _mockSignInManager
            .Setup(sm => sm.CheckPasswordSignInAsync(user, request.Password, false))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Description.Should().Contain("Invalid userName or password");
    }

    #endregion
}
