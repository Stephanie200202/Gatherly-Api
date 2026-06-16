using System;
using System.Collections.Generic;
using System.Text;

namespace Gatherly.Applicaton.DTOs
{
    public class UserProfileResponseDto
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Role { get; set; }
        public string ProfilePhoto { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
