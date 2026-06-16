using Gatherly.Applicaton.DTOs;

namespace Gatherly.Application.Abstractions
{
    public interface IPasswordRecovery
    {


        Task<bool> ForgotPasswordAsync(ForgetPasswordDto forgotPasswordDto);
        Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);

    }
}
