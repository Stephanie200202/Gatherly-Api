using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gatherly.Application.IServices;
using Gatherly.Application.DTOs.Feedback;
using Gatherly.Domain.Interfaces;
using Gatherly.Domain.Entities;

namespace Gatherly.Application.Services
{
    public class FeedbackAnalyticsService : IFeedbackAnalyticsService
    {
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IRegistrationRepository _registrationRepository;

        public FeedbackAnalyticsService(
            IFeedbackRepository feedbackRepository,
            IEventRepository eventRepository,
            IRegistrationRepository registrationRepository)
        {
            _feedbackRepository = feedbackRepository;
            _eventRepository = eventRepository;
            _registrationRepository = registrationRepository;
        }

        public async Task<SubmitFeedbackResponseDto> SubmitFeedbackAsync(Guid eventId, Guid userId, SubmitFeedbackRequestDto requestDto)
        {
            try
            {
                var targetEvent = await _eventRepository.GetByIdAsync(eventId);
                if (targetEvent == null) throw new KeyNotFoundException("The requested event was not found.");

                // Validate that the attendee actually registered for the event
                var registration = await _registrationRepository.GetByUserAndEventAsync(userId, eventId);
                if (registration == null)
                {
                    throw new UnauthorizedAccessException("You cannot submit feedback for an event you did not register for.");
                }

                // Prevent duplicates
                var existingFeedback = await _feedbackRepository.GetByUserAndEventAsync(userId, eventId);
                if (existingFeedback != null)
                {
                    throw new InvalidOperationException("You have already submitted feedback for this event.");
                }

                var feedbackEntity = new Feedback
                {
                    FeedbackId = Guid.NewGuid(),
                    EventId = eventId,
                    UserId = userId,
                    Rating = requestDto.Rating,
                    Comment = requestDto.Comment,
                    SubmittedAt = DateTime.UtcNow
                };

                await _feedbackRepository.AddAsync(feedbackEntity);

                return new SubmitFeedbackResponseDto
                {
                    FeedbackId = feedbackEntity.FeedbackId,
                    EventId = feedbackEntity.EventId,
                    Rating = feedbackEntity.Rating,
                    Comment = feedbackEntity.Comment,
                    SubmittedAt = feedbackEntity.SubmittedAt
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Feedback submission failed: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<FeedbackListItemDto>> GetEventFeedbackAsync(Guid eventId)
        {
            try
            {
                var feedbackItems = await _feedbackRepository.GetByEventIdAsync(eventId);

                return feedbackItems.Select(f => new FeedbackListItemDto
                {
                    FeedbackId = f.FeedbackId,
                    AttendeeName = "Anonymous Attendee", // Anonymous design layout choice
                    Rating = f.Rating,
                    Comment = f.Comment,
                    SubmittedAt = f.SubmittedAt
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Failed to load event feedback lists: {ex.Message}", ex);
            }
        }

        public async Task<EventAnalyticsResponseDto> GetEventAnalyticsAsync(Guid eventId, Guid organizerId)
        {
            try
            {
                var targetEvent = await _eventRepository.GetByIdAsync(eventId);
                if (targetEvent == null) throw new KeyNotFoundException("Event record not found.");
                if (targetEvent.OrganizerId != organizerId) throw new UnauthorizedAccessException("Access denied. You are not the organizer of this event.");

                var totalRegistrations = await _registrationRepository.GetByEventIdAsync(eventId);
                var totalFeedback = await _feedbackRepository.GetByEventIdAsync(eventId);

                int registrationCount = totalRegistrations.Count();
                int checkedInCount = totalRegistrations.Count(r => r.Status == "CheckedIn");
                int feedbackCount = totalFeedback.Count();

                double averageRating = feedbackCount > 0 ? totalFeedback.Average(f => f.Rating) : 0.0;
                double attendanceRate = registrationCount > 0 ? ((double)checkedInCount / registrationCount) * 100 : 0.0;

                return new EventAnalyticsResponseDto
                {
                    EventId = targetEvent.EventId,
                    EventTitle = targetEvent.Title,
                    TotalCapacity = targetEvent.Capacity,
                    TotalRegistrations = registrationCount,
                    TotalCheckedIn = checkedInCount,
                    AttendanceRatePercentage = Math.Round(attendanceRate, 2),
                    TotalFeedbackReceived = feedbackCount,
                    AverageRating = Math.Round(averageRating, 1)
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Failed to compute event performance analytics reporting: {ex.Message}", ex);
            }
        }
    }
}