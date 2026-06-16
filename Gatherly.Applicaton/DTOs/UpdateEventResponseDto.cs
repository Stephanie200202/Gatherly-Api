using Gatherly.Application.DTOs.Events;

namespace Gatherly.Application.DTOs.Events
{
    public class UpdateEventResponseDto
    {
        public Guid EventId { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}