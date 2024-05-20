using Domain.Bislerium.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Bislerium.Interface
{
    public interface IAuthService
    {

        //for seeding roles in database
        Task<GeneralServiceResponseDto> SeedRolesAsync();

        //for register new user 
        Task<GeneralServiceResponseDto> RegisterAsync(RegisterDto registerDto);

        //for login user 
        Task<LoginServiceResponseDto?> LoginAsync(LoginDto loginDto);
        //for update user roles 
        Task<GeneralServiceResponseDto> UpdateRoleAsync(ClaimsPrincipal User, UpdateRoleDto updateRoleDto);

        //for getting usernames 
        Task<IEnumerable<UserWithRoleDto>> GetUsernamesListAsync();

        //for forget password
        Task<GeneralServiceResponseDto> ForgotPassword(ForgotPasswordDTO model);

        //for reset password 
        Task<GeneralServiceResponseDto> ResetPassword(ResetPasswordDTO resetPassword);
        //for verify otp modal 

        Task<GeneralServiceResponseDto> VerifyOTP(VerifyOTPDTO model);
    }
}
