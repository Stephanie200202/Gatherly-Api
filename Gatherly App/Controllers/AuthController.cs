
using Gatherly.Application.Services;
using Gatherly.Applicaton.DTOs;
using Gatherly.Applicaton.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
namespace Gatherly_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthServices _authService;

        public AuthController(IAuthServices authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] ResgisterRequestDto request)
        {
            try
            {
                var data = await _authService.RegisterAsync( request);
                return StatusCode(201, new ApiResponseDto<RegisterResponseDto>
                {
                    Success = true,
                    Message = "Account created successfully",
                    Data = data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseDto<object>
                {
                    Success = false,
                    Message = ex.Message,
                    Errors = new[] { new { field = "registration", message = ex.Message } }
                });
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginrequest)
        {
            try
            {
                var data = await _authService.LoginAsync(loginrequest);
                return Ok(new ApiResponseDto<LoginResponseDto>
                {
                    Success = true,
                    Message = "Login successful",
                    Data = data
                });
            }
            catch (Exception ex)
            {
                return Unauthorized(new ApiResponseDto<object>
                {
                    Success = false,
                    Message = ex.Message,
                    Errors = null
                });
            }
        }

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestdDTO request)
        {
            try
            {
                var data = await _authService.RefreshTokenAsync(request);
                return Ok(new ApiResponseDto<TokenResponseDto>
                {
                    Success = true,
                    Message = "Token refreshed successfully",
                    Data = data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseDto<object>
                {
                    Success = false,
                    Message = ex.Message,
                    Errors = null
                });
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

                await _authService.LogoutAsync(Guid.Parse(userIdClaim));
                return Ok(new ApiResponseDto<object>
                {
                    Success = true,
                    Message = "Logged out successfully",
                    Data = null
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseDto<object>
                {
                    Success = false,
                    Message = ex.Message,
                    Errors = null
                });
            }
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgetPasswordDto request)
        {
            try
            {
                await _authService.ForgotPasswordAsync(request);
                return Ok(new ApiResponseDto<object>
                {
                    Success = true,
                    Message = "Reset instructions sent to your email or phone",
                    Data = null
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseDto<object>
                {
                    Success = false,
                    Errors = null
                });
            }
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto request)
        {
            try
            {
                await _authService.ResetPasswordAsync(request);
                return Ok(new ApiResponseDto<object>
                {
                    Success = true,
                    Message = "Password reset successfully",
                    Data = null
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseDto<object>
                {
                    Success = false,
                    Message = ex.Message,
                    Errors = null
                });
            }
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMe()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

                var data = await _authService.GetProfileAsync(Guid.Parse(userIdClaim));
                return Ok(new ApiResponseDto<UserProfileResponseDto>
                {
                    Success = true,
                    Message = "Profile fetched successfully",
                    Data = data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseDto<object>
                {
                    Success = false,
                    Message = ex.Message,
                    Errors = null
                });
            }
        }

        [HttpPut("me")]
        [Authorize]
        public async Task<IActionResult> UpdateMe([FromBody] UpdateProfileRequestDto request)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

                var userId = Guid.Parse(userIdClaim);
                await _authService.UpdateProfileAsync(userId, request);

                return Ok(new ApiResponseDto<object>
                {
                    Success = true,
                    Message = "Profile updated",
                    Data = new { userId = userId, updatedAt = DateTime.UtcNow }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseDto<object>
                {
                    Success = false,
                    Message = ex.Message,
                    Errors = null
                });
            }
        }
    }
}
    