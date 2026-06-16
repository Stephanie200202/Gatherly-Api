using Gatherly.Application.Abstractions; // Double check this matches your DTO folder namespace
using Gatherly.Applicaton.DTOs;
using Gatherly.Domain.IRepositories;
using Gatherly.Infrastructure.Repositories;


namespace Gatherly.Infrastructure.HelperServices
{
    public class PasswordRecoveryService : IPasswordRecovery
    {
        //private readonly IPasswordRecoveryService _passwordRecoveryService;
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;

        public PasswordRecoveryService(IEmailService emailService, IUserRepository userRepository)
        {

            _emailService = emailService;
            _userRepository = userRepository;
            //_passwordRecoveryService = passwordRecoveryService;
        }

        // 1. FORGOT PASSWORD FLOW
        public async Task<bool> ForgotPasswordAsync(ForgetPasswordDto forgotPasswordDto)
        {
            var user = await _userRepository.GetByIdentifierAsync(forgotPasswordDto.Email);
            if (user == null)
            {
                return false;
            }

            // Generate a clean 6-digit code
            var random = new Random();
            string otpCode = random.Next(100000, 999999).ToString();

            // Save OTP info directly to database entity properties
            user.ResetToken = otpCode;
            user.ResetTokenExpiryTime = DateTime.UtcNow.AddMinutes(5);
            await _userRepository.UpdateAsync(user);

            // Build HTML email string
            string emailBody = $@"
            <h3>Your Password Reset Request</h3>
            <p>Your 6-digit verification code is: <b>{otpCode}</b></p>
            <p>This code will expire in 5 minutes.</p>";

            // Send out to the email service using your verified entity property
            await _emailService.SendEmailAsync(user.Email, "Your Password Reset Verification Code", emailBody);

            return true;
        }

        // 2. RESET PASSWORD FLOW
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

    }
}