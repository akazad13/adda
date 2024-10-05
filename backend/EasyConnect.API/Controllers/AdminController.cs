using System.Linq;
using System.Threading.Tasks;
using EasyConnect.API.Data;
using EasyConnect.API.Dtos;
using EasyConnect.API.ExternalServices.Cloudinary;
using EasyConnect.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EasyConnect.API.Controllers;
[ApiController]
[Route("api/admin")]
public class AdminController(
    IAdminRepository repo,
    UserManager<User> userManager,
    ICloudinaryService cloudinaryService
        ) : ControllerBase
{
    private readonly IAdminRepository _repo = repo;
    private readonly UserManager<User> _userManager = userManager;
    private readonly ICloudinaryService _cloudinaryService = cloudinaryService;

    [Authorize(Policy = "RequireAdminRole")]
    [HttpGet("usersWithRoles")]
    public async Task<IActionResult> GetUsersWithRolesAsync()
    {
        System.Collections.Generic.IEnumerable<object> userList = await _repo.GetUsersWithRolesAsync();
        return Ok(userList);
    }

    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpGet("photosForModeration")]
    public async Task<IActionResult> GetPhotosForModerationAsync()
    {
        System.Collections.Generic.IEnumerable<object> photos = await _repo.GetAllPhotosAsync();
        return Ok(photos);
    }

    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpPut("photo/{photoId}")]
    public async Task<IActionResult> ApprovePhotoAsync(int photoId)
    {
        Photo photo = await _repo.GetPhotoAsync(photoId);

        photo.IsApproved = true;

        if (await _repo.SaveAllAsync())
        {
            return NoContent();
        }
        return BadRequest("Failed to Approve user photo");
    }

    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpDelete("photo/{photoId}")]
    public async Task<IActionResult> DeletePhotoAsync(int photoId)
    {
        Photo photo = await _repo.GetPhotoAsync(photoId);

        if (photo.IsMain)
        {
            return BadRequest("You cannot delete your main photo");
        }

        if (photo.PublicId != null)
        {
            ErrorOr.ErrorOr<ErrorOr.Success> result = await _cloudinaryService.DeletePhotoAsync(photo.PublicId);

            if (!result.IsError)
            {
                _repo.Delete(photo);
            }
        }

        if (photo.PublicId == null)
        {
            _repo.Delete(photo);
        }

        if (await _repo.SaveAllAsync())
        {
            return Ok();
        }
        return BadRequest("Failed to delete the photo");
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpPost("editRoles/{userName}")]
    public async Task<IActionResult> EditRolesAsync(string userName, RoleEditDto roleEditDto)
    {
        User user = await _userManager.FindByNameAsync(userName);

        System.Collections.Generic.IList<string> userRoles = await _userManager.GetRolesAsync(user);

        string[] selectedRoles = roleEditDto.RoleName;

        selectedRoles ??= [];
        IdentityResult result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

        if (!result.Succeeded)
        {
            return BadRequest("Failed to add to roles");
        }

        result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

        if (!result.Succeeded)
        {
            return BadRequest("Failed to remove the roles");
        }

        return Ok(await _userManager.GetRolesAsync(user));
    }
}
