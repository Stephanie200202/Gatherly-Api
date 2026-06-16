using System;

namespace Gatherly.Application.DTOs.Registrations
{
    public class CancelRegistrationResponseDto
    {
        public Guid RegistrationId { get; set; }
        public string Status { get; set; } = "Cancelled";
        public DateTime CancelledAt { get; set; }
    }
}