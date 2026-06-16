using Gatherly.Application.DTOs.Common;
using Gatherly.Application.DTOs.Events;
using Gatherly.Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Gatherly_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventsController(IEventService eventService)
        {
            _eventService = eventService;
        }

        private Guid GetAuthenticatedUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new UnauthorizedAccessException("Structural tracking token missing valid authentication claims context.");
            }
            return Guid.Parse(userIdClaim);
        }
        
       
        [HttpPost]
        [Authorize(Roles = "Organizer, Admin")]
        public async Task<IActionResult> CreateEvent([FromBody] CreateEventRequestDto requestDto)
        {
            try
            {
           
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponseEnvelopeDto<object>.CreateError("Validation failed", ModelState));
                }

          
                var organizerIdStr = User.FindFirst("sub")?.Value
                                     ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

              
                if (string.IsNullOrEmpty(organizerIdStr))
                {
                    var availableClaims = string.Join(", ", User.Claims.Select(c => $"{c.Type}:{c.Value}"));

                    return Unauthorized(ApiResponseEnvelopeDto<object>.CreateError(
                        $"User ID not found. Token actually contains these claims: [{availableClaims}]"
                    ));
                }

    
                Guid organizerId = Guid.Parse(organizerIdStr);

              
                var response = await _eventService.CreateEventAsync(requestDto, organizerId);
                var envelope = ApiResponseEnvelopeDto<CreateEventResponseDto>.CreateSuccess(response, "Event created successfully");

                return CreatedAtAction(nameof(GetEventById), new { eventId = response.EventId }, envelope);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponseEnvelopeDto<object>.CreateError(ex.Message));
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetEvents(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? category = null,
            [FromQuery] string? search = null,
            [FromQuery] string? date = null)
        {
            try
            {
                var response = await _eventService.GetPublicEventsAsync(page, pageSize, category, search, date);
                return Ok(ApiResponseEnvelopeDto<PaginatedResponseDto<EventListItemDto>>.CreateSuccess(response));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponseEnvelopeDto<object>.CreateError(ex.Message));
            }
        }

        [HttpGet("{eventId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetEventById(Guid eventId)
        {
            try
            {
                var response = await _eventService.GetEventDetailsAsync(eventId);
                if (response == null)
                {
                    return NotFound(ApiResponseEnvelopeDto<object>.CreateError("Event not found"));
                }

                if (response.Visibility == "Private")
                {
                    // Evaluate permission if private event
                    var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (string.IsNullOrEmpty(currentUserId) || Guid.Parse(currentUserId) != response.Organizer.UserId)
                    {
                        return DaylightForbiddenAccessError();
                    }
                }

                return Ok(ApiResponseEnvelopeDto<EventDetailsResponseDto>.CreateSuccess(response));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponseEnvelopeDto<object>.CreateError(ex.Message));
            }
        }

        [HttpPut("{eventId}")]
        [Authorize(Roles = "Organizer")]
        public async Task<IActionResult> UpdateEvent(Guid eventId, [FromBody] UpdateEventRequestDto requestDto)
        {
            try
            {
                Guid organizerId = GetAuthenticatedUserId();
                var response = await _eventService.UpdateEventAsync(eventId, requestDto, organizerId);
                return Ok(ApiResponseEnvelopeDto<UpdateEventResponseDto>.CreateSuccess(response, "Event updated successfully"));
            }
            catch (KeyNotFoundException) { return NotFound(ApiResponseEnvelopeDto<object>.CreateError("Event structure not located.")); }
            catch (UnauthorizedAccessException) { return DaylightForbiddenAccessError(); }
            catch (Exception ex) { return BadRequest(ApiResponseEnvelopeDto<object>.CreateError(ex.Message)); }
        }

        [HttpDelete("{eventId}")]
        [Authorize(Roles = "Organizer")]
        public async Task<IActionResult> DeleteEvent(Guid eventId)
        {
            try
            {
                Guid organizerId = GetAuthenticatedUserId();
                bool verificationResult = await _eventService.CancelAndDeleteEventAsync(eventId, organizerId);
                if (!verificationResult) return NotFound(ApiResponseEnvelopeDto<object>.CreateError("Target database structure absent."));

                return Ok(ApiResponseEnvelopeDto<object>.CreateSuccess(null!, "Event cancelled and deleted"));
            }
            catch (UnauthorizedAccessException) { return DaylightForbiddenAccessError(); }
            catch (Exception ex) { return StatusCode(StatusCodes.Status500InternalServerError, ApiResponseEnvelopeDto<object>.CreateError(ex.Message)); }
        }

        [HttpPatch("{eventId}/publish")]
        [Authorize(Roles = "Organizer")]
        public async Task<IActionResult> PublishEvent(Guid eventId)
        {
            try
            {
                Guid organizerId = GetAuthenticatedUserId();
                var response = await _eventService.PublishEventAsync(eventId, organizerId);
                return Ok(ApiResponseEnvelopeDto<PublishEventResponseDto>.CreateSuccess(response, "Event is now live"));
            }
            catch (UnauthorizedAccessException) { return DaylightForbiddenAccessError(); }
            catch (Exception ex) { return BadRequest(ApiResponseEnvelopeDto<object>.CreateError(ex.Message)); }
        }

        [HttpPatch("{eventId}/close")]
        [Authorize(Roles = "Organizer")]
        public async Task<IActionResult> CloseEvent(Guid eventId)
        {
            try
            {
                Guid organizerId = GetAuthenticatedUserId();
                var response = await _eventService.CloseRegistrationAsync(eventId, organizerId);
                return Ok(ApiResponseEnvelopeDto<CloseEventResponseDto>.CreateSuccess(response, "Registration closed"));
            }
            catch (UnauthorizedAccessException) { return DaylightForbiddenAccessError(); }
            catch (Exception ex) { return BadRequest(ApiResponseEnvelopeDto<object>.CreateError(ex.Message)); }
        }

        [HttpGet("my-events")]
        [Authorize(Roles = "Organizer")]
        public async Task<IActionResult> GetMyEvents()
        {
            try
            {
                Guid organizerId = GetAuthenticatedUserId();
                var response = await _eventService.GetOrganizerEventsAsync(organizerId);
                return Ok(ApiResponseEnvelopeDto<PaginatedResponseDto<EventListItemDto>>.CreateSuccess(response));
            }
            catch (Exception ex) { return StatusCode(StatusCodes.Status500InternalServerError, ApiResponseEnvelopeDto<object>.CreateError(ex.Message)); }
        }

        [HttpPost("{eventId}/banner")]
        [Authorize(Roles = "Organizer")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadBanner(Guid eventId, IFormFile banner)
        {
            try
            {
                if (banner == null || banner.Length == 0) return BadRequest(ApiResponseEnvelopeDto<object>.CreateError("File content payload validation failed."));
                if (banner.Length > 5 * 1024 * 1024) return BadRequest(ApiResponseEnvelopeDto<object>.CreateError("File sizes cannot scale past 5MB constraints."));

                var activeExtension = Path.GetExtension(banner.FileName).ToLower();
                if (activeExtension != ".jpg" && activeExtension != ".png" && activeExtension != ".jpeg")
                {
                    return BadRequest(ApiResponseEnvelopeDto<object>.CreateError("Invalid media parameters. Only JPG/PNG properties accepted."));
                }

                Guid organizerId = GetAuthenticatedUserId();
                using var payloadMemory = new MemoryStream();
                await banner.CopyToAsync(payloadMemory);

                var result = await _eventService.UploadBannerAsync(eventId, payloadMemory.ToArray(), activeExtension, organizerId);
                return Ok(ApiResponseEnvelopeDto<UploadBannerResponseDto>.CreateSuccess(result, "Banner image successfully applied."));
            }
            catch (UnauthorizedAccessException) { return DaylightForbiddenAccessError(); }
            catch (Exception ex) { return StatusCode(StatusCodes.Status500InternalServerError, ApiResponseEnvelopeDto<object>.CreateError(ex.Message)); }
        }

        [HttpPut("{eventId}/settings")]
        [Authorize(Roles = "Organizer")]
        public async Task<IActionResult> UpdateSettings(Guid eventId, [FromBody] UpdateSettingsRequestDto requestDto)
        {
            try
            {
                Guid organizerId = GetAuthenticatedUserId();
                bool executionState = await _eventService.UpdateSettingsAsync(eventId, requestDto, organizerId);
                if (!executionState) return BadRequest(ApiResponseEnvelopeDto<object>.CreateError("Settings compilation mismatch encountered."));

                return Ok(ApiResponseEnvelopeDto<object>.CreateSuccess(null!, "Event settings updated"));
            }
            catch (UnauthorizedAccessException) { return DaylightForbiddenAccessError(); }
            catch (Exception ex) { return StatusCode(StatusCodes.Status500InternalServerError, ApiResponseEnvelopeDto<object>.CreateError(ex.Message)); }
        }

        private ObjectResult DaylightForbiddenAccessError()
        {
            return StatusCode(StatusCodes.Status403Forbidden, ApiResponseEnvelopeDto<object>.CreateError("Authenticated but execution permissions are contextually invalid."));
        }
    }
}
