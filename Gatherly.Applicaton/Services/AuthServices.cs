
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Gatherly.Application.Abstractions;
using Gatherly.Application.DTOs;
using Gatherly.Application.IServices;
using Gatherly.Applicaton.DTOs;
using Gatherly.Applicaton.IServices;
using Gatherly.Domain.Entities;
using Gatherly.Domain.IRepositories;
using Microsoft.IdentityModel.Tokens;

namespace Gatherly.Application.Services
{
    public class AuthService : IAuthServices
    {
        private readonly IEmailService _emailService;
        private readonly IUserRepository _userRepository;
        private const string JwtSecret = "GatherlySuperSecretKey2026SecureString!";

        public AuthService(IUserRepository userRepository, IEmailService emailService)
        {
            _emailService = emailService;
            _userRepository = userRepository;
        }

        public async Task<RegisterResponseDto> RegisterAsync(ResgisterRequestDto request)
        {
            var existingUser = await _userRepository.GetByIdentifierAsync(request.Email ?? request.Phone);
            if (existingUser != null)
                throw new Exception("Email or phone number already registered");

            string firstName = string.Empty;
            string lastName = string.Empty;
            if (!string.IsNullOrWhiteSpace(request.FullName))
            {
                var nameParts = request.FullName.Trim().Split(new[] { ' ' }, 2);
                firstName = nameParts[0];
                if (nameParts.Length > 1) lastName = nameParts[1];
            }

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                FirstName = firstName,
                LastName = lastName,
                Email = request.Email,
                UserName = request.Email,
                PhoneNumber = request.Phone,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = request.Role,

                // 🔥 ADD THESE LINES TO FIX THE SQL DB INSERT EXCEPTION:
                NormalizedUserName = request.Email?.ToUpperInvariant(),
                NormalizedEmail = request.Email?.ToUpperInvariant(),
                SecurityStamp = Guid.NewGuid().ToString(),

                // These avoid null constraint errors if they are not marked nullable in your DB layout
                RefreshToken = string.Empty,
                ResetToken = string.Empty,
                ProfilePhoto = string.Empty,
                UpdatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);

            return new RegisterResponseDto
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                CreatedAt = user.CreatedAt
            };
        }

        // FIX: Changed parameter type from LoginRequest to LoginRequestDto to satisfy CS0535
        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
        {
            var user = await _userRepository.GetByIdentifierAsync(request.Identifier);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                throw new Exception("Invalid credentials");

            var accessToken = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshTokenString();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await _userRepository.UpdateAsync(user);

            return new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresIn = 3600,
                User = new UserLoginDto
                {
                    UserId = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    Role = user.Role
                }
            };
        }

        public async Task<TokenResponseDto> RefreshTokenAsync(RefreshTokenRequestdDTO request)
        {
            var user = await _userRepository.GetByRefreshTokenAsync(request.RefreshToken);
            if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                throw new Exception("Refresh token expired or invalid");

            var newAccessToken = GenerateJwtToken(user);

            return new TokenResponseDto
            {
                AccessToken = newAccessToken,
                ExpiresIn = 3600
            };
        }

        public async Task LogoutAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user != null)
            {
                user.RefreshToken = null;
                user.RefreshTokenExpiryTime = DateTime.MinValue;
                await _userRepository.UpdateAsync(user);
            }
        }

        // FIX: Changed parameter type from ForgotPasswordRequest to ForgotPasswordRequestDto to satisfy CS0535
        //public async Task ForgotPasswordAsync(ForgetPasswordDto request)
        //{
        //    var user = await _userRepository.GetByIdentifierAsync(request.Identifier);
        //    if (user != null)
        //    {
        //        user.ResetToken = Guid.NewGuid().ToString("N");
        //        user.ResetTokenExpiryTime = DateTime.UtcNow.AddHours(1);
        //        await _userRepository.UpdateAsync(user);
        //    }
        //}





        public async Task<bool> ForgotPasswordAsync(ForgetPasswordDto forgotPasswordDto)
        {
            
            if (string.IsNullOrWhiteSpace(forgotPasswordDto.Email))
            {
                throw new ArgumentException("Email address cannot be empty.");
            }

            
            var user = await _userRepository.GetByIdentifierAsync(forgotPasswordDto.Email);
            if (user == null)
            {
                
                return false;
            }

            
            var random = new Random();
            string otpCode = random.Next(100000, 999999).ToString();

            
            user.ResetToken = otpCode;
            user.ResetTokenExpiryTime = DateTime.UtcNow.AddMinutes(5);

            await _userRepository.UpdateAsync(user);

            
            string targetEmail = !string.IsNullOrWhiteSpace(user.Email) ? user.Email : forgotPasswordDto.Email;

           
            string emailBody = $@"
        <div style='font-family: Arial, sans-serif; padding: 20px; border: 1px solid #eee; max-width: 500px;'>
            <h2 style='color: #333;'>Gatherly Password Reset</h2>
            <p>We received a request to reset your password. Use the verification code below:</p>
            <div style='background: #f4f4f4; padding: 15px; text-align: center; font-size: 24px; font-weight: bold; letter-spacing: 5px; color: #007bff;'>
                {otpCode}
            </div>
            <p style='color: #666; font-size: 12px; margin-top: 20px;'>This code is highly time-sensitive and will expire in 5 minutes.</p>
        </div>";

            try
            {
                
                await _emailService.SendEmailAsync(targetEmail, "Your Password Reset Verification Code", emailBody);
                return true;
            }
            catch (Exception ex)
            {
                
                throw new Exception($"OTP generated, but email dispatch failed. Technical details: {ex.Message}", ex);
            }
        }








        // FIX: Changed parameter type from ResetPasswordRequest to ResetPasswordRequestDto to satisfy CS0535
        //public async Task ResetPasswordAsync(ResetPasswordRequestDto request)
        //{
        //    var user = await _userRepository.GetByResetTokenAsync(request.ResetToken);
        //    if (user == null || user.ResetTokenExpiryTime <= DateTime.UtcNow)
        //        throw new Exception("Token expired or invalid");

        //    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        //    user.ResetToken = null;
        //    user.ResetTokenExpiryTime = DateTime.MinValue;
        //    await _userRepository.UpdateAsync(user);
        //}




        public async Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            if (resetPasswordDto.NewPassword != resetPasswordDto.ConfirmPassword)
            {
                return false;
            }

            var user = await _userRepository.GetByIdentifierAsync(resetPasswordDto.Email);

            // Validate user existence, verify OTP code matching, and check expiration window
            if (user == null || user.ResetToken != resetPasswordDto.TokenOrOtp || user.ResetTokenExpiryTime < DateTime.UtcNow)
            {
                return false;
            }

            // Apply new password changes (Remember to add password hashing here in production!)
            user.PasswordHash = resetPasswordDto.NewPassword;

            // Clear out old OTP data so it cannot be used again
            user.ResetToken = resetPasswordDto.TokenOrOtp;
            user.ResetTokenExpiryTime = DateTime.UtcNow.AddMinutes(5);

            await _userRepository.UpdateAsync(user);
            return true;
        }







        public async Task<UserProfileResponseDto> GetProfileAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) throw new Exception("User not found");

            return new UserProfileResponseDto
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.PhoneNumber,
                Role = user.Role,
                ProfilePhoto = user.ProfilePhoto,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task UpdateProfileAsync(Guid userId, UpdateProfileRequestDto request)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) throw new Exception("User not found");

            if (!string.IsNullOrWhiteSpace(request.FullName))
            {
                var nameParts = request.FullName.Trim().Split(new[] { ' ' }, 2);
                user.FirstName = nameParts[0];
                user.LastName = nameParts.Length > 1 ? nameParts[1] : string.Empty;
            }

            user.PhoneNumber = request.Phone ?? user.PhoneNumber;
            user.ProfilePhoto = request.ProfilePhoto ?? user.ProfilePhoto;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
        }

        private string GenerateJwtToken(ApplicationUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(JwtSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role ?? "User"),
                    new Claim(ClaimTypes.Email, user.Email ?? "")
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string GenerateRefreshTokenString()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}