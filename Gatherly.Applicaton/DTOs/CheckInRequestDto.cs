using System;
using System.ComponentModel.DataAnnotations;

namespace Gatherly.Application.DTOs.Registrations
{
    public class CheckInRequestDto
    {
        [Required]
        public Guid RegistrationId { get; set; }
    }
}