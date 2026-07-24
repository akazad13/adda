using Adda.API.ExternalServices.Cloudinary;
using Adda.API.Models;
using Adda.API.Repositories.PhotoRepository;
using Adda.API.Repositories.UserRepository;
using Adda.API.Services.PhotoService;
using Adda.API.Tests.Fixtures;
using ErrorOr;
using FluentAssertions;
using Moq;
using Xunit;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Http;

namespace Adda.API.Tests.Unit.Services;

/// <summary>
/// Unit tests for PhotoService
/// </summary>
public class PhotoServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IPhotoRepository> _mockPhotoRepository;
    private readonly Mock<ICloudinaryService> _mockCloudinaryService;
    private readonly PhotoService _photoService;

    public PhotoServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockPhotoRepository = new Mock<IPhotoRepository>();
        _mockCloudinaryService = new Mock<ICloudinaryService>();

        _photoService = new PhotoService(
            _mockUserRepository.Object,
            _mockPhotoRepository.Object,
            _mockCloudinaryService.Object);
    }

    #region AddAsync - Happy Path

    [Fact]
    public async Task AddAsync_WithValidFile_AddsPhotoSuccessfully()
    {
        // Arrange
        var userId = 1;
        var user = TestDataFactory.CreateUser(userId);
        user.Photos = new List<Photo>();

        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.Length).Returns(1024);
        fileMock.Setup(f => f.FileName).Returns("test.jpg");

        var uploadResponse = new PhotoUploadResult(
            "https://cloudinary.com/photo.jpg",
            "public_id_123");

        _mockUserRepository
            .Setup(ur => ur.GetAsync(userId, true))
            .ReturnsAsync(user);

        _mockCloudinaryService
            .Setup(cs => cs.UploadPhotoAsync(fileMock.Object))
            .ReturnsAsync(uploadResponse);

        _mockUserRepository
            .Setup(ur => ur.SaveAllAsync())
            .ReturnsAsync(true);

        // Act
        var result = await _photoService.AddAsync(userId, fileMock.Object);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Url.Should().Be(uploadResponse.Url);
        result.Value.PublicId.Should().Be(uploadResponse.PublicId);
        _mockUserRepository.Verify(ur => ur.SaveAllAsync(), Times.Once);
    }

    [Fact]
    public async Task AddAsync_WhenFirstPhoto_SetsAsMainPhoto()
    {
        // Arrange
        var userId = 1;
        var user = TestDataFactory.CreateUser(userId);
        user.Photos = new List<Photo>();

        var fileMock = new Mock<IFormFile>();

        var uploadResponse = new PhotoUploadResult(
            "https://cloudinary.com/photo.jpg",
            "public_id_123");

        _mockUserRepository
            .Setup(ur => ur.GetAsync(userId, true))
            .ReturnsAsync(user);

        _mockCloudinaryService
            .Setup(cs => cs.UploadPhotoAsync(It.IsAny<IFormFile>()))
            .ReturnsAsync(uploadResponse);

        _mockUserRepository
            .Setup(ur => ur.SaveAllAsync())
            .ReturnsAsync(true);

        // Act
        var result = await _photoService.AddAsync(userId, fileMock.Object);

        // Assert
        result.Value.IsMain.Should().BeTrue();
    }

    [Fact]
    public async Task AddAsync_WhenExistingMainPhoto_DoesNotSetNewAsMain()
    {
        // Arrange
        var userId = 1;
        var user = TestDataFactory.CreateUser(userId);
        var mainPhoto = TestDataFactory.CreatePhoto(1, userId);
        mainPhoto.IsMain = true;
        user.Photos = new List<Photo> { mainPhoto };

        var fileMock = new Mock<IFormFile>();

        var uploadResponse = new PhotoUploadResult(
            "https://cloudinary.com/photo2.jpg",
            "public_id_456");

        _mockUserRepository
            .Setup(ur => ur.GetAsync(userId, true))
            .ReturnsAsync(user);

        _mockCloudinaryService
            .Setup(cs => cs.UploadPhotoAsync(It.IsAny<IFormFile>()))
            .ReturnsAsync(uploadResponse);

        _mockUserRepository
            .Setup(ur => ur.SaveAllAsync())
            .ReturnsAsync(true);

        // Act
        var result = await _photoService.AddAsync(userId, fileMock.Object);

        // Assert
        result.Value.IsMain.Should().BeFalse();
    }

    #endregion

    #region AddAsync - Error Cases

    [Fact]
    public async Task AddAsync_WithNullFile_ReturnsFailureError()
    {
        // Arrange
        var userId = 1;

        // Act
        var result = await _photoService.AddAsync(userId, null!);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Description.Should().Contain("No file was uploaded");
    }

    [Fact]
    public async Task AddAsync_WithNonexistentUser_ReturnsFailureError()
    {
        // Arrange
        var userId = 999;
        var fileMock = new Mock<IFormFile>();

        _mockUserRepository
            .Setup(ur => ur.GetAsync(userId, true))
            .ReturnsAsync((User)null!);

        // Act
        var result = await _photoService.AddAsync(userId, fileMock.Object);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Description.Should().Contain("User not found");
    }

    [Fact]
    public async Task AddAsync_WhenCloudinaryUploadFails_ReturnsFailureError()
    {
        // Arrange
        var userId = 1;
        var user = TestDataFactory.CreateUser(userId);
        user.Photos = new List<Photo>();

        var fileMock = new Mock<IFormFile>();

        var uploadError = Error.Failure("Upload failed");

        _mockUserRepository
            .Setup(ur => ur.GetAsync(userId, true))
            .ReturnsAsync(user);

        _mockCloudinaryService
            .Setup(cs => cs.UploadPhotoAsync(fileMock.Object))
            .Throws(new Exception("Upload failed"));

        // Act
        var result = await _photoService.AddAsync(userId, fileMock.Object);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Description.Should().Contain("Upload failed");
    }

    [Fact]
    public async Task AddAsync_WhenSaveFails_ReturnsFailureError()
    {
        // Arrange
        var userId = 1;
        var user = TestDataFactory.CreateUser(userId);
        user.Photos = new List<Photo>();

        var fileMock = new Mock<IFormFile>();

        var uploadResponse = new PhotoUploadResult(
            "https://cloudinary.com/photo.jpg",
            "public_id_123");

        _mockUserRepository
            .Setup(ur => ur.GetAsync(userId, true))
            .ReturnsAsync(user);

        _mockCloudinaryService
            .Setup(cs => cs.UploadPhotoAsync(fileMock.Object))
            .ReturnsAsync(uploadResponse);

        _mockUserRepository
            .Setup(ur => ur.SaveAllAsync())
            .ReturnsAsync(false);

        // Act
        var result = await _photoService.AddAsync(userId, fileMock.Object);

        // Assert
        result.IsError.Should().BeTrue();
    }

    [Fact]
    public async Task AddAsync_WhenExceptionThrown_ReturnsFailureError()
    {
        // Arrange
        var userId = 1;
        var fileMock = new Mock<IFormFile>();
        var exceptionMessage = "Database error";

        _mockUserRepository
            .Setup(ur => ur.GetAsync(userId, true))
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act
        var result = await _photoService.AddAsync(userId, fileMock.Object);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Description.Should().Contain(exceptionMessage);
    }

    #endregion

    #region ApprovePhotoAsync

    [Fact]
    public async Task ApprovePhotoAsync_WithValidPhotoId_ApprovesPhoto()
    {
        // Arrange
        var photoId = 1;
        var photo = TestDataFactory.CreatePhoto(photoId, 1);
        photo.IsApproved = false;

        _mockPhotoRepository
            .Setup(pr => pr.GetPhotoAsync(photoId))
            .ReturnsAsync(photo);

        _mockPhotoRepository
            .Setup(pr => pr.SaveAllAsync())
            .ReturnsAsync(true);

        // Act
        var result = await _photoService.ApprovePhotoAsync(photoId);

        // Assert
        result.IsError.Should().BeFalse();
        photo.IsApproved.Should().BeTrue();
    }

    [Fact]
    public async Task ApprovePhotoAsync_WhenSaveFails_ReturnsFailureError()
    {
        // Arrange
        var photoId = 1;
        var photo = TestDataFactory.CreatePhoto(photoId, 1);

        _mockPhotoRepository
            .Setup(pr => pr.GetPhotoAsync(photoId))
            .ReturnsAsync(photo);

        _mockPhotoRepository
            .Setup(pr => pr.SaveAllAsync())
            .ReturnsAsync(false);

        // Act
        var result = await _photoService.ApprovePhotoAsync(photoId);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Description.Should().Contain("Failed to Approve");
    }

    #endregion

    #region DeleteAsync(int userId, int photoId) - Happy Path

    [Fact]
    public async Task DeleteAsync_WithValidPhotoId_DeletesPhotoSuccessfully()
    {
        // Arrange
        var userId = 1;
        var photoId = 2;
        var user = TestDataFactory.CreateUser(userId);
        var photo = TestDataFactory.CreatePhoto(photoId, userId);
        photo.IsMain = false;
        user.Photos = new List<Photo> { photo };

        _mockUserRepository
            .Setup(ur => ur.GetAsync(userId, true))
            .ReturnsAsync(user);

        _mockPhotoRepository
            .Setup(pr => pr.GetAsync(photoId))
            .ReturnsAsync(photo);

        _mockCloudinaryService
            .Setup(cs => cs.DeletePhotoAsync(photo.PublicId))
            .ReturnsAsync(Result.Success);

        _mockPhotoRepository
            .Setup(pr => pr.SaveAllAsync())
            .ReturnsAsync(true);

        // Act
        var result = await _photoService.DeleteAsync(userId, photoId);

        // Assert
        result.IsError.Should().BeFalse();
        _mockPhotoRepository.Verify(pr => pr.Delete(photo), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithPhotoWithoutPublicId_DeletesWithoutCloudinary()
    {
        // Arrange
        var userId = 1;
        var photoId = 2;
        var user = TestDataFactory.CreateUser(userId);
        var photo = TestDataFactory.CreatePhoto(photoId, userId);
        photo.IsMain = false;
        photo.PublicId = null;
        user.Photos = new List<Photo> { photo };

        _mockUserRepository
            .Setup(ur => ur.GetAsync(userId, true))
            .ReturnsAsync(user);

        _mockPhotoRepository
            .Setup(pr => pr.GetAsync(photoId))
            .ReturnsAsync(photo);

        _mockPhotoRepository
            .Setup(pr => pr.SaveAllAsync())
            .ReturnsAsync(true);

        // Act
        var result = await _photoService.DeleteAsync(userId, photoId);

        // Assert
        result.IsError.Should().BeFalse();
        _mockCloudinaryService.Verify(cs => cs.DeletePhotoAsync(It.IsAny<string>()), Times.Never);
        _mockPhotoRepository.Verify(pr => pr.Delete(photo), Times.Once);
    }

    #endregion

    #region DeleteAsync(int userId, int photoId) - Error Cases

    [Fact]
    public async Task DeleteAsync_WithMainPhoto_ReturnsFailureError()
    {
        // Arrange
        var userId = 1;
        var photoId = 1;
        var user = TestDataFactory.CreateUser(userId);
        var photo = TestDataFactory.CreatePhoto(photoId, userId);
        photo.IsMain = true;
        user.Photos = new List<Photo> { photo };

        _mockUserRepository
            .Setup(ur => ur.GetAsync(userId, true))
            .ReturnsAsync(user);

        _mockPhotoRepository
            .Setup(pr => pr.GetAsync(photoId))
            .ReturnsAsync(photo);

        // Act
        var result = await _photoService.DeleteAsync(userId, photoId);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Description.Should().Contain("cannot delete your main photo");
    }

    [Fact]
    public async Task DeleteAsync_WithNonexistentUser_ReturnsFailureError()
    {
        // Arrange
        var userId = 999;
        var photoId = 1;

        _mockUserRepository
            .Setup(ur => ur.GetAsync(userId, true))
            .ReturnsAsync((User)null!);

        // Act
        var result = await _photoService.DeleteAsync(userId, photoId);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Description.Should().Contain("Can not find the photo");
    }

    [Fact]
    public async Task DeleteAsync_WithPhotoNotOwnedByUser_ReturnsFailureError()
    {
        // Arrange
        var userId = 1;
        var photoId = 2;
        var user = TestDataFactory.CreateUser(userId);
        user.Photos = new List<Photo>();

        _mockUserRepository
            .Setup(ur => ur.GetAsync(userId, true))
            .ReturnsAsync(user);

        // Act
        var result = await _photoService.DeleteAsync(userId, photoId);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Description.Should().Contain("Can not find the photo");
    }

    [Fact]
    public async Task DeleteAsync_WhenSaveFails_ReturnsFailureError()
    {
        // Arrange
        var userId = 1;
        var photoId = 2;
        var user = TestDataFactory.CreateUser(userId);
        var photo = TestDataFactory.CreatePhoto(photoId, userId);
        photo.IsMain = false;
        user.Photos = new List<Photo> { photo };

        _mockUserRepository
            .Setup(ur => ur.GetAsync(userId, true))
            .ReturnsAsync(user);

        _mockPhotoRepository
            .Setup(pr => pr.GetAsync(photoId))
            .ReturnsAsync(photo);

        _mockCloudinaryService
            .Setup(cs => cs.DeletePhotoAsync(photo.PublicId))
            .ReturnsAsync(Result.Success);

        _mockPhotoRepository
            .Setup(pr => pr.SaveAllAsync())
            .ReturnsAsync(false);

        // Act
        var result = await _photoService.DeleteAsync(userId, photoId);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Description.Should().Contain("Failed to delete the photo");
    }

    [Fact]
    public async Task DeleteAsync_WhenExceptionThrown_ReturnsFailureError()
    {
        // Arrange
        var userId = 1;
        var photoId = 1;
        var exceptionMessage = "Database error";

        _mockUserRepository
            .Setup(ur => ur.GetAsync(userId, true))
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act
        var result = await _photoService.DeleteAsync(userId, photoId);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Description.Should().Contain(exceptionMessage);
    }

    #endregion

    #region DeleteAsync(int photoId)

    [Fact]
    public async Task DeleteAsync_AdminDeletePhoto_WithValidId_DeletesSuccessfully()
    {
        // Arrange
        var photoId = 1;
        var photo = TestDataFactory.CreatePhoto(photoId, 1);
        photo.IsMain = false;

        _mockPhotoRepository
            .Setup(pr => pr.GetPhotoAsync(photoId))
            .ReturnsAsync(photo);

        _mockCloudinaryService
            .Setup(cs => cs.DeletePhotoAsync(photo.PublicId))
            .ReturnsAsync(Result.Success);

        _mockPhotoRepository
            .Setup(pr => pr.SaveAllAsync())
            .ReturnsAsync(true);

        // Act
        var result = await _photoService.DeleteAsync(photoId);

        // Assert
        result.IsError.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_AdminDeletePhoto_WithMainPhoto_ReturnsValidationError()
    {
        // Arrange
        var photoId = 1;
        var photo = TestDataFactory.CreatePhoto(photoId, 1);
        photo.IsMain = true;

        _mockPhotoRepository
            .Setup(pr => pr.GetPhotoAsync(photoId))
            .ReturnsAsync(photo);

        // Act
        var result = await _photoService.DeleteAsync(photoId);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.Validation);
    }

    #endregion
}
