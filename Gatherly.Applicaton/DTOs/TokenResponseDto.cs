using System;
using System.Collections.Generic;
using System.Text;

namespace Gatherly.Applicaton.DTOs
{
    public class TokenResponseDto
    {
        public string AccessToken { get; set; }
        public int ExpiresIn { get; set; }
    }
}
