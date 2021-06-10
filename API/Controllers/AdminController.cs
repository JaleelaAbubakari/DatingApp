using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using API.Entities;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using API.DTOs;
using API.Interfaces;
using API.Data;
using System.Security.Claims;
using System.Collections.Generic;

namespace API.Controllers
{
    public class AdminController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;
        public AdminController(UserManager<AppUser> userManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("users-with-roles")]
        public async Task<ActionResult> GetUsersWithRoles()
        {
            var users = await _userManager.Users
                .Include(r => r.UserRoles)
                .ThenInclude(r => r.Role)
                .OrderBy(u => u.UserName)
                .Select(u => new
                {
                    u.Id,
                    Username = u.UserName,
                    Roles = u.UserRoles.Select(r => r.Role.Name).ToList()
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpPost("edit-roles/{username}")]
        public async Task<ActionResult> EditRoles(string username, [FromQuery] string roles)
        {
            var selectedRoles = roles.Split(",").ToArray();

            var user = await _userManager.FindByNameAsync(username);

            if (user == null) return NotFound("Could not find user");

            var userRoles = await _userManager.GetRolesAsync(user);

            var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

            if (!result.Succeeded) return BadRequest("Failed to add to roles");

            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

            if (!result.Succeeded) return BadRequest("Failed to remove from roles");

            return Ok(await _userManager.GetRolesAsync(user));
            
        }


        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photos-to-moderate")]
        public async Task<ActionResult<IEnumerable<PhotoForApprovalDto>>> GetPhotosForApproval()
        {
            var photos = await _unitOfWork.PhotoRepository.GetUnapprovedPhotos();

            return Ok(photos);
        }

        [HttpDelete("reject-photo/{photoId}")]
        public async Task<ActionResult> RejectPhoto(int photoId)
        {

            var photo = await  _unitOfWork.PhotoRepository.GetPhotoById(photoId);

             _unitOfWork.PhotoRepository.RemovePhoto(photo);

            if (await _unitOfWork.Complete()) return Ok();

            return BadRequest("Problem removing the photo");
        }

        [HttpPut("approve-photo/{photoId}")]
        public async Task<ActionResult> ApprovePhoto(int photoId)
        { 
            var photo = await  _unitOfWork.PhotoRepository.GetPhotoById(photoId);

            var user = await _unitOfWork.UserRepository.GetUserByPhotoIdAsync(photoId);

            photo.IsApproved = true;

             var main = user.Photos.FirstOrDefault(x => x.IsMain);
            if (main == null) photo.IsMain = true;
            

             if (await _unitOfWork.Complete()) return NoContent();

                return BadRequest("Failed to approve photo");
        }


    }
}


