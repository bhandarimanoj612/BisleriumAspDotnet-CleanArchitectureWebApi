using Application.Bislerium.Interface;
using Domain.Bislerium.Models;
using Domain.Bislerium.Models.Dto;
using Infrastructure.Bislerium.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Presentation.Bislerium.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfileControllers : ControllerBase
    {
        private readonly IUserProfile _userProfile; // Change from IAuthService to IUserProfile
       
        private readonly UserManager<ApplicationUser> _userManager;
        public UserProfileControllers(IUserProfile userProfile,  UserManager<ApplicationUser> userManager) // Change from IAuthService to IUserProfile
        {
            _userProfile = userProfile; // Change from _authService to _userProfile
          
            _userManager = userManager;
        }

        // Route -> List of all users with details
        [HttpGet]
        [Route("users/GetAllUser")]
        public async Task<ActionResult<IEnumerable<UserInfoResult>>> GetUsersList()
        {
            var usersList = await _userProfile.GetUsersListAsync(); // Change from _authService to _userProfile

            return Ok(usersList);
        }

        // Route -> Get a User by UserName
        [HttpGet]
        [Route("users/GetByUserName/{userName}")]
        public async Task<ActionResult<UserInfoResult>> GetUserDetailsByUserName([FromRoute] string userName)
        {
            var user = await _userProfile.GetUserDetailsByUserNameAsync(userName); // Change from _authService to _userProfile
            if (user is not null)
            {
                return Ok(user);
            }
            else
            {
                return NotFound("UserName not found");
            }
        }

        // Route -> Get a User by ID
        [HttpGet]
        [Route("users/GetByUserId/{userId}")]
        public async Task<ActionResult<UserInfoResult>> GetUserDetailsByUserId([FromRoute] string userId)
        {
            var user = await _userProfile.GetUserDetailsByUserIdAsync(userId);
            if (user is not null)
            {
                return Ok(user);
            }
            else
            {
                return NotFound("User not found");
            }
        }

        //update user profile 
        [HttpPut("users/UpdateUsers/{userId}")]
        public async Task<IActionResult> UpdateProfileOfUser(string userId, UpdateUserProfileDto userProfileDto)
        {
            var result = await _userProfile.UpdateUserProfile(userId, userProfileDto); 
            if (result)
                return Ok("User profile updated successfully");
            else
                return BadRequest("Failed to update user profile");
        }

        //for delte user 
        [HttpDelete]
        [Route("DeleteUser")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var verifyOTPResult = await _userProfile.DeleteUser(userId); 
            return StatusCode(verifyOTPResult.StatusCode, verifyOTPResult.Message);
        }

        // for upload image
        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadImage(IFormFile file, string username)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("File is empty or null.");
                }

                var user = await _userManager.FindByNameAsync(username);

                if (user == null)
                {
                    return NotFound("User not found.");
                }

                var filename = await _userProfile.UploadFile(file);

                user.ProfileImg = filename;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return Ok("Image uploaded successfully.");
                }
                else
                {
                    return BadRequest("Failed to upload image.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
