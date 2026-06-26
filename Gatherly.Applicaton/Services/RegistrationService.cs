using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gatherly.Application.IServices;
using Gatherly.Application.DTOs.Registrations;
using Gatherly.Domain.Interfaces;
using Gatherly.Domain.Entities;

namespace Gatherly.Application.Services
{
    public class RegistrationService : IRegistrationService
    {
        private readonly IRegistrationRepository _registrationRepository;
        private readonly IEventRepository _eventRepository;

        public RegistrationService(IRegistrationRepository registrationRepository, IEventRepository eventRepository)
        {
            _registrationRepository = registrationRepository;
            _eventRepository = eventRepository;
        }

        //public async Task<RegisterEventResponseDto> RegisterForEventAsync(Guid eventId, Guid userId, RegisterEventRequestDto requestDto)
        //{
        //    try
        //    {
        //        var targetEvent = await _eventRepository.GetByIdAsync(eventId);
        //        if (targetEvent == null) throw new KeyNotFoundException("The requested event does not exist.");
        //        if (targetEvent.Status != "Published") throw new InvalidOperationException("Registration is only open for published events.");

        //        var existingReg = await _registrationRepository.GetByUserAndEventAsync(userId, eventId);
        //        if (existingReg != null && existingReg.Status != "Cancelled")
        //        {
        //            throw new InvalidOperationException("You are already registered for this event.");
        //        }

        //        int currentAttendeeCount = await _registrationRepository.GetCountByEventIdAsync(eventId);
        //        if (currentAttendeeCount >= targetEvent.Capacity)
        //        {
        //            throw new InvalidOperationException("This event has reached its maximum capacity capacity.");
        //        }

        //        var newRegistration = new Registration
        //        {
        //            RegistrationId = Guid.NewGuid(),
        //            EventId = eventId,
        //            UserId = userId,
        //            TicketType = requestDto.TicketType,
        //            Status = "Confirmed",
        //            RegisteredAt = DateTime.UtcNow
        //        };

        //        await _registrationRepository.AddAsync(newRegistration);

        //        return new RegisterEventResponseDto
        //        {
        //            RegistrationId = newRegistration.RegistrationId,
        //            EventId = newRegistration.EventId,
        //            UserId = newRegistration.UserId,
        //            TicketType = newRegistration.TicketType,
        //            Status = newRegistration.Status,
        //            RegisteredAt = newRegistration.RegisteredAt,
        //            AccessCode = $"GATH-{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}"
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new ApplicationException($"Registration processing failed: {ex.Message}", ex);
        //    }
        //}





        public async Task<RegisterEventResponseDto> RegisterForEventAsync(Guid eventId, Guid? userId, RegisterEventRequestDto requestDto)
        {
            try
            {
                // 1. Fetch the target event to check availability and ticket price
                var targetEvent = await _eventRepository.GetByIdAsync(eventId);
                if (targetEvent == null) throw new KeyNotFoundException("The requested event does not exist.");
                if (targetEvent.Status != "Published") throw new InvalidOperationException("Registration is only open for published events.");
                if (targetEvent.Status == "Closed") throw new InvalidOperationException("Registration for this event have closed");
                // 2. Validate Guest Checkout Info
                if (!userId.HasValue && string.IsNullOrWhiteSpace(requestDto.GuestEmail))
                {
                    throw new ArgumentException("Email is required for guest registration.");
                }

                // 3. Prevent Duplicate active registrations
                var existingReg = await _registrationRepository.GetByUserOrEmailAndEventAsync(userId, requestDto.GuestEmail, eventId);
                if (existingReg != null && existingReg.Status != "Cancelled")
                {
                    throw new InvalidOperationException("This user/email is already registered for this event.");
                }

                // 4. Capacity Limit Check
                int currentAttendeeCount = await _registrationRepository.GetCountByEventIdAsync(eventId);
                if (currentAttendeeCount >= targetEvent.Capacity)
                {
                    throw new InvalidOperationException("This event has reached its maximum capacity.");
                }

                // 5. PROCESS THE PAYMENT (Simulated or via a service)
                string paymentTransactionId = $"GATH-TX-{Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper()}";

                // If your event has a price tag, we simulate a successful payment execution here
                if (targetEvent.Price > 0 && string.IsNullOrWhiteSpace(requestDto.PaymentToken))
                {
                    throw new ArgumentException("Payment token is required for paid events.");
                }

                // 6. Map everything into the entity (including your new payment tracking fields!)
                var newRegistration = new Registration
                {
                    RegistrationId = Guid.NewGuid(),
                    EventId = eventId,
                    UserId = userId,
                    GuestName = userId.HasValue ? null : requestDto.GuestName,
                    GuestEmail = userId.HasValue ? null : requestDto.GuestEmail,
                    TicketType = requestDto.TicketType,
                    Status = "Confirmed",
                    RegisteredAt = DateTime.UtcNow,

                    // Here is where the magic happens:
                    AmountPaid = targetEvent.Price,
                    PaymentReference = targetEvent.Price > 0 ? paymentTransactionId : "FREE_EVENT"
                };

                // 7. Persist the single record to your database
                await _registrationRepository.AddAsync(newRegistration);

                // 8. Return response back to Swagger
                return new RegisterEventResponseDto
                {
                    RegistrationId = newRegistration.RegistrationId,
                    EventId = newRegistration.EventId,
                    UserId = newRegistration.UserId,
                    GuestEmail = newRegistration.GuestEmail,
                    TicketType = newRegistration.TicketType,
                    Status = newRegistration.Status,
                    RegisteredAt = newRegistration.RegisteredAt,
                    AccessCode = $"GATH-{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}"
                };
            }
            catch (Exception ex)
            {
                var innerError = ex.InnerException?.Message ?? ex.Message;
                throw new ApplicationException($"Registration processing failed: {innerError}", ex);
            }
        }




        public async Task<CancelRegistrationResponseDto> CancelRegistrationAsync(Guid registrationId, Guid userId)
        {
            try
            {
                var registration = await _registrationRepository.GetByIdAsync(registrationId);
                if (registration == null) throw new KeyNotFoundException("Registration record not found.");
                if (registration.UserId != userId) throw new UnauthorizedAccessException("You are not authorized to cancel this registration.");
                if (registration.Status == "Cancelled") throw new InvalidOperationException("This registration is already cancelled.");

                registration.Status = "Cancelled";
                await _registrationRepository.UpdateAsync(registration);

                return new CancelRegistrationResponseDto
                {
                    RegistrationId = registration.RegistrationId,
                    Status = registration.Status,
                    CancelledAt = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Cancellation failed: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<AttendeeRegistrationListItemDto>> GetEventRegistrationsAsync(Guid eventId, Guid organizerId)
        {
            try
            {
                var targetEvent = await _eventRepository.GetByIdAsync(eventId);
                if (targetEvent == null) throw new KeyNotFoundException("Event record not found.");
                if (targetEvent.OrganizerId != organizerId) throw new UnauthorizedAccessException("Access denied. You do not own this event resource.");

                var records = await _registrationRepository.GetByEventIdAsync(eventId);

                return records.Select(r => new AttendeeRegistrationListItemDto
                {
                    RegistrationId = r.RegistrationId,
                    UserId = r.UserId,
                    AttendeeName = "Registered Attendee", // Mapped from user repository context in full stack
                    AttendeeEmail = "attendee@gatherly.io",
                    TicketType = r.TicketType,
                    Status = r.Status,
                    RegisteredAt = r.RegisteredAt
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Failed to load event attendee registrations: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<MyRegistrationListItemDto>> GetMyRegistrationsAsync(Guid userId)
        {
            try
            {
                var items = await _registrationRepository.GetByUserIdAsync(userId);
                var registrationList = new List<MyRegistrationListItemDto>();

                foreach (var item in items)
                {
                    var targetEvent = await _eventRepository.GetByIdAsync(item.EventId);
                    registrationList.Add(new MyRegistrationListItemDto
                    {
                        RegistrationId = item.RegistrationId,
                        EventId = item.EventId,
                        EventTitle = targetEvent?.Title ?? "Unknown Event",
                        EventDate = targetEvent?.Date ?? string.Empty,
                        Venue = targetEvent?.Venue ?? string.Empty,
                        TicketType = item.TicketType,
                        Status = item.Status
                    });
                }

                return registrationList;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Failed to fetch personal registrations list: {ex.Message}", ex);
            }
        }

        public async Task<CheckInResponseDto> CheckInAttendeeAsync(Guid eventId, CheckInRequestDto requestDto, Guid gateManagerId)
        {
            try
            {
                var targetEvent = await _eventRepository.GetByIdAsync(eventId);
                if (targetEvent == null) throw new KeyNotFoundException("Event structure missing.");

                var registration = await _registrationRepository.GetByIdAsync(requestDto.RegistrationId);
                if (registration == null) throw new KeyNotFoundException("No active registration found matching the identifier.");
                if (registration.EventId != eventId) throw new InvalidOperationException("Registration parameter validation mismatch against the requested event.");
                if (registration.Status == "Cancelled") throw new InvalidOperationException("Cannot check in. Ticket status is cancelled.");
                if (registration.Status == "CheckedIn") throw new InvalidOperationException("Attendee has already been checked in.");

                registration.Status = "CheckedIn";
                registration.CheckedInAt = DateTime.UtcNow;
                await _registrationRepository.UpdateAsync(registration);

                return new CheckInResponseDto
                {
                    RegistrationId = registration.RegistrationId,
                    AttendeeName = "Verified Guest",
                    Status = registration.Status,
                    CheckedInAt = registration.CheckedInAt.Value
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Gate operation check-in pipeline failed: {ex.Message}", ex);
            }
        }
    }
}