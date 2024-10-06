using System.Threading.Tasks;
using Adda.API.Dtos;
using Adda.API.Models;
using Adda.API.Security.CurrentUserProvider;
using Adda.API.Services.PhotoService;
using AutoMapper;
using ErrorOr;
using Microsoft.AspNetCore.Mvc;

namespace Adda.API.Controllers;

[ApiController]
[Route("api/users/{userId}/photos")]
public class PhotosController(IMapper mapper, ICurrentUserProvider currentUser, IPhotoService photoService) : ControllerBase
{
    private readonly IMapper _mapper = mapper;
    private readonly ICurrentUserProvider _currentUser = currentUser;
    private readonly IPhotoService _photoService = photoService;

    [HttpGet("{id}", Name = "GetPhoto")]
    public async Task<IActionResult> GetAsync(int id)
    {
        ErrorOr<Photo> result = await _photoService.GetAsync(id);

        if (!result.IsError)
        {
            PhotoResponse photo = _mapper.Map<PhotoResponse>(result.Value);
            return Ok(photo);
        }
        return BadRequest("Failed to get photo details");
    }

    [HttpPost]
    public async Task<IActionResult> AddPhotoForUserAsync(
        int userId,
        [FromForm] CreatePhotoRequest request
    )
    {
        if (userId != _currentUser.UserId)
        {
            return Unauthorized();
        }

        ErrorOr<Photo> result = await _photoService.AddAsync(userId, request.File);

        if (!result.IsError)
        {
            PhotoResponse photoToReturn = _mapper.Map<PhotoResponse>(result.Value);
            return CreatedAtRoute(
                "GetPhoto",
                new { userId, id = result.Value.Id },
                photoToReturn
            );
        }

        return BadRequest("Could not add the photo");
    }

    [HttpPost("{id}/setMain")]
    public async Task<IActionResult> SetMainPhotoAsync(int userId, int id)
    {
        if (userId != _currentUser.UserId)
        {
            return Unauthorized();
        }

        ErrorOr<Success> result = await _photoService.SetMainPhotoAsync(userId, id);

        if (!result.IsError)
        {
            return NoContent();
        }
        return BadRequest("Could not set photo to main");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(int userId, int id)
    {
        if (userId != _currentUser.UserId)
        {
            return Unauthorized();
        }

        ErrorOr<Success> result = await _photoService.DeleteAsync(userId, id);

        if (!result.IsError)
        {
            return Ok();
        }
        return BadRequest("Failed to delete the photo");
    }
}
