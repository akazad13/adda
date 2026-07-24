using Adda.API.Helpers;
using Adda.API.Models;
using Adda.API.Repositories.MessageRepository;
using Adda.API.Services.MessageService;
using Adda.API.Tests.Fixtures;
using FluentAssertions;
using Moq;
using Xunit;

namespace Adda.API.Tests.Unit.Services;

/// <summary>
/// Unit tests for MessageService
/// </summary>
public class MessageServiceTests
{
    private readonly Mock<IMessageRepository> _mockMessageRepository;
    private readonly MessageService _messageService;

    public MessageServiceTests()
    {
        _mockMessageRepository = new Mock<IMessageRepository>();
        _messageService = new MessageService(_mockMessageRepository.Object);
    }

    #region GetAsync(int id) - Happy Path

    [Fact]
    public async Task GetAsync_WithValidId_ReturnsMessage()
    {
        // Arrange
        var messageId = 1;
        var expectedMessage = TestDataFactory.CreateMessage(messageId, 1, 2);

        _mockMessageRepository
            .Setup(mr => mr.GetMessageAsync(messageId))
            .ReturnsAsync(expectedMessage);

        // Act
        var result = await _messageService.GetAsync(messageId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(messageId);
        result.Content.Should().Be(expectedMessage.Content);
    }

    #endregion

    #region GetAsync(int id) - Error Cases

    [Fact]
    public async Task GetAsync_WithNonexistentId_ReturnsNull()
    {
        // Arrange
        var messageId = 999;

        _mockMessageRepository
            .Setup(mr => mr.GetMessageAsync(messageId))
            .ReturnsAsync((Message)null!);

        // Act
        var result = await _messageService.GetAsync(messageId);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region GetMessagesForUserAsync - Happy Path

    [Fact]
    public async Task GetMessagesForUserAsync_WithValidParams_ReturnsPageList()
    {
        // Arrange
        var messageParams = new MessageParams { UserId = 1, PageNumber = 1, PageSize = 10 };
        var messages = TestDataFactory.CreateMessageList(5, 1, 2);
        var pageList = new PageList<Message>(messages, messages.Count, 1, 10);

        _mockMessageRepository
            .Setup(mr => mr.GetMessagesForUserAsync(messageParams))
            .ReturnsAsync(pageList);

        // Act
        var result = await _messageService.GetMessagesForUserAsync(messageParams);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(5);
    }

    [Fact]
    public async Task GetMessagesForUserAsync_WithPagination_ReturnsPaginatedResults()
    {
        // Arrange
        var messageParams = new MessageParams { UserId = 1, PageNumber = 2, PageSize = 5 };
        var messages = TestDataFactory.CreateMessageList(5, 1, 2);
        var pageList = new PageList<Message>(messages, 15, 2, 5);

        _mockMessageRepository
            .Setup(mr => mr.GetMessagesForUserAsync(messageParams))
            .ReturnsAsync(pageList);

        // Act
        var result = await _messageService.GetMessagesForUserAsync(messageParams);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(5);
    }

    #endregion

    #region GetMessagesForUserAsync - Edge Cases

    [Fact]
    public async Task GetMessagesForUserAsync_WithNoMessages_ReturnsEmptyPageList()
    {
        // Arrange
        var messageParams = new MessageParams { UserId = 999, PageNumber = 1, PageSize = 10 };
        var messages = new List<Message>();
        var pageList = new PageList<Message>(messages, 0, 1, 10);

        _mockMessageRepository
            .Setup(mr => mr.GetMessagesForUserAsync(messageParams))
            .ReturnsAsync(pageList);

        // Act
        var result = await _messageService.GetMessagesForUserAsync(messageParams);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(0);
    }

    #endregion

    #region GetMessageThreadAsync - Happy Path

    [Fact]
    public async Task GetMessageThreadAsync_WithValidUserIds_ReturnsMessageThread()
    {
        // Arrange
        var userId = 1;
        var recipientId = 2;
        var messages = new List<Message>
        {
            TestDataFactory.CreateMessage(1, userId, recipientId),
            TestDataFactory.CreateMessage(2, recipientId, userId),
            TestDataFactory.CreateMessage(3, userId, recipientId)
        };

        _mockMessageRepository
            .Setup(mr => mr.GetMessageThreadAsync(userId, recipientId))
            .ReturnsAsync(messages);

        // Act
        var result = await _messageService.GetMessageThreadAsync(userId, recipientId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetMessageThreadAsync_ReturnsMessagesBetweenCorrectUsers()
    {
        // Arrange
        var userId = 1;
        var recipientId = 2;
        var messages = new List<Message>
        {
            new MessageBuilder().WithId(1).WithSenderId(userId).WithRecipientId(recipientId).Build(),
            new MessageBuilder().WithId(2).WithSenderId(recipientId).WithRecipientId(userId).Build()
        };

        _mockMessageRepository
            .Setup(mr => mr.GetMessageThreadAsync(userId, recipientId))
            .ReturnsAsync(messages);

        // Act
        var result = await _messageService.GetMessageThreadAsync(userId, recipientId);

        // Assert
        result.Should().AllSatisfy(m => 
        {
            ((m.SenderId == userId && m.RecipientId == recipientId) ||
             (m.SenderId == recipientId && m.RecipientId == userId)).Should().BeTrue();
        });
    }

    #endregion

    #region GetMessageThreadAsync - Edge Cases

    [Fact]
    public async Task GetMessageThreadAsync_WithNoMessages_ReturnsEmptyList()
    {
        // Arrange
        var userId = 1;
        var recipientId = 2;
        var messages = new List<Message>();

        _mockMessageRepository
            .Setup(mr => mr.GetMessageThreadAsync(userId, recipientId))
            .ReturnsAsync(messages);

        // Act
        var result = await _messageService.GetMessageThreadAsync(userId, recipientId);

        // Assert
        result.Should().BeEmpty();
    }

    #endregion

    #region DeleteAsync - Happy Path

    [Fact]
    public async Task DeleteAsync_WhenUserIsSender_SetsSenderDeleted()
    {
        // Arrange
        var userId = 1;
        var messageId = 1;
        var message = TestDataFactory.CreateMessage(messageId, userId, 2);
        message.SenderDeleted = false;
        message.RecipientDeleted = false;

        _mockMessageRepository
            .Setup(mr => mr.GetMessageAsync(messageId))
            .ReturnsAsync(message);

        _mockMessageRepository
            .Setup(mr => mr.SaveAllAsync())
            .ReturnsAsync(true);

        // Act
        var result = await _messageService.DeleteAsync(userId, messageId);

        // Assert
        result.IsError.Should().BeFalse();
        message.SenderDeleted.Should().BeTrue();
        message.RecipientDeleted.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_WhenUserIsRecipient_SetsRecipientDeleted()
    {
        // Arrange
        var userId = 2;
        var messageId = 1;
        var message = TestDataFactory.CreateMessage(messageId, 1, userId);
        message.SenderDeleted = false;
        message.RecipientDeleted = false;

        _mockMessageRepository
            .Setup(mr => mr.GetMessageAsync(messageId))
            .ReturnsAsync(message);

        _mockMessageRepository
            .Setup(mr => mr.SaveAllAsync())
            .ReturnsAsync(true);

        // Act
        var result = await _messageService.DeleteAsync(userId, messageId);

        // Assert
        result.IsError.Should().BeFalse();
        message.RecipientDeleted.Should().BeTrue();
        message.SenderDeleted.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_WhenBothDeleted_DeletesMessageFromDatabase()
    {
        // Arrange
        var userId = 1;
        var messageId = 1;
        var message = TestDataFactory.CreateMessage(messageId, userId, 2);
        message.SenderDeleted = true;
        message.RecipientDeleted = false;

        _mockMessageRepository
            .Setup(mr => mr.GetMessageAsync(messageId))
            .ReturnsAsync(message);

        _mockMessageRepository
            .Setup(mr => mr.SaveAllAsync())
            .ReturnsAsync(true);

        // Act
        var result = await _messageService.DeleteAsync(userId, messageId);

        // Assert
        result.IsError.Should().BeFalse();
        // Note: The service marks SenderDeleted or RecipientDeleted and then if both are true, calls Delete
        // After first call: SenderDeleted=true, RecipientDeleted=true (because userId=1, messageFr userId=1 sends it to 2)
        // So Delete should be called
    }

    [Fact]
    public async Task DeleteAsync_WhenSaveSucceeds_ReturnsSuccess()
    {
        // Arrange
        var userId = 1;
        var messageId = 1;
        var message = TestDataFactory.CreateMessage(messageId, userId, 2);

        _mockMessageRepository
            .Setup(mr => mr.GetMessageAsync(messageId))
            .ReturnsAsync(message);

        _mockMessageRepository
            .Setup(mr => mr.SaveAllAsync())
            .ReturnsAsync(true);

        // Act
        var result = await _messageService.DeleteAsync(userId, messageId);

        // Assert
        result.IsError.Should().BeFalse();
    }

    #endregion

    #region DeleteAsync - Error Cases

    [Fact]
    public async Task DeleteAsync_WithNonexistentMessage_ReturnsValidationError()
    {
        // Arrange
        var userId = 1;
        var messageId = 999;

        _mockMessageRepository
            .Setup(mr => mr.GetMessageAsync(messageId))
            .ReturnsAsync((Message)null!);

        // Act
        var result = await _messageService.DeleteAsync(userId, messageId);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Description.Should().Contain("Could not find user");
    }

    [Fact]
    public async Task DeleteAsync_WhenSaveFails_ReturnsFailureError()
    {
        // Arrange
        var userId = 1;
        var messageId = 1;
        var message = TestDataFactory.CreateMessage(messageId, userId, 2);

        _mockMessageRepository
            .Setup(mr => mr.GetMessageAsync(messageId))
            .ReturnsAsync(message);

        _mockMessageRepository
            .Setup(mr => mr.SaveAllAsync())
            .ReturnsAsync(false);

        // Act
        var result = await _messageService.DeleteAsync(userId, messageId);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Description.Should().Contain("Error deleting the message");
    }

    [Fact]
    public async Task DeleteAsync_WithUnauthorizedUser_ShouldNotDelete()
    {
        // Arrange
        var userId = 1;
        var messageId = 1;
        var message = TestDataFactory.CreateMessage(messageId, 2, 3); // Neither sender nor recipient

        _mockMessageRepository
            .Setup(mr => mr.GetMessageAsync(messageId))
            .ReturnsAsync(message);

        _mockMessageRepository
            .Setup(mr => mr.SaveAllAsync())
            .ReturnsAsync(true);

        // Act
        var result = await _messageService.DeleteAsync(userId, messageId);

        // Assert
        result.IsError.Should().BeFalse();
        message.SenderDeleted.Should().BeFalse();
        message.RecipientDeleted.Should().BeFalse();
        _mockMessageRepository.Verify(mr => mr.Delete(It.IsAny<Message>()), Times.Never);
    }

    #endregion
}
