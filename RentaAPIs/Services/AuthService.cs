using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using RentaAPIs.DTOs;
using RentaAPIs.Models;

namespace RentaAPIs.Services
{
    public class AuthService
    {

        private readonly UserManager<AppUser> _userManager;
        private readonly IJwtService _jwtService;

        public AuthService(UserManager<AppUser> userManager, IJwtService jwtService)
        {
            _userManager = userManager;
            _jwtService = jwtService;
        }

        public async Task<string> GetAuthenticatorKey(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if(user == null)
            {
                throw new Exception("User Not Found");
            }

            var key = await _userManager.GetAuthenticatorKeyAsync(user);

            if(string.IsNullOrEmpty(key))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);

                key = await _userManager.GetAuthenticatorKeyAsync(user);
            }

            return key;
        }

        public async Task<bool> VerifyAuthenticatorCodeAsync(string userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return false;

            var isValid = await _userManager.VerifyTwoFactorTokenAsync(user, _userManager.Options.Tokens.AuthenticatorTokenProvider, code);

            if (!isValid)
                return false;

            await _userManager.SetTwoFactorEnabledAsync(user, true);

            return true;
        }


        public async Task<LoginResponseDTO> Login (LoginRequestDTO lrDTO)
        {
            var normalisedEmail = _userManager.NormalizeEmail(lrDTO.Email);
            
            Console.WriteLine($"Email: {lrDTO.Email}");
            Console.WriteLine($"Normalized Email: {normalisedEmail}");
            
            var user = await _userManager.FindByEmailAsync(lrDTO.Email);

            if(user == null)
            {
                return new LoginResponseDTO
                {
                    Success = false,
                    RequiresTwoFactor = false,
                    RequiresTwoFactorSetup = false,
                    Message = "Invalid email or password USER"
                };
            }

            var passwordValid = await _userManager.CheckPasswordAsync(user, lrDTO.Password);

            if(!passwordValid)
            {
                return new LoginResponseDTO
                {
                    Success = false,
                    RequiresTwoFactor = false,
                    RequiresTwoFactorSetup = false,
                    Message = "Invalid email or password PASSWORD"
                };
            }

            var twoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user);

            if(!twoFactorEnabled)
            {
                return new LoginResponseDTO
                {
                    Success = true,
                    RequiresTwoFactor = false,
                    RequiresTwoFactorSetup = true,
                    UserId = user.Id,
                    AccessToken = null,
                    Message = "Two-factor authentication setup required"
                };
            }

            if(string.IsNullOrEmpty(lrDTO.TwoFactorCode))
            {
                return new LoginResponseDTO
                {
                    Success = true,
                    RequiresTwoFactor = true,
                    RequiresTwoFactorSetup = false,
                    AccessToken = null,
                    Message = "Enter your Google Authenticator code"
                };
            }

            var codeValid = await _userManager.VerifyTwoFactorTokenAsync(user, _userManager.Options.Tokens.AuthenticatorTokenProvider, lrDTO.TwoFactorCode);

            if(!codeValid)
            {
                return new LoginResponseDTO
                {
                    Success = false,
                    RequiresTwoFactor = true,
                    RequiresTwoFactorSetup = false,
                    AccessToken = null,
                    Message = "Invalid Authenticator code"
                };
            }

            var roles = await _userManager.GetRolesAsync(user);

            if(roles.Count == 0)
            {
                return new LoginResponseDTO
                {
                    Success = false,
                    RequiresTwoFactor = false,
                    Message = "User does not have a valid role"
                };
            }

            var token = _jwtService.GenerateToken(user, roles);

            return new LoginResponseDTO
            {
                Success = true,
                RequiresTwoFactor = false,
                RequiresTwoFactorSetup = false,
                AccessToken = token,
                Message = "Login successfull"
            };
        }
    }
}
