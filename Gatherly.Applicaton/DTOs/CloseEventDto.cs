using System;

namespace Gatherly.Application.DTOs.Events
{
    public class CloseEventResponseDto
    {
        public Guid EventId { get; set; }
        public string Status { get; set; } = "RegistrationClosed";
    }
}