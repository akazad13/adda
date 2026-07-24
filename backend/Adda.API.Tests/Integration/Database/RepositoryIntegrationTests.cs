using Adda.API.Data;
using Adda.API.Helpers;
using Adda.API.Models;
using Adda.API.Repositories.MessageRepository;
using Adda.API.Repositories.PhotoRepository;
using Adda.API.Repositories.UserRepository;
using Adda.API.Tests.Fixtures;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Xunit;

namespace Adda.API.Tests.Integration.Database;

/// <summary>
/// Base class for database integration tests using in-memory database
/// </summary>
public abstract class DatabaseIntegrationTestBase : IAsyncLifetime
{
    protected readonly DbContextOptions<DataContext> _options;
    protected DataContext _context = null!;

    protected DatabaseIntegrationTestBase()
    {
        _options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    public virtual async Task InitializeAsync()
    {
        _context = new DataContext(_options);
        await _context.Database.EnsureCreatedAsync();
    }

    public virtual async Task DisposeAsync()
    {
        await _context.Database.EnsureDeletedAsync();
        await _context.DisposeAsync();
    }

    protected async Task SeedDataAsync(params object[] entities)
    {
        foreach (var entity in entities)
        {
            _context.Add(entity);
        }
        await _context.SaveChangesAsync();
    }
}

/// <summary>
/// Integration tests for UserRepository using in-memory database
/// </summary>
public class UserRepositoryIntegrationTests : DatabaseIntegrationTestBase
{
    private IUserRepository _userRepository = null!;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _userRepository = new UserRepository(_context);
    }

    #region GetAsync - Happy Path

    [Fact]
    public async Task GetAsync_WithValidId_ReturnsUser()
    {
        // Arrange
        var user = TestDataFactory.CreateUser(1, "testuser");
        await SeedDataAsync(user);

        // Act
        var result = await _userRepository.GetAsync(1, false);

        // Assert
        result.Should().NotBeNull();
        result.UserName.Should().Be("testuser");
    }

    [Fact]
    public async Task GetAsync_WhenCurrentUser_IncludesAllData()
    {
        // Arrange
        var user = TestDataFactory.CreateUser(1, "testuser");
        user.Photos = new List<Photo> { TestDataFactory.CreatePhoto(1, 1) };
        await SeedDataAsync(user);

        // Act
        var result = await _userRepository.GetAsync(1, true);

        // Assert
        result.Photos.Should().NotBeEmpty();
    }

    #endregion

    #region GetAsync - Error Cases

    [Fact]
    public async Task GetAsync_WithNonexistentId_ReturnsNull()
    {
        // Act
        var result = await _userRepository.GetAsync(999, false);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region GetAsync(UserParams)

    [Fact]
    public async Task GetAsync_WithFilterParams_ReturnsPaginatedUsers()
    {
        // Arrange
        var users = TestDataFactory.CreateUserList(10);
        await SeedDataAsync(users.ToArray());

        var filterParams = new UserParams { UserId = 1, PageNumber = 1, PageSize = 5 };

        // Act
        var result = await _userRepository.GetAsync(filterParams);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(5);
    }

    [Fact]
    public async Task GetAsync_WithMultiplePages_ReturnCorrectPage()
    {
        // Arrange
        var users = TestDataFactory.CreateUserList(15);
        await SeedDataAsync(users.ToArray());

        var filterParams = new UserParams { UserId = 1, PageNumber = 2, PageSize = 5 };

        // Act
        var result = await _userRepository.GetAsync(filterParams);

        // Assert
        result.Count.Should().Be(5);
    }

    #endregion

    #region GetUsersWithRolesAsync

    [Fact]
    public async Task GetUsersWithRolesAsync_ReturnsUsersWithRoles()
    {
        // Arrange
        var user = TestDataFactory.CreateUser(1, "testuser");
        await SeedDataAsync(user);

        // Act
        var result = await _userRepository.GetUsersWithRolesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
    }

    #endregion

    #region GetBookmarkAsync

    [Fact]
    public async Task GetBookmarkAsync_WithExistingBookmark_ReturnsBookmark()
    {
        // Arrange
        var user1 = TestDataFactory.CreateUser(1, "user1");
        var user2 = TestDataFactory.CreateUser(2, "user2");
        var bookmark = new Bookmark { BookmarkerId = 1, BookmarkedId = 2 };

        await SeedDataAsync(user1, user2, bookmark);

        // Act
        var result = await _userRepository.GetBookmarkAsync(1, 2);

        // Assert
        result.Should().NotBeNull();
        result.BookmarkerId.Should().Be(1);
    }

    [Fact]
    public async Task GetBookmarkAsync_WithNonexistentBookmark_ReturnsNull()
    {
        // Act
        var result = await _userRepository.GetBookmarkAsync(1, 2);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region AddAsync and SaveAllAsync

    [Fact]
    public async Task AddAsync_WithNewBookmark_SavesSuccessfully()
    {
        // Arrange
        var user1 = TestDataFactory.CreateUser(1, "user1");
        var user2 = TestDataFactory.CreateUser(2, "user2");
        await SeedDataAsync(user1, user2);

        var bookmark = new Bookmark { BookmarkerId = 1, BookmarkedId = 2 };

        // Act
        await _userRepository.AddAsync(bookmark);
        var saved = await _userRepository.SaveAllAsync();

        // Assert
        saved.Should().BeTrue();
        var result = await _userRepository.GetBookmarkAsync(1, 2);
        result.Should().NotBeNull();
    }

    #endregion
}

/// <summary>
/// Integration tests for MessageRepository using in-memory database
/// </summary>
public class MessageRepositoryIntegrationTests : DatabaseIntegrationTestBase
{
    private IMessageRepository _messageRepository = null!;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _messageRepository = new MessageRepository(_context);
    }

    #region GetMessageAsync

    [Fact]
    public async Task GetMessageAsync_WithValidId_ReturnsMessage()
    {
        // Arrange
        var user1 = TestDataFactory.CreateUser(1, "user1");
        var user2 = TestDataFactory.CreateUser(2, "user2");
        var message = TestDataFactory.CreateMessage(1, 1, 2);

        await SeedDataAsync(user1, user2, message);

        // Act
        var result = await _messageRepository.GetMessageAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Content.Should().Be(message.Content);
    }

    [Fact]
    public async Task GetMessageAsync_WithNonexistentId_ReturnsNull()
    {
        // Act
        var result = await _messageRepository.GetMessageAsync(999);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region GetMessagesForUserAsync

    [Fact]
    public async Task GetMessagesForUserAsync_WithValidParams_ReturnsPaginatedMessages()
    {
        // Arrange
        var user1 = TestDataFactory.CreateUser(1, "user1");
        var user2 = TestDataFactory.CreateUser(2, "user2");
        var messages = TestDataFactory.CreateMessageList(10, 1, 2);

        await SeedDataAsync(new object[] { user1, user2 }.Concat(messages).ToArray());

        // user1 is the sender (SenderId=1), so use Outbox to retrieve their sent messages
        var messageParams = new MessageParams { UserId = 1, PageNumber = 1, PageSize = 5, MessageContainer = "Outbox" };

        // Act
        var result = await _messageRepository.GetMessagesForUserAsync(messageParams);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(5);
    }

    [Fact]
    public async Task GetMessagesForUserAsync_WithNoMessages_ReturnsEmptyList()
    {
        // Arrange
        var messageParams = new MessageParams { UserId = 999, PageNumber = 1, PageSize = 10 };

        // Act
        var result = await _messageRepository.GetMessagesForUserAsync(messageParams);

        // Assert
        result.Count.Should().Be(0);
    }

    #endregion

    #region GetMessageThreadAsync

    [Fact]
    public async Task GetMessageThreadAsync_WithValidUserIds_ReturnsThread()
    {
        // Arrange
        var user1 = TestDataFactory.CreateUser(1, "user1");
        var user2 = TestDataFactory.CreateUser(2, "user2");
        var messages = new List<Message>
        {
            TestDataFactory.CreateMessage(1, 1, 2),
            TestDataFactory.CreateMessage(2, 2, 1),
            TestDataFactory.CreateMessage(3, 1, 2)
        };

        await SeedDataAsync(new object[] { user1, user2 }.Concat(messages).ToArray());

        // Act
        var result = await _messageRepository.GetMessageThreadAsync(1, 2);

        // Assert
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetMessageThreadAsync_WithNoMessages_ReturnsEmptyList()
    {
        // Act
        var result = await _messageRepository.GetMessageThreadAsync(1, 2);

        // Assert
        result.Should().BeEmpty();
    }

    #endregion

    #region SaveAllAsync

    [Fact]
    public async Task SaveAllAsync_AfterModifyingMessage_SavesChanges()
    {
        // Arrange
        var user1 = TestDataFactory.CreateUser(1, "user1");
        var user2 = TestDataFactory.CreateUser(2, "user2");
        var message = TestDataFactory.CreateMessage(1, 1, 2);

        await SeedDataAsync(user1, user2, message);

        var retrievedMessage = await _messageRepository.GetMessageAsync(1);
        retrievedMessage.IsRead = true;

        // Act
        var saved = await _messageRepository.SaveAllAsync();

        // Assert
        saved.Should().BeTrue();

        var verifyMessage = await _messageRepository.GetMessageAsync(1);
        verifyMessage.IsRead.Should().BeTrue();
    }

    #endregion

    #region Delete

    [Fact]
    public async Task Delete_RemovesMessageFromDatabase()
    {
        // Arrange
        var user1 = TestDataFactory.CreateUser(1, "user1");
        var user2 = TestDataFactory.CreateUser(2, "user2");
        var message = TestDataFactory.CreateMessage(1, 1, 2);

        await SeedDataAsync(user1, user2, message);

        // Act
        var messageToDelete = await _messageRepository.GetMessageAsync(1);
        _messageRepository.Delete(messageToDelete);
        await _messageRepository.SaveAllAsync();

        // Assert
        var result = await _messageRepository.GetMessageAsync(1);
        result.Should().BeNull();
    }

    #endregion
}

/// <summary>
/// Integration tests for PhotoRepository using in-memory database
/// </summary>
public class PhotoRepositoryIntegrationTests : DatabaseIntegrationTestBase
{
    private IPhotoRepository _photoRepository = null!;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _photoRepository = new PhotoRepository(_context);
    }

    #region GetPhotoAsync

    [Fact]
    public async Task GetPhotoAsync_WithValidId_ReturnsPhoto()
    {
        // Arrange
        var user = TestDataFactory.CreateUser(1, "testuser");
        var photo = TestDataFactory.CreatePhoto(1, 1);
        user.Photos = new List<Photo> { photo };

        await SeedDataAsync(user);

        // Act
        var result = await _photoRepository.GetPhotoAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Url.Should().Be(photo.Url);
    }

    [Fact]
    public async Task GetPhotoAsync_WithNonexistentId_ReturnsNull()
    {
        // Act
        var result = await _photoRepository.GetPhotoAsync(999);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region GetAsync

    [Fact]
    public async Task GetAsync_WithValidId_ReturnsPhoto()
    {
        // Arrange
        var user = TestDataFactory.CreateUser(1, "testuser");
        var photo = TestDataFactory.CreatePhoto(1, 1);
        user.Photos = new List<Photo> { photo };

        await SeedDataAsync(user);

        // Act
        var result = await _photoRepository.GetAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
    }

    #endregion

    #region SaveAllAsync

    [Fact]
    public async Task SaveAllAsync_AfterModifyingPhoto_SavesChanges()
    {
        // Arrange
        var user = TestDataFactory.CreateUser(1, "testuser");
        // Seed photo with IsApproved=false so toggling to true is an actual change
        var photo = new PhotoBuilder().WithId(1).WithUserId(1).Build();
        photo.IsApproved = false;
        user.Photos = new List<Photo> { photo };

        await SeedDataAsync(user);

        var retrievedPhoto = await _photoRepository.GetPhotoAsync(1);
        retrievedPhoto.IsApproved = true;

        // Act
        var saved = await _photoRepository.SaveAllAsync();

        // Assert
        saved.Should().BeTrue();

        var verifyPhoto = await _photoRepository.GetPhotoAsync(1);
        verifyPhoto.IsApproved.Should().BeTrue();
    }

    #endregion

    #region Delete

    [Fact]
    public async Task Delete_RemovesPhotoFromDatabase()
    {
        // Arrange
        var user = TestDataFactory.CreateUser(1, "testuser");
        var photo = TestDataFactory.CreatePhoto(1, 1);
        user.Photos = new List<Photo> { photo };

        await SeedDataAsync(user);

        // Act
        var photoToDelete = await _photoRepository.GetPhotoAsync(1);
        _photoRepository.Delete(photoToDelete);
        await _photoRepository.SaveAllAsync();

        // Assert
        var result = await _photoRepository.GetPhotoAsync(1);
        result.Should().BeNull();
    }

    #endregion
}
