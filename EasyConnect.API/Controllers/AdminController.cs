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
        public async Task<IActionResult> GetUsersWithRoles()
        {
            var userList = await _repo.GetUsersWithRoles();
            return Ok(userList);
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photosForModeration")]
        public async Task<IActionResult> GetPhotosForModeration()
        {
            var photos = await _repo.GetAllPhotos();
            return Ok(photos);
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpPut("photo/{photoId}")]
        public async Task<IActionResult> ApprovePhoto(int photoId)
        {
            var photo = await _repo.GetPhoto(photoId);

            photo.IsApproved = true;

            if (await _repo.SaveAll())
            {
                return NoContent();
            }
            return BadRequest("Failed to Approve user photo");
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpDelete("photo/{photoId}")]
        public async Task<IActionResult> DeletePhoto(int photoId)
        {
            var photo = await _repo.GetPhoto(photoId);

            if (photo.IsMain)
            {
                return BadRequest("You cannot delete your main photo");
            }

            if (photo.PublicId != null)
            {
                var result = await _cloudinaryService.DeletePhotoAsync(photo.PublicId);

                if (!result.IsError)
                {
                    _repo.Delete(photo);
                }
            }

            if (photo.PublicId == null)
            {
                _repo.Delete(photo);
            }

            if (await _repo.SaveAll())
            {
                return Ok();
            }
            return BadRequest("Failed to delete the photo");
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("editRoles/{userName}")]
        public async Task<IActionResult> editRoles(string userName, RoleEditDto roleEditDto)
        {
            var user = await _userManager.FindByNameAsync(userName);

            var userRoles = await _userManager.GetRolesAsync(user);

            var selectedRoles = roleEditDto.RoleName;

            selectedRoles ??= [];
            var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

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
