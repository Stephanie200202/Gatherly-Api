using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gatherly.Application.DTOs.Feedback;

namespace Gatherly.Application.IServices
{
    public interface IFeedbackAnalyticsService
    {
        Task<SubmitFeedbackResponseDto> SubmitFeedbackAsync(Guid eventId, Guid userId, SubmitFeedbackRequestDto requestDto);
        Task<IEnumerable<FeedbackListItemDto>> GetEventFeedbackAsync(Guid eventId);
        Task<EventAnalyticsResponseDto> GetEventAnalyticsAsync(Guid eventId, Guid organizerId);
    }
}