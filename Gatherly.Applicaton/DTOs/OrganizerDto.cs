using System;

namespace Gatherly.Application.DTOs.Events
{
    public class OrganizerDto
    {
        public Guid UserId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}