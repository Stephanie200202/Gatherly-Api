using System;
using System.Collections.Generic;
using System.Text;

namespace Gatherly.Applicaton.DTOs
{
    public class LoginRequestDto
    {
        public string Identifier { get; set; }
        public string Password { get; set; }
    }
}
