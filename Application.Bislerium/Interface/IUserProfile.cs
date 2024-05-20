using Domain.Bislerium.Models.Dto;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Bislerium.Interface
{
    public interface IUserProfile
    {

        //retrive all username in database
        Task<IEnumerable<UserInfoResult>> GetUsersListAsync();
        //retrive user details by username 
        Task<UserInfoResult?> GetUserDetailsByUserNameAsync(string userName);
//get user details by user id
        Task<UserInfoResult?> GetUserDetailsByUserIdAsync(string userId);
            //update user information
            Task<bool> UpdateUserProfile(string userId, UpdateUserProfileDto userProfileDto);

        //for deleting user 
        Task<GeneralServiceResponseDto> DeleteUser(string userId);

        Task<string> UploadFile(IFormFile file);
    }
}
