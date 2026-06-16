using System;
using System.Collections.Generic;
using System.Text;

namespace Gatherly.Applicaton.DTOs
{
   public class UserLoginDto
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
}
