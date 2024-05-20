using Application.Bislerium.Interface;
using Domain.Bislerium.Models;
using Domain.Bislerium.Models.Dto;
using Infrastructure.Bislerium.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Bislerium.Services
{
    public class UserProfile : IUserProfile
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;

        public UserProfile(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
            ApplicationDbContext db
            )
        {
            _userManager = userManager;
            _roleManager = roleManager;

            _db = db;
        }

        //for getting user  list 
        public async Task<IEnumerable<UserInfoResult>> GetUsersListAsync()
        {
            var users = await _userManager.Users.ToListAsync();

            List<UserInfoResult> userInfoResults = new List<UserInfoResult>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var userInfo = GenerateUserInfoObject(user, roles);
                userInfoResults.Add(userInfo);
            }

            return userInfoResults;
        }

        //for getting userdetails by username 
        public async Task<UserInfoResult?> GetUserDetailsByUserNameAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user is null)
                return null;

            var roles = await _userManager.GetRolesAsync(user);
            var userInfo = GenerateUserInfoObject(user, roles);
            return userInfo;
        }
        //get by user profile 
        public async Task<UserInfoResult?> GetUserDetailsByUserIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return null;

            var roles = await _userManager.GetRolesAsync(user);
            var userInfo = GenerateUserInfoObject(user, roles);
            return userInfo;
        }
        //update user profile
        public async Task<bool> UpdateUserProfile(string userId, UpdateUserProfileDto userProfileDto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;

            user.FirstName = userProfileDto.FirstName;
            user.LastName = userProfileDto.LastName;
            user.Address = userProfileDto.Address;
            //user.ProfileImg = userProfileDto.ProfileImg;
            user.UserName = userProfileDto.Username;
            user.UpdatedAt = DateTime.Now; // Assuming you have an UpdatedAt property in ApplicationUser

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        //delete user from databases 

        public async Task<GeneralServiceResponseDto> DeleteUser(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return new GeneralServiceResponseDto
                    {
                        IsSucceed = false,
                        StatusCode = 404,
                        Message = "User not found."
                    };
                }

                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return new GeneralServiceResponseDto
                    {
                        IsSucceed = true,
                        StatusCode = 200,
                        Message = "User deleted successfully."
                    };
                }
                else
                {
                    return new GeneralServiceResponseDto
                    {
                        IsSucceed = false,
                        StatusCode = 400,
                        Message = "Failed to delete user."
                    };
                }
            }
            catch (Exception ex)
            {
                return new GeneralServiceResponseDto
                {
                    IsSucceed = false,
                    StatusCode = 500,
                    Message = $"Internal server error: {ex.Message}"
                };
            }
        }

        //GenerateUserInfoObject
        private UserInfoResult GenerateUserInfoObject(ApplicationUser user, IEnumerable<string> Roles)
        {
            // Instead of this, we can use Automapper packages. But i don't want it in this project
            return new UserInfoResult()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                ProfileImg=user.ProfileImg,
                Roles = Roles
            };
        }



        public async Task<string> UploadFile(IFormFile file)
        {
            string filename = "";
            try
            {
                // Check if the file is valid
                if (file == null || file.Length == 0)
                {
                    throw new ArgumentException("File is empty or null.");
                }

                // Check file size
                if (file.Length > 3 * 1024 * 1024) // 3 MB in bytes
                {
                    throw new ArgumentException("File size exceeds the maximum limit (3MB).");
                }

                // Generate unique filename
                var extension = "." + file.FileName.Split('.')[^1];
                filename = $"{DateTime.Now.Ticks}{extension}";

                // Specify file path
                var filepath = Path.Combine(Directory.GetCurrentDirectory(), "Images", "Files");

                // Create directory if it doesn't exist
                if (!Directory.Exists(filepath))
                {
                    Directory.CreateDirectory(filepath);
                }

                // Save the file to the server
                var fullPath = Path.Combine(filepath, filename);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return filename;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error uploading file: {ex.Message}");
            }
        }

    }
}
