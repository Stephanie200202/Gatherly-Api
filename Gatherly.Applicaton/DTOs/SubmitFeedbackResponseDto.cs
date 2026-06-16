using System;

namespace Gatherly.Application.DTOs.Feedback
{
    public class SubmitFeedbackResponseDto
    {
        public Guid FeedbackId { get; set; }
        public Guid EventId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime SubmittedAt { get; set; }
    }
}