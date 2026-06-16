using System;

namespace Gatherly.Application.DTOs.Registrations
{
    public class RegisterEventResponseDto
    {
        public Guid RegistrationId { get; set; }
        public Guid EventId { get; set; }
        public Guid UserId { get; set; }
        public string TicketType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime RegisteredAt { get; set; }
        public string AccessCode { get; set; } = string.Empty; // Simulated unique pass-code
    }
}