using System;
using System.Threading.Tasks;
using Gatherly.Application.DTOs.Events;
using Gatherly.Application.DTOs.Common;

namespace Gatherly.Application.IServices
{
    public interface IEventService
    {
        Task<CreateEventResponseDto> CreateEventAsync(CreateEventRequestDto requestDto, Guid organizerId);
        Task<PaginatedResponseDto<EventListItemDto>> GetPublicEventsAsync(int page, int pageSize, string? category, string? search, string? date);
        Task<EventDetailsResponseDto?> GetEventDetailsAsync(Guid eventId);
        Task<UpdateEventResponseDto> UpdateEventAsync(Guid eventId, UpdateEventRequestDto requestDto, Guid organizerId);
        Task<bool> CancelAndDeleteEventAsync(Guid eventId, Guid organizerId);
        Task<PublishEventResponseDto> PublishEventAsync(Guid eventId, Guid organizerId);
        Task<CloseEventResponseDto> CloseRegistrationAsync(Guid eventId, Guid organizerId);
        Task<PaginatedResponseDto<EventListItemDto>> GetOrganizerEventsAsync(Guid organizerId);
        Task<UploadBannerResponseDto> UploadBannerAsync(Guid eventId, byte[] fileBytes, string fileExtension, Guid organizerId);
        Task<bool> UpdateSettingsAsync(Guid eventId, UpdateSettingsRequestDto requestDto, Guid organizerId);
    }
}