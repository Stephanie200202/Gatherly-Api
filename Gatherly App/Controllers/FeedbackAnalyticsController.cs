using Gatherly.Application.DTOs.Common;
using Gatherly.Application.DTOs.Feedback;
using Gatherly.Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Gatherly.Presentation.Controllers
{
    [ApiController]
    [Route("api")]
    public class FeedbackAnalyticsController : ControllerBase
    {
        private readonly IFeedbackAnalyticsService _feedbackAnalyticsService;

        public FeedbackAnalyticsController(IFeedbackAnalyticsService feedbackAnalyticsService)
        {
            _feedbackAnalyticsService = feedbackAnalyticsService;
        }

        private Guid GetAuthenticatedUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new UnauthorizedAccessException("The request does not contain valid identity credentials.");
            }
            return Guid.Parse(userIdClaim);
        }

        [HttpPost("events/{eventId}/feedback")]
        [Authorize(Roles = "Attendee")]
        public async Task<IActionResult> SubmitFeedback(Guid eventId, [FromBody] SubmitFeedbackRequestDto requestDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponseEnvelopeDto<object>.CreateError("Invalid input fields provided.", ModelState));
                }

                Guid userId = GetAuthenticatedUserId();
                var resultDto = await _feedbackAnalyticsService.SubmitFeedbackAsync(eventId, userId, requestDto);
                var responseEnvelope = ApiResponseEnvelopeDto<SubmitFeedbackResponseDto>.CreateSuccess(resultDto, "Thank you! Your feedback has been saved successfully.");

                return StatusCode(StatusCodes.Status201Created, responseEnvelope);
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseEnvelopeDto<object>.CreateError(ex.Message));
            }
        }

        [HttpGet("events/{eventId}/feedback")]
        [AllowAnonymous]
        public async Task<IActionResult> GetEventFeedback(Guid eventId)
        {
            try
            {
                var itemsListDto = await _feedbackAnalyticsService.GetEventFeedbackAsync(eventId);
                return Ok(ApiResponseEnvelopeDto<IEnumerable<FeedbackListItemDto>>.CreateSuccess(itemsListDto));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponseEnvelopeDto<object>.CreateError(ex.Message));
            }
        }

        [HttpGet("events/{eventId}/analytics")]
        [Authorize(Roles = "Organizer")]
        public async Task<IActionResult> GetEventAnalytics(Guid eventId)
        {
            try
            {
                Guid organizerId = GetAuthenticatedUserId();
                var analyticsResultDto = await _feedbackAnalyticsService.GetEventAnalyticsAsync(eventId, organizerId);
                return Ok(ApiResponseEnvelopeDto<EventAnalyticsResponseDto>.CreateSuccess(analyticsResultDto));
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode(StatusCodes.Status403Forbidden, ApiResponseEnvelopeDto<object>.CreateError("You are not authorized to access this event's metrics."));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponseEnvelopeDto<object>.CreateError(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponseEnvelopeDto<object>.CreateError(ex.Message));
            }
        }
    }
}