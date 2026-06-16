using System;

namespace Gatherly.Application.DTOs.Registrations
{
    public class MyRegistrationListItemDto
    {
        public Guid RegistrationId { get; set; }
        public Guid EventId { get; set; }
        public string EventTitle { get; set; } = string.Empty;
        public string EventDate { get; set; } = string.Empty;
        public string Venue { get; set; } = string.Empty;
        public string TicketType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}