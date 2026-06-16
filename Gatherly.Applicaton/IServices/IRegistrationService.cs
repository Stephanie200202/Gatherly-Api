using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gatherly.Application.DTOs.Registrations;
using Gatherly.Application.DTOs.Common;

namespace Gatherly.Application.IServices
{
    public interface IRegistrationService
    {
        Task<RegisterEventResponseDto> RegisterForEventAsync(Guid eventId, Guid userId, RegisterEventRequestDto requestDto);
        Task<CancelRegistrationResponseDto> CancelRegistrationAsync(Guid registrationId, Guid userId);
        Task<IEnumerable<AttendeeRegistrationListItemDto>> GetEventRegistrationsAsync(Guid eventId, Guid organizerId);
        Task<IEnumerable<MyRegistrationListItemDto>> GetMyRegistrationsAsync(Guid userId);
        Task<CheckInResponseDto> CheckInAttendeeAsync(Guid eventId, CheckInRequestDto requestDto, Guid gateManagerId);
    }
}