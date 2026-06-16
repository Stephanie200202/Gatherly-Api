using System;
using System.ComponentModel.DataAnnotations;

namespace Gatherly.Application.DTOs.Events
{
    public class CreateEventRequestDto
    {
        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Category { get; set; } = string.Empty; // Conference, Concert, Religious, Campus, etc.

        [Required]
        public string Venue { get; set; } = string.Empty;

        [Required]
        public string Date { get; set; } = string.Empty; // Format: YYYY-MM-DD

        [Required]
        public string StartTime { get; set; } = string.Empty; // Format: HH:mm

        public string? EndTime { get; set; }

        [MaxLength(2000)]
        public string? Description { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Capacity { get; set; }

        [Required]
        public string Visibility { get; set; } = "Public"; // Public or Private

        public bool AllowReEntry { get; set; } = false;

        public bool VipEnabled { get; set; } = false;

        public string? RsvpDeadline { get; set; } // ISO 8601
    }
}