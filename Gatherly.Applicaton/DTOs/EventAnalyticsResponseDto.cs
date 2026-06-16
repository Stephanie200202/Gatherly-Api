using System;

namespace Gatherly.Application.DTOs.Feedback
{
    public class EventAnalyticsResponseDto
    {
        public Guid EventId { get; set; }
        public string EventTitle { get; set; } = string.Empty;
        public int TotalCapacity { get; set; }
        public int TotalRegistrations { get; set; }
        public int TotalCheckedIn { get; set; }
        public double AttendanceRatePercentage { get; set; }
        public int TotalFeedbackReceived { get; set; }
        public double AverageRating { get; set; }
    }
}