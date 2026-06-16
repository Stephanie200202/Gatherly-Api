using System;

namespace Gatherly.Application.DTOs.Feedback
{
    public class FeedbackListItemDto
    {
        public Guid FeedbackId { get; set; }
        public string AttendeeName { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime SubmittedAt { get; set; }
    }
}