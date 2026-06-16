using System;
using System.Collections.Generic;
using System.Text;

namespace Gatherly.Applicaton.DTOs
{
    public class LoginResponseDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public int ExpiresIn { get; set; }
        public UserLoginDto User { get; set; }
    }
}
