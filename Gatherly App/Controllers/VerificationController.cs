//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Authorization;
//using Gatherly.Applicaton.IServices;
//using Gatherly.Domain.Entities;

//namespace Gatherly.API.Controllers
//{
//    [ApiController]
//    // 🔏 Every path in this controller is now anchored to a specific event route
//    [Route("api/event/{eventId:guid}/verification")]
//    [Authorize(Roles = "Organizer,Admin")]
//    public class VerificationController : ControllerBase
//    {
//        private readonly IVerificationService _verificationService;

//        public VerificationController(IVerificationService verificationService)
//        {
//            _verificationService = verificationService;
//        }

       
//        /// <summary>
//        /// GET api/event/{eventId}/verification/search
//        /// Manual guest list lookup locked to this specific event.
//        /// </summary>
//        [HttpGet("search")]
//        public ActionResult<IEnumerable<object>> ManualSearch(
//            Guid eventId,
//            [FromQuery] string searchTerm,
//            [FromQuery] string gateId)
//        {
//            var results = _verificationService.ManualSearch(
//                eventId,
//                searchTerm ?? string.Empty,
//                gateId ?? "Gate-Manual"
//            );
//            return Ok(results);
//        }

//        /// <summary>
//        /// GET api/event/{eventId}/verification/logs
//        /// Retrieves the entire access control history log for this event.
//        /// </summary>
//        [HttpGet("logs")]
//        public ActionResult<IEnumerable<VerificationLog>> GetLog(Guid eventId)
//        {
//            var logs = _verificationService.GetLog(eventId);
//            return Ok(logs);
//        }

//        /// <summary>
//        /// GET api/event/{eventId}/verification/dashboard
//        /// Real-time dashboard capacity tracking for this venue instance.
//        /// </summary>
//        [HttpGet("dashboard")]
//        public IActionResult GetActiveAttendees(Guid eventId)
//        {
//            var activeDashboardData = _verificationService.GetActiveAttendees(eventId);
//            return Ok(activeDashboardData);
//        }
//    }

//    #region Minimal Scan Request Layout
   
//    #endregion
//}