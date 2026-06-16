using System;

namespace Gatherly.Application.DTOs.Registrations
{
    public class AttendeeRegistrationListItemDto
    {
        public Guid RegistrationId { get; set; }
        public Guid UserId { get; set; }
        public string AttendeeName { get; set; } = string.Empty;
        public string AttendeeEmail { get; set; } = string.Empty;
        public string TicketType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime RegisteredAt { get; set; }
    }
}