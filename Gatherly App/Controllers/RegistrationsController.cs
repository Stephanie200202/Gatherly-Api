using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Gatherly.Application.IServices;
using Gatherly.Application.DTOs.Registrations;
using Gatherly.Application.DTOs.Common;

namespace Gatherly.Presentation.Controllers
{
    [ApiController]
    [Route("api")]
    public class RegistrationsController : ControllerBase
    {
        private readonly IRegistrationService _registrationService;

        public RegistrationsController(IRegistrationService registrationService)
        {
            _registrationService = registrationService;
        }

        private Guid GetAuthenticatedUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new UnauthorizedAccessException("Authentication token profile data unverified.");
            }
            return Guid.Parse(userIdClaim);
        }

        //[HttpPost("events/{eventId}/register")]
        ////[Authorize(Roles = "Attendee")]
        //public async Task<IActionResult> RegisterForEvent(Guid eventId, [FromBody] RegisterEventRequestDto requestDto)
        //{
        //    try
        //    {
        //        Guid userId = GetAuthenticatedUserId();
        //        var resultDto = await _registrationService.RegisterForEventAsync(eventId, userId, requestDto);
        //        var envelopeDto = ApiResponseEnvelopeDto<RegisterEventResponseDto>.CreateSuccess(resultDto, "Event registration processing successful.");
        //        return StatusCode(StatusCodes.Status201Created, envelopeDto);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ApiResponseEnvelopeDto<object>.CreateError(ex.Message));
        //    }
        //}



        [HttpPost("events/{eventId}/register")]
        public async Task<IActionResult> RegisterForEvent(Guid eventId, [FromBody] RegisterEventRequestDto requestDto)
        {
            try
            {
                Guid? userId = null;

              
                try
                {
                    userId = GetAuthenticatedUserId();
                }
                catch
                {
                   
                    userId = null;
                }

                var resultDto = await _registrationService.RegisterForEventAsync(eventId, userId, requestDto);
                var envelopeDto = ApiResponseEnvelopeDto<RegisterEventResponseDto>.CreateSuccess(resultDto, "Event registration processing successful.");
                return StatusCode(StatusCodes.Status201Created, envelopeDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseEnvelopeDto<object>.CreateError(ex.Message));
            }
        }




        [HttpPost("registrations/{registrationId}/cancel")]
        //[Authorize(Roles = "Attendee")]
        public async Task<IActionResult> CancelRegistration(Guid registrationId)
        {
            try
            {
                Guid userId = GetAuthenticatedUserId();
                var resultDto = await _registrationService.CancelRegistrationAsync(registrationId, userId);
                return Ok(ApiResponseEnvelopeDto<CancelRegistrationResponseDto>.CreateSuccess(resultDto, "Your registration has been successfully cancelled."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseEnvelopeDto<object>.CreateError(ex.Message));
            }
        }

        [HttpGet("events/{eventId}/registrations")]
        [Authorize(Roles = "Organizer")]
        public async Task<IActionResult> GetEventRegistrations(Guid eventId)
        {
            try
            {
                Guid organizerId = GetAuthenticatedUserId();
                var listDto = await _registrationService.GetEventRegistrationsAsync(eventId, organizerId);
                return Ok(ApiResponseEnvelopeDto<IEnumerable<AttendeeRegistrationListItemDto>>.CreateSuccess(listDto));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponseEnvelopeDto<object>.CreateError(ex.Message));
            }
        }

        [HttpGet("my-registrations")]
        [Authorize(Roles = "Attendee")]
        public async Task<IActionResult> GetMyRegistrations()
        {
            try
            {
                Guid userId = GetAuthenticatedUserId();
                var listDto = await _registrationService.GetMyRegistrationsAsync(userId);
                return Ok(ApiResponseEnvelopeDto<IEnumerable<MyRegistrationListItemDto>>.CreateSuccess(listDto));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponseEnvelopeDto<object>.CreateError(ex.Message));
            }
        }

        [HttpPost("events/{eventId}/check-in")]
        [Authorize(Roles = "GateManager,Organizer")]
        public async Task<IActionResult> CheckInAttendee(Guid eventId, [FromBody] CheckInRequestDto requestDto)
        {
            try
            {
                Guid staffId = GetAuthenticatedUserId();
                var resultDto = await _registrationService.CheckInAttendeeAsync(eventId, requestDto, staffId);
                return Ok(ApiResponseEnvelopeDto<CheckInResponseDto>.CreateSuccess(resultDto, "Attendee access validation checked in successfully."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseEnvelopeDto<object>.CreateError(ex.Message));
            }
        }
    }
}