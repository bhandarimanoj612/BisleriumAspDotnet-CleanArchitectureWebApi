using Application.Bislerium.Interface;
using Domain.Bislerium.Models;
using Domain.Bislerium.Models.Dto;
using Infrastructure.Bislerium.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Bislerium.Services
{
    public class AuthService:IAuthService 
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailService _emailService;

        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _db;

        public AuthService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, 
            IConfiguration configuration,
            IEmailService emailService,
            ApplicationDbContext db
            )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _emailService = emailService;
            _db = db;
        }

        public async Task<GeneralServiceResponseDto> SeedRolesAsync()
        {
            bool isAdminRoleExists = await _roleManager.RoleExistsAsync(StaticUserRoles.ADMIN);//adming

            bool isBloggerRoleExists = await _roleManager.RoleExistsAsync(StaticUserRoles.BLOGGERS);//blogger

            if (isAdminRoleExists && isBloggerRoleExists)
                return new GeneralServiceResponseDto()
                {
                    IsSucceed = true,
                    StatusCode = 200,
                    Message = "Roles Seeding is Already Done"
                };

            await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.ADMIN));//admin

            await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.BLOGGERS));//blogger

            return new GeneralServiceResponseDto()
            {
                IsSucceed = true,
                StatusCode = 201,
                Message = "Roles Seeding Done Successfully"
            };
        }



        public async Task<GeneralServiceResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            try
            {
                var isExistsUser = await _userManager.FindByNameAsync(registerDto.UserName);
                if (isExistsUser != null)
                {
                    return new GeneralServiceResponseDto()
                    {
                        IsSucceed = false,
                        StatusCode = 409,
                        Message = "UserName Already Exists"
                    };
                }

                ApplicationUser newUser = new ApplicationUser()
                {
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    Email = registerDto.Email,
                    UserName = registerDto.UserName,
                    Address = registerDto.Address,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    VerificationCode = GenerateRandomCode() // Initialize VerificationCode
                };

                var createUserResult = await _userManager.CreateAsync(newUser, registerDto.Password);

                if (!createUserResult.Succeeded)
                {
                    var errorString = "User Creation failed because: ";
                    foreach (var error in createUserResult.Errors)
                    {
                        errorString += " # " + error.Description;
                    }
                    return new GeneralServiceResponseDto()
                    {
                        IsSucceed = false,
                        StatusCode = 400,
                        Message = errorString
                    };
                }

                // If this is the first registered user, assign the Admin role
                if (_userManager.Users.Count() == 1)
                {
                    await _userManager.AddToRoleAsync(newUser, StaticUserRoles.ADMIN);//admin
                }
                else
                {
                    // Add a Default USER Role blogger to all users
                    await _userManager.AddToRoleAsync(newUser, StaticUserRoles.BLOGGERS);//blogger
                }

                // Send verification code via email
                var message = new Message(new string[] { newUser.Email }, "Email Verification Code", $"Your verification code is: {newUser.VerificationCode}");
                _emailService.SendEmail(message);

                return new GeneralServiceResponseDto()
                {
                    IsSucceed = true,
                    StatusCode = 201,
                    Message = $"Please check your email for verification. {newUser.UserName} registered successfully."
                };
            }
            catch (Exception ex)
            {
                var errorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return new GeneralServiceResponseDto()
                {
                    IsSucceed = false,
                    StatusCode = 500,
                    Message = $"Internal server error: {errorMessage}"
                };
            }
        }

        //login 
        public async Task<LoginServiceResponseDto?> LoginAsync(LoginDto loginDto)
        {
            // Find user with username
            var user = await _userManager.FindByNameAsync(loginDto.UserName);
            if (user is null)
                return new LoginServiceResponseDto
                {
                    IsSucceed = false,
                    StatusCode = 404, // Not Found
                    Message = "User not found."
                };

            // check password of user
            var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!isPasswordCorrect)
                return new LoginServiceResponseDto
                {
                    IsSucceed = false,
                    StatusCode = 400, // Bad request
                    Message = "Username or password is incorrect."
                };

            ApplicationUser userFromDb = await _db.ApplicationUsers.FirstOrDefaultAsync(u => u.UserName.ToLower() == loginDto.UserName.ToLower());
            if (userFromDb == null || !await _userManager.CheckPasswordAsync(userFromDb, loginDto.Password))
            {
                return new LoginServiceResponseDto
                {
                    IsSucceed = false,
                    StatusCode = 400, // Bad request
                    Message = "Username or password is incorrect."
                };
            }

            // Check if the user's email is verified
            if (!userFromDb.EmailConfirmed)
            {
                // Generate a new verification code
                var newVerificationCode = GenerateRandomCode();

                // Update the user's verification code
                userFromDb.VerificationCode = newVerificationCode;
                await _userManager.UpdateAsync(userFromDb);

                // Send the verification code to the user's email
                var message = new Message(new string[] { userFromDb.Email }, "Verification Code", $"Your verification code is: {newVerificationCode}");
                _emailService.SendEmail(message);

                return new LoginServiceResponseDto
                {
                    IsSucceed = false,
                    StatusCode = 400, // Bad request
                    Message = "Email is not verified. A new verification code has been sent to your email. Please verify your email before logging in."
                };
            }

            // Return Token and userInfo to front-end
            var newToken = await GenerateJWTTokenAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            
            var userInfo = GenerateUserInfoObject(user, roles);

            return new LoginServiceResponseDto()
            {
                IsSucceed = true,
                StatusCode = 200, // OK
                NewToken = newToken,
                UserInfo = userInfo,
                Message = "Login Successfully"
            };
        }



        //for sending email verification

        public async Task<GeneralServiceResponseDto> ForgotPassword(ForgotPasswordDTO model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                var otp = GenerateRandomCode(); // Generate OTP
                user.VerificationCode = otp; // Store OTP in user entity
                await _userManager.UpdateAsync(user);

                var message = new Message(new string[] { user.Email }, "Reset Password OTP", $"Your OTP for password reset is: {otp}");
                _emailService.SendEmail(message);

                return new GeneralServiceResponseDto { IsSucceed = true, Message = $"An OTP has been sent to your email: {user.Email}. Please check your inbox." };
            }
            return new GeneralServiceResponseDto { IsSucceed = false, Message = "User not found." };
        }

        // Utility method to generate a random code
        private string GenerateRandomCode()
        {
            // Implement your random code generation logic here
            // Example: Generate a random 6-digit numeric code
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }
        //reset password
        public async Task<GeneralServiceResponseDto> ResetPassword(ResetPasswordDTO resetPassword)
        {
            var user = await _userManager.FindByEmailAsync(resetPassword.Email);
            if (user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, resetPassword.Password);

                if (result.Succeeded)
                {
                    // Clear OTP after successful password reset
                    user.VerificationCode = resetPassword.OTP;
                    await _userManager.UpdateAsync(user);

                    return new GeneralServiceResponseDto { IsSucceed = true, Message = "Password has been reset successfully." };
                }
                else
                {
                    return new GeneralServiceResponseDto { IsSucceed = false, Message = "Failed to reset password." };
                }
            }
            return new GeneralServiceResponseDto { IsSucceed = false, Message = "User not found." };
        }

        // Verify OTP and mark email verification as true
        public async Task<GeneralServiceResponseDto> VerifyOTP(VerifyOTPDTO model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                if (user.VerificationCode == model.OTP) // Check if OTP matches
                {
                    // Clear OTP after successful verification
                    user.VerificationCode = model.OTP;
                    // Mark email verification as true
                    user.EmailConfirmed = true;
                    await _userManager.UpdateAsync(user);

                    // Redirect the user to the reset password screen
                    return new GeneralServiceResponseDto { IsSucceed = true, Message = "OTP verified successfully" };
                }
                else
                {
                    return new GeneralServiceResponseDto { IsSucceed = false, Message = "Invalid OTP." };
                }
            }
            return new GeneralServiceResponseDto { IsSucceed = false, Message = "User not found." };
        }





        public async Task<GeneralServiceResponseDto> UpdateRoleAsync(ClaimsPrincipal user, UpdateRoleDto updateRoleDto)
        {
           

            var userToUpdate = await _userManager.FindByNameAsync(updateRoleDto.UserName);
            if (userToUpdate == null)
            {
                return new GeneralServiceResponseDto()
                {
                    IsSucceed = false,
                    StatusCode = 404,
                    Message = "Invalid UserName"
                };
            }

            try
            {
                // Remove all existing roles and assign the new role
                await _userManager.RemoveFromRolesAsync(userToUpdate, await _userManager.GetRolesAsync(userToUpdate));
                await _userManager.AddToRoleAsync(userToUpdate, updateRoleDto.NewRole.ToString());

                return new GeneralServiceResponseDto()
                {
                    IsSucceed = true,
                    StatusCode = 200,
                    Message = "Role updated successfully"
                };
            }
            catch (Exception ex)
            {
                // Log the exception or handle it accordingly
                return new GeneralServiceResponseDto()
                {
                    IsSucceed = false,
                    StatusCode = 500,
                    Message = "An error occurred while updating the role"
                };
            }
        }


        public async Task<IEnumerable<UserWithRoleDto>> GetUsernamesListAsync()
        {
            var usersWithRoles = await _userManager.Users
                .ToListAsync(); // Materialize the users before async operations

            var usersWithRolesDto = new List<UserWithRoleDto>();

            foreach (var user in usersWithRoles)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var role = roles.FirstOrDefault() ?? "No Role";

                usersWithRolesDto.Add(new UserWithRoleDto
                {
                    UserName = user.UserName,
                    Role = role
                });
            }

            return usersWithRolesDto;
        }

        //GenerateJWTTokenAsync
        private async Task<string> GenerateJWTTokenAsync(ApplicationUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim("FirstName", user.FirstName),
                new Claim("LastName", user.LastName),
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var authSecret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var signingCredentials = new SigningCredentials(authSecret, SecurityAlgorithms.HmacSha256);

            var tokenObject = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: signingCredentials
                );

            string token = new JwtSecurityTokenHandler().WriteToken(tokenObject);
            return token;
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
                Roles = Roles,
                // Check if ProfileImg is not null before assigning
                ProfileImg = user.ProfileImg != null ? user.ProfileImg : "ImageisNull"
            };
        }


    }
}
