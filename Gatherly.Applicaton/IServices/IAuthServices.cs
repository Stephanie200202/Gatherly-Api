using Gatherly.Applicaton.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gatherly.Applicaton.IServices
{
    public interface IAuthServices
    {
        Task<RegisterResponseDto> RegisterAsync(ResgisterRequestDto request);
        Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
        Task<TokenResponseDto> RefreshTokenAsync(RefreshTokenRequestdDTO request);
        Task LogoutAsync(Guid userId);
        Task<bool> ForgotPasswordAsync(ForgetPasswordDto forgotPasswordDto);
        Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
        Task<UserProfileResponseDto> GetProfileAsync(Guid userId);
        Task UpdateProfileAsync(Guid userId, UpdateProfileRequestDto request);
    }
}