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
        var photoFromRepo = await _repo.GetPhoto(id);

        var photo = _mapper.Map<PhotoForReturnDto>(photoFromRepo);

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

        var userFromRepo = await _repo.GetUser(userId, true);

        var file = photoForCreationDto.File;
        if (file == null)
        {
            return BadRequest("No file was uploaded");
        }
        
        var res = await _cloudinaryService.UploadPhotoAsync(file);

        if (res.IsError)
        {
            return BadRequest(res.FirstError.Description);
        }

        photoForCreationDto.Url = res.Value.Url.ToString();
        photoForCreationDto.PublicId = res.Value.PublicId;

        var photo = _mapper.Map<Photo>(photoForCreationDto);

        if (!userFromRepo.Photos.Any(u => u.IsMain))
        {
            photo.IsMain = true;
        }

        userFromRepo.Photos.Add(photo);

        if (await _repo.SaveAll())
        {
            var photoToReturn = _mapper.Map<PhotoForReturnDto>(photo);
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

        var user = await _repo.GetUser(userId, true);

        if (!user.Photos.Any(p => p.Id == id))
        {
            return Unauthorized();
        }

        var photoFromRepo = await _repo.GetPhoto(id);

        if (photoFromRepo.IsMain)
        {
            return BadRequest("This is already the main photo");
        }

        var currentMainPhoto = await _repo.GetMainPhotoForUser(userId);
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

        var user = await _repo.GetUser(userId, true);

        if (!user.Photos.Any(p => p.Id == id))
        {
            return Unauthorized();
        }

        var photoFromRepo = await _repo.GetPhoto(id);

        if (photoFromRepo.IsMain)
        {
            return BadRequest("You cannot delete your main photo");
        }

        if (photoFromRepo.PublicId != null)
        {
            var res = await _cloudinaryService.DeletePhotoAsync(photoFromRepo.PublicId);

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
