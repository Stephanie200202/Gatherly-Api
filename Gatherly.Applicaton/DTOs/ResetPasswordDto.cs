using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Gatherly.Applicaton.DTOs
{
    public class ResetPasswordDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;


        [Required, StringLength(6, MinimumLength = 6)]
        public string TokenOrOtp { get; set; } = string.Empty;

        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
