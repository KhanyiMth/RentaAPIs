using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RentaAPIs.DTOs;
using RentaAPIs.Models;
using RentaAPIs.Services;
using System.Security.Claims;

namespace RentaAPIs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _auth;
        private readonly UserManager<AppUser> _userManager;
        private readonly IJwtService _jwtService;


        public AuthController (AuthService auth, UserManager<AppUser> userManager, IJwtService jwtService)
        {
            _auth = auth;
            _userManager = userManager;
            _jwtService = jwtService;
        }

        [AllowAnonymous]
        [HttpGet("2fa/authenticator-key")]
        public async Task<IActionResult> GetAuthenticatorKey([FromQuery] string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);

                if(user == null)
                {
                    return NotFound(new
                    {
                        message = "User not found"
                    });
                }

                var key = await _userManager.GetAuthenticatorKeyAsync(user);

                if(string.IsNullOrEmpty(key))
                {
                    var result = await _userManager.ResetAuthenticatorKeyAsync(user);

                    if(!result.Succeeded)
                    {
                        return BadRequest(new
                        {
                            message = "Could not generate authenticator key"
                        });
                    }

                    key = await _userManager.GetAuthenticatorKeyAsync(user);
                }

                var otpauthUrl =
                    $"otpauth://totp/Renta:{Uri.EscapeDataString(user.Email!)}" +
                    $"?secret={key}" +
                    $"&issuer=Renta";

                return Ok(new
                {
                    secret = key,
                    otpauthUrl = otpauthUrl,
                    email = user.Email
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = ex.Message
                });
            }
        }

        [AllowAnonymous]
        [HttpPost("2fa/verify-login")]
        public async Task<IActionResult> VerifyLoginAuthenticator(VerifyAuthenticatorRequest request)
        {
            if(string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Code))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Email and authentication code are required"
                });
            }

            var user = await _userManager.FindByEmailAsync(request.Email);

            if(user == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "User not found"
                });
            }

            var isValid = await _auth.VerifyAuthenticatorCodeAsync(user.Id, request.Code);

            if(!isValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid authenticator code"
                });
            }

            var roles = await _userManager.GetRolesAsync(user);

            var token = _jwtService.GenerateToken(user, roles);

            return Ok(new
            {
                success = true,
                accessToken = token,
                message = "Login successful"
            });
        }

        [AllowAnonymous]
        [HttpPost("2fa/verify")]
        public async Task<IActionResult> VerifyAuthenticator(VerifyAuthenticatorRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Code))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Email and authentication code are required"
                });
            }

            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "User not found"
                });
            }

            var isValid = await _userManager.VerifyTwoFactorTokenAsync(user, _userManager.Options.Tokens.AuthenticatorTokenProvider, request.Code);

            if (!isValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid authentication code"
                });
            }

            var enableResult = await _userManager.SetTwoFactorEnabledAsync(user, true);

            if(!enableResult.Succeeded)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Unable to enable two-factor authentication"
                });
            }

            var roles = await _userManager.GetRolesAsync(user);

            var token = _jwtService.GenerateToken(user, roles);

            return Ok(new
            {
                success = true,
                accessToken = token,
                message = "Two-factor authentication enabled successfully."
            });
        }


        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login (LoginRequestDTO lrDTO)
        {
            var result = await _auth.Login(lrDTO);

            if(result.Message == "Invalid email or password")
            {
                return Unauthorized(result);
            }

            return Ok(result);
        }
    }
}
