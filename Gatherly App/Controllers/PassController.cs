//using Gatherly.Application.IServices;
//using Gatherly.Applicaton.IServices;
//using Gatherly.Domain.Entities;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;

//namespace Gatherly.API.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    [Authorize] // 🔒 Secured globally via JWT
//    public class PassController : ControllerBase
//    {
//        private readonly IPassService _passService;

//        public PassController(IPassService passService)
//        {
//            _passService = passService;
//        }

//        [HttpGet("{id:guid}")]
//        [Authorize(Roles = "Attendee,Organizer,Admin")]
//        public ActionResult<Pass> GetById(Guid id)
//        {
//            var pass = _passService.GetPassDetails(id);
//            if (pass == null)
//            {
//                return NotFound(new { message = $"Pass with ID {id} was not found." });
//            }
//            return Ok(pass);
//        }

//        [HttpGet("registration/{registrationId:guid}")]
//        [Authorize(Roles = "Attendee,Organizer,Admin")]
//        public ActionResult<Pass> GetByRegistrationId(Guid registrationId)
//        {
//            var pass = _passService.GetPassByRegistration(registrationId);
//            if (pass == null)
//            {
//                return NotFound(new { message = $"No pass found for registration ID {registrationId}." });
//            }
//            return Ok(pass);
//        }


//        [HttpPost("{id:guid}/invalidate")]
//        [Authorize(Roles = "Organizer,Admin")]
//        public IActionResult Invalidate(Guid id, [FromBody] string reason)
//        {
//            if (string.IsNullOrWhiteSpace(reason))
//            {
//                return BadRequest(new { message = "An invalidation reason must be provided." });
//            }

//            var success = _passService.InvalidatePass(id, reason);
//            if (!success)
//            {
//                return NotFound(new { message = $"Failed to invalidate. Pass with ID {id} does not exist." });
//            }
//            return Ok(new { message = "Pass successfully status-updated to Invalidated." });
//        }

//        [HttpPost("{id:guid}/regenerate")]
//        [Authorize(Roles = "Organizer,Admin")]
//        public ActionResult<Pass> Regenerate(Guid id, [FromBody] string reason)
//        {
//            if (string.IsNullOrWhiteSpace(reason))
//            {
//                return BadRequest(new { message = "A regeneration reason must be provided." });
//            }

//            var newPass = _passService.RegeneratePass(id, reason);
//            if (newPass == null)
//            {
//                return NotFound(new { message = $"Failed to regenerate. Original ticket with ID {id} not found." });
//            }

//            // Standard REST compliance pointing cleanly to PassId
//            return CreatedAtAction(nameof(GetById), new { id = newPass.PassId }, newPass);
//        }
//    }
//}