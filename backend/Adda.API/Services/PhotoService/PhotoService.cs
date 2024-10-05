using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Adda.API.ExternalServices.Cloudinary;
using Adda.API.Models;
using Adda.API.Repositories.PhotoRepository;
using Adda.API.Repositories.UserRepository;
using ErrorOr;
using Microsoft.AspNetCore.Http;

namespace Adda.API.Services.PhotoService;

public class PhotoService(IUserRepository userRepository, IPhotoRepository photoRepository, ICloudinaryService cloudinaryService) : IPhotoService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPhotoRepository _photoRepository = photoRepository;
    private readonly ICloudinaryService _cloudinaryService = cloudinaryService;

    public async Task<ErrorOr<Photo>> AddAsync(int userId, IFormFile file)
    {
        try
        {
            User userFromRepo = await _userRepository.GetAsync(userId, true);

            if (file == null)
            {
                return Error.Failure("No file was uploaded");
            }

            ErrorOr<PhotoUploadResult> res = await _cloudinaryService.UploadPhotoAsync(
                file
            );

            if (res.IsError)
            {
                return Error.Failure(res.FirstError.Description);
            }

            Photo photo = new()
            {
                Url = res.Value.Url,
                PublicId = res.Value.PublicId,
                DateAdded = DateTime.Now,
                IsMain = false,
                UserId = userId,
                User = userFromRepo,
            };

            if (!userFromRepo.Photos.Any(u => u.IsMain))
            {
                photo.IsMain = true;
            }

            userFromRepo.Photos.Add(photo);

            if (await _userRepository.SaveAllAsync())
            {
                return photo;
            }
            return Error.Failure(description: "");
        }
        catch (Exception ex)
        {
            return Error.Failure(description: ex.Message);
        }
    }

    public async Task<ErrorOr<Success>> ApprovePhotoAsync(int photoId)
    {
        Photo photo = await _photoRepository.GetPhotoAsync(photoId);

        photo.IsApproved = true;

        if (await _photoRepository.SaveAllAsync())
        {
            return Result.Success;
        }
        return Error.Failure(description: "Failed to Approve user photo");
    }

    public async Task<ErrorOr<Success>> DeleteAsync(int userId, int id)
    {
        try
        {
            User user = await _userRepository.GetAsync(userId, true);

            if (!user.Photos.Any(p => p.Id == id))
            {
                return Error.Failure(description: "Can not find the photo");
            }

            Photo photoFromRepo = await _photoRepository.GetAsync(id);

            if (photoFromRepo.IsMain)
            {
                return Error.Failure(description: "You cannot delete your main photo");
            }

            if (photoFromRepo.PublicId != null)
            {
                ErrorOr<Success> res = await _cloudinaryService.DeletePhotoAsync(photoFromRepo.PublicId);

                if (!res.IsError)
                {
                    _photoRepository.Delete(photoFromRepo);
                }
            }
            else
            {
                _photoRepository.Delete(photoFromRepo);
            }

            if (await _photoRepository.SaveAllAsync())
            {
                return Result.Success;
            }
            return Error.Failure(description: "Failed to delete the photo");
        }
        catch (Exception ex)
        {
            return Error.Failure(description: ex.Message);
        }
    }

    public async Task<ErrorOr<Success>> DeleteAsync(int photoId)
    {
        Photo photo = await _photoRepository.GetPhotoAsync(photoId);

        if (photo.IsMain)
        {
            return Error.Validation("You cannot delete your main photo");
        }

        if (photo.PublicId != null)
        {
            ErrorOr<Success> result = await _cloudinaryService.DeletePhotoAsync(photo.PublicId);

            if (!result.IsError)
            {
                _photoRepository.Delete(photo);
            }
        }

        if (photo.PublicId == null)
        {
            _photoRepository.Delete(photo);
        }

        if (await _photoRepository.SaveAllAsync())
        {
            return Result.Success;
        }
        return Error.Failure(description: "Failed to delete the photo");
    }

    public Task<IEnumerable<object>> GetAllPhotosAsync()
    {
        return _photoRepository.GetAllPhotosAsync();
    }

    public async Task<ErrorOr<Photo>> GetAsync(int id)
    {
        return await _photoRepository.GetAsync(id);
    }

    public async Task<Photo> GetPhotoAsync(int photoId)
    {
        return await _photoRepository.GetPhotoAsync(photoId);
    }

    public async Task<ErrorOr<Success>> SetMainPhotoAsync(int userId, int id)
    {
        User user = await _userRepository.GetAsync(userId, true);

        if (!user.Photos.Any(p => p.Id == id))
        {
            return Error.Failure(description: "Can not find the photo");
        }

        Photo photoFromRepo = await _photoRepository.GetAsync(id);

        if (photoFromRepo.IsMain)
        {
            return Error.Validation(description: "This is already the main photo");
        }

        Photo currentMainPhoto = await _photoRepository.GetMainPhotoForUserAsync(userId);
        currentMainPhoto.IsMain = false;

        photoFromRepo.IsMain = true;

        if (await _photoRepository.SaveAllAsync())
        {
            return Result.Success;
        }
        return Error.Failure(description: "Could not set photo to main");
    }


}
