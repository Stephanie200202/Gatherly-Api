using System;
using Microsoft.AspNetCore.Http;

namespace Gatherly.Application.DTOs.Events
{
    public class CreateEventResponseDto
    {
        public Guid EventId { get; set; }
        public decimal Price { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Status { get; set; } = "Draft";
        public string EventLink { get; set; } = string.Empty;
        public string? BannerUrl { get; set; }
        public string RegistrationUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}