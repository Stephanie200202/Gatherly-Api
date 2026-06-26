using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Gatherly.Application.DTOs.Common;
using Gatherly.Application.DTOs.Events;
using Gatherly.Application.IServices;
using Gatherly.Domain.Entities;
using Gatherly.Domain.Interfaces;
using Gatherly.Domain.layer;

namespace Gatherly.Application.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;

        public EventService(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public async Task<CreateEventResponseDto> CreateEventAsync(CreateEventRequestDto requestDto, Guid organizerId)
        {
            try
            {
                string? uniqueBannerUrl = null;
                Guid newEventId = Guid.NewGuid();

                // 👇 1. PROCESS THE FILE UPLOAD SIMULTANEOUSLY
                if (requestDto.BannerFile != null && requestDto.BannerFile.Length > 0)
                {
                    string fileExtension = Path.GetExtension(requestDto.BannerFile.FileName);

                    using (var memoryStream = new MemoryStream())
                    {
                        await requestDto.BannerFile.CopyToAsync(memoryStream);
                        byte[] fileBytes = memoryStream.ToArray();

                        // Emulated secure file storage processing engine pipeline logic combined here:
                        uniqueBannerUrl = $"https://cdn.gatherly.io/banners/{newEventId}_{Guid.NewGuid():N}{fileExtension}";

                        // If you wire up AWS S3 or Azure Blob storage later, you pass 'fileBytes' to it here
                    }
                }

                // 2. Map data fields directly into the Event entity model
                var newEvent = new Event
                {
                    EventId = newEventId,
                    Price = requestDto.Price,
                    Title = requestDto.Title,
                    Category = requestDto.Category,
                    Venue = requestDto.Venue,
                    Date = requestDto.Date,
                    StartTime = requestDto.StartTime,
                    EndTime = requestDto.EndTime,
                    Description = requestDto.Description,
                    Capacity = requestDto.Capacity,
                    Visibility = requestDto.Visibility,
                    AllowReEntry = requestDto.AllowReEntry,
                    VipEnabled = requestDto.VipEnabled,
                    RsvpDeadline = requestDto.RsvpDeadline,

                    // 👇 3. SAVE THE GENERATED URL PATH STRING HERE
                    BannerUrl = uniqueBannerUrl,

                    Status = "Draft",
                    OrganizerId = organizerId,
                    CreatedAt = DateTime.UtcNow
                };

                bool saved = await _eventRepository.AddAsync(newEvent);
                if (!saved)
                {
                    throw new Exception("Database failure encountered during structural write operation.");
                }

                string urlSlug = requestDto.Title.ToLower()
                    .Replace(" ", "-")
                    .Replace("'", "")
                    .Replace("\"", "");

                return new CreateEventResponseDto
                {
                    EventId = newEvent.EventId,
                    Title = newEvent.Title,
                    Status = newEvent.Status,
                    Price = newEvent.Price,

                    // 👇 4. RETURN THE URL PATH STRING BACK TO THE CLIENT
                    BannerUrl = newEvent.BannerUrl,

                    EventLink = $"https://gatherly.io/events/{urlSlug}-{newEvent.EventId}",
                    RegistrationUrl = $"https://gatherly.io/register/{urlSlug}-{newEvent.EventId}",
                    CreatedAt = newEvent.CreatedAt
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Structural creation failed: {ex.Message}", ex);
            }
        }


        public async Task<PaginatedResponseDto<EventListItemDto>> GetPublicEventsAsync(int page, int pageSize, string? category, string? search, string? date)
        {
            try
            {
                var (entities, totalCount) = await _eventRepository.GetPublicEventsAsync(page, pageSize, category, search, date);

                var items = entities.Select(e => new EventListItemDto
                {
                    EventId = e.EventId,
                    Title = e.Title,
                    Category = e.Category,
                    Venue = e.Venue,
                    Date = e.Date,
                    StartTime = e.StartTime,
                    BannerUrl = e.BannerUrl,
                    Capacity = e.Capacity,
                    RegisteredCount = 0, // In production, calculated via aggregation
                    Status = e.Status,
                    OrganizerName = e.Organizer != null ? e.Organizer.FullName : "Unknown Organizer"
                });

                return new PaginatedResponseDto<EventListItemDto>
                {
                    Items = items,
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Failed to query public lists: {ex.Message}", ex);
            }
        }

        public async Task<EventDetailsResponseDto?> GetEventDetailsAsync(Guid eventId)
        {
            try
            {
                var e = await _eventRepository.GetByIdAsync(eventId);
                if (e == null) return null;

                return new EventDetailsResponseDto
                {
                    EventId = e.EventId,
                    Title = e.Title,
                    Category = e.Category,
                    Venue = e.Venue,
                    Date = e.Date,
                    StartTime = e.StartTime,
                    EndTime = e.EndTime,
                    Description = e.Description ?? string.Empty,
                    Capacity = e.Capacity,
                    RegisteredCount = 0,
                    AvailableSpots = e.Capacity,
                    Status = e.Status,
                    Visibility = e.Visibility,
                    AllowReEntry = e.AllowReEntry,
                    VipEnabled = e.VipEnabled,
                    RsvpDeadline = e.RsvpDeadline,
                    BannerUrl = e.BannerUrl,
                    Organizer = new OrganizerDto { UserId = e.OrganizerId, Name=e.Organizer != null ? e.Organizer.FullName : "Unknown Organizer" },
                    CreatedAt = e.CreatedAt
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Failed to retrieve element details: {ex.Message}", ex);
            }
        }

        public async Task<UpdateEventResponseDto> UpdateEventAsync(Guid eventId, UpdateEventRequestDto requestDto, Guid organizerId)
        {
            try
            {
                var existing = await _eventRepository.GetByIdAsync(eventId) ?? throw new KeyNotFoundException("Target record structure cannot be verified.");
                if (existing.OrganizerId != organizerId) throw new UnauthorizedAccessException("Verification failed: Target context ownership mismatch.");
                if (existing.Status == "Cancelled") throw new InvalidOperationException("Action revoked: Target element is structurally locked.");

                if (requestDto.Title != null) existing.Title = requestDto.Title;
                if (requestDto.Category != null) existing.Category = requestDto.Category;
                if (requestDto.Venue != null) existing.Venue = requestDto.Venue;
                if (requestDto.Date != null) existing.Date = requestDto.Date;
                if (requestDto.StartTime != null) existing.StartTime = requestDto.StartTime;
                if (requestDto.EndTime != null) existing.EndTime = requestDto.EndTime;
                if (requestDto.Description != null) existing.Description = requestDto.Description;
                if (requestDto.Capacity != null) existing.Capacity = requestDto.Capacity.Value;
                if (requestDto.Visibility != null) existing.Visibility = requestDto.Visibility;
                if (requestDto.AllowReEntry != null) existing.AllowReEntry = requestDto.AllowReEntry.Value;
                if (requestDto.VipEnabled != null) existing.VipEnabled = requestDto.VipEnabled.Value;
                if (requestDto.RsvpDeadline != null) existing.RsvpDeadline = requestDto.RsvpDeadline;

                await _eventRepository.UpdateAsync(existing);

                return new UpdateEventResponseDto { EventId = existing.EventId, UpdatedAt = DateTime.UtcNow };
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Modification pipeline broken: {ex.Message}", ex);
            }
        }

        public async Task<bool> CancelAndDeleteEventAsync(Guid eventId, Guid organizerId)
        {
            try
            {
                var existing = await _eventRepository.GetByIdAsync(eventId);
                if (existing == null) return false;
                if (existing.OrganizerId != organizerId) throw new UnauthorizedAccessException("Ownership authorization failure.");

                existing.Status = "Cancelled";
                return await _eventRepository.UpdateAsync(existing);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Deletion operation aborted: {ex.Message}", ex);
            }
        }

        public async Task<PublishEventResponseDto> PublishEventAsync(Guid eventId, Guid organizerId)
        {
            try
            {
                var existing = await _eventRepository.GetByIdAsync(eventId) ?? throw new KeyNotFoundException("Event record not found.");
                if (existing.OrganizerId != organizerId) throw new UnauthorizedAccessException("Ownership validation failed.");
                if (existing.Status == "Published") throw new InvalidOperationException("Event status conflict: Target sequence is already active.");

                existing.Status = "Published";
                await _eventRepository.UpdateAsync(existing);

                return new PublishEventResponseDto
                {
                    EventId = existing.EventId,
                    Status = existing.Status,
                    EventLink = $"https://gatherly.io/events/{existing.EventId}"
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Publish pipeline failed: {ex.Message}", ex);
            }
        }

        public async Task<CloseEventResponseDto> CloseRegistrationAsync(Guid eventId, Guid organizerId)
        {
            try
            {
                var existing = await _eventRepository.GetByIdAsync(eventId) ?? throw new KeyNotFoundException("Event record not found.");
                if (existing.OrganizerId != organizerId) throw new UnauthorizedAccessException("Ownership validation failed.");

                existing.Status = "RegistrationClosed";
                await _eventRepository.UpdateAsync(existing);

                return new CloseEventResponseDto { EventId = existing.EventId, Status = existing.Status };
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Closure pipeline failed: {ex.Message}", ex);
            }
        }

        public async Task<PaginatedResponseDto<EventListItemDto>> GetOrganizerEventsAsync(Guid organizerId)
        {
            try
            {
                var entities = await _eventRepository.GetEventsByOrganizerAsync(organizerId);
                var items = entities.Select(e => new EventListItemDto
                {
                    EventId = e.EventId,
                    Title = e.Title,
                    Category = e.Category,
                    Venue = e.Venue,
                    Date = e.Date,
                    StartTime = e.StartTime,
                    Capacity = e.Capacity,
                    Status = e.Status,
                    RegisteredCount = 0
                }).ToList();

                return new PaginatedResponseDto<EventListItemDto>
                {
                    Items = items,
                    TotalCount = items.Count,
                    Page = 1,
                    PageSize = Math.Max(1, items.Count),
                    TotalPages = 1
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Failed to query organizer space data: {ex.Message}", ex);
            }
        }

        public async Task<UploadBannerResponseDto> UploadBannerAsync(Guid eventId, byte[] fileBytes, string fileExtension, Guid organizerId)
        {
            try
            {
                var existing = await _eventRepository.GetByIdAsync(eventId) ?? throw new KeyNotFoundException("Event structure missing.");
                if (existing.OrganizerId != organizerId) throw new UnauthorizedAccessException("Context authentication mismatched context.");

                // Emulated secure file storage processing engine pipeline
                string uniqueUrl = $"https://cdn.gatherly.io/banners/{eventId}_{Guid.NewGuid():N}{fileExtension}";
                existing.BannerUrl = uniqueUrl;

                await _eventRepository.UpdateAsync(existing);
                return new UploadBannerResponseDto { BannerUrl = uniqueUrl };
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"File persistence structure breakdown: {ex.Message}", ex);
            }
        }

        public async Task<bool> UpdateSettingsAsync(Guid eventId, UpdateSettingsRequestDto requestDto, Guid organizerId)
        {
            try
            {
                var existing = await _eventRepository.GetByIdAsync(eventId) ?? throw new KeyNotFoundException("Event structural mapping missed.");
                if (existing.OrganizerId != organizerId) throw new UnauthorizedAccessException("Access verification failed.");

                existing.AllowReEntry = requestDto.AllowReEntry;
                existing.VipEnabled = requestDto.VipEnabled;
                existing.Capacity = requestDto.Capacity;
                existing.Visibility = requestDto.Visibility;
                if (requestDto.RsvpDeadline != null) existing.RsvpDeadline = requestDto.RsvpDeadline;

                return await _eventRepository.UpdateAsync(existing);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Configuration adjustments failed: {ex.Message}", ex);
            }
        }
    }
}