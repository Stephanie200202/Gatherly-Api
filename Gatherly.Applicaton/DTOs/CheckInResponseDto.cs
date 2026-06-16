using System;

namespace Gatherly.Application.DTOs.Registrations
{
    public class CheckInResponseDto
    {
        public Guid RegistrationId { get; set; }
        public string AttendeeName { get; set; } = string.Empty;
        public string Status { get; set; } = "CheckedIn";
        public DateTime CheckedInAt { get; set; }
    }
}