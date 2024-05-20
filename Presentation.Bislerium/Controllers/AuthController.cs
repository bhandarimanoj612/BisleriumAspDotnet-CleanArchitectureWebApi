using Application.Bislerium.Interface;
using Domain.Bislerium.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Bislerium.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        // Route -> Seed Roles to DB this will send role to database from below 
        [HttpPost]
        [Route("seed-roles")]
        public async Task<IActionResult> SeedRoles()
        {
            var seedResult = await _authService.SeedRolesAsync();
            return StatusCode(seedResult.StatusCode, seedResult.Message);
        }

        // Route -> Register
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var registerResult = await _authService.RegisterAsync(registerDto);
            return StatusCode(registerResult.StatusCode, registerResult.Message);
        }

        // Route -> Login
        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<LoginServiceResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            var loginResult = await _authService.LoginAsync(loginDto);

            if (loginResult is null)
            {
                return Unauthorized("Your credentials are invalid. Please contact to an Admin");
            }

            return Ok(loginResult);
        }

      
        [HttpPost]
        [Route("update-role")]
        [Authorize(Roles = StaticUserRoles.ADMIN)]
        public async Task<IActionResult> UpdateRole([FromBody] UpdateRoleDto updateRoleDto)
        {
            var updateRoleResult = await _authService.UpdateRoleAsync(User, updateRoleDto);

            if (updateRoleResult.IsSucceed)
            {
                return Ok(updateRoleResult.Message);
            }
            else
            {
                return StatusCode(updateRoleResult.StatusCode, updateRoleResult.Message);
            }
        }

        // Route -> Get List of all usernames with roles
        [HttpGet]
        [Route("usernames/AllWithRoles")]
        //[Authorize(Roles = StaticUserRoles.ADMIN)] // Main admin can get usernames with roles
        public async Task<ActionResult<IEnumerable<UserWithRoleDto>>> GetUsernamesListWithRoles()
        {
            var usersWithRoles = await _authService.GetUsernamesListAsync();

            return Ok(usersWithRoles);
        }

        //for forget-password

        [HttpPost]
        [Route("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO model)
        {
            var forgotPasswordResult = await _authService.ForgotPassword(model);
            return StatusCode(forgotPasswordResult.StatusCode, forgotPasswordResult.Message);
        }
        //for reset-password
        [HttpPost]
        [Route("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO resetPassword)
        {
            var resetPasswordResult = await _authService.ResetPassword(resetPassword);
            return StatusCode(resetPasswordResult.StatusCode, resetPasswordResult.Message);
        }
        //for verifying our reset password otp 
        [HttpPost]
        [Route("verify-otp")]
        public async Task<IActionResult> VerifyOTP([FromBody] VerifyOTPDTO model)
        {
            var verifyOTPResult = await _authService.VerifyOTP(model);
            return StatusCode(verifyOTPResult.StatusCode, verifyOTPResult.Message);
        }


    }
}
