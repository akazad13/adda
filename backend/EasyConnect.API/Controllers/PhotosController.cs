using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using EasyConnect.API.Data;
using EasyConnect.API.Dtos;
using EasyConnect.API.ExternalServices.Cloudinary;
using EasyConnect.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace EasyConnect.API.Controllers;

[ApiController]
[Route("api/users/{userId}/photos")]
public class PhotosController(IMemberRepository repo, IMapper mapper, ICloudinaryService cloudinaryService) : ControllerBase
{
    private readonly IMemberRepository _repo = repo;
    private readonly IMapper _mapper = mapper;
    private readonly ICloudinaryService _cloudinaryService = cloudinaryService;

    [HttpGet("{id}", Name = "GetPhoto")]
    public async Task<IActionResult> GetPhoto(int id)
    {
        Photo photoFromRepo = await _repo.GetPhoto(id);

        PhotoForReturnDto photo = _mapper.Map<PhotoForReturnDto>(photoFromRepo);

        return Ok(photo);
    }

    [HttpPost]
    public async Task<IActionResult> AddPhotoForUser(
        int userId,
        [FromForm] PhotoForCreationDto photoForCreationDto
    )
    {
        if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
        {
            return Unauthorized();
        }

        User userFromRepo = await _repo.GetUser(userId, true);

        Microsoft.AspNetCore.Http.IFormFile file = photoForCreationDto.File;
        if (file == null)
        {
            return BadRequest("No file was uploaded");
        }

        ErrorOr.ErrorOr<PhotoUploadResult> res = await _cloudinaryService.UploadPhotoAsync(file);

        if (res.IsError)
        {
            return BadRequest(res.FirstError.Description);
        }

        photoForCreationDto.Url = res.Value.Url.ToString();
        photoForCreationDto.PublicId = res.Value.PublicId;

        Photo photo = _mapper.Map<Photo>(photoForCreationDto);

        if (!userFromRepo.Photos.Any(u => u.IsMain))
        {
            photo.IsMain = true;
        }

        userFromRepo.Photos.Add(photo);

        if (await _repo.SaveAll())
        {
            PhotoForReturnDto photoToReturn = _mapper.Map<PhotoForReturnDto>(photo);
            return CreatedAtRoute(
                "GetPhoto",
                new { userId, id = photo.Id },
                photoToReturn
            );
        }

        return BadRequest("Could not add the photo");
    }

    [HttpPost("{id}/setMain")]
    public async Task<IActionResult> SetMainPhoto(int userId, int id)
    {
        if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
        {
            return Unauthorized();
        }

        User user = await _repo.GetUser(userId, true);

        if (!user.Photos.Any(p => p.Id == id))
        {
            return Unauthorized();
        }

        Photo photoFromRepo = await _repo.GetPhoto(id);

        if (photoFromRepo.IsMain)
        {
            return BadRequest("This is already the main photo");
        }

        Photo currentMainPhoto = await _repo.GetMainPhotoForUser(userId);
        currentMainPhoto.IsMain = false;

        photoFromRepo.IsMain = true;

        if (await _repo.SaveAll())
        {
            return NoContent();
        }
        return BadRequest("Could not set photo to main");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePhoto(int userId, int id)
    {
        if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
        {
            return Unauthorized();
        }

        User user = await _repo.GetUser(userId, true);

        if (!user.Photos.Any(p => p.Id == id))
        {
            return Unauthorized();
        }

        Photo photoFromRepo = await _repo.GetPhoto(id);

        if (photoFromRepo.IsMain)
        {
            return BadRequest("You cannot delete your main photo");
        }

        if (photoFromRepo.PublicId != null)
        {
            ErrorOr.ErrorOr<ErrorOr.Success> res = await _cloudinaryService.DeletePhotoAsync(photoFromRepo.PublicId);

            if (res.IsError)
            {
                _repo.Delete(photoFromRepo);
            }
        }

        if (photoFromRepo.PublicId == null)
        {
            _repo.Delete(photoFromRepo);
        }

        if (await _repo.SaveAll())
        {
            return Ok();
        }
        return BadRequest("Failed to delete the photo");
    }
}
