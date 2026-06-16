using System;

namespace Gatherly.Application.DTOs.Events
{
    public class PublishEventResponseDto
    {
        public Guid EventId { get; set; }
        public string Status { get; set; } = "Published";
        public string EventLink { get; set; } = string.Empty;
    }
}