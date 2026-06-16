using System;
using System.ComponentModel.DataAnnotations;

namespace Gatherly.Application.DTOs.Registrations
{
    public class RegisterEventRequestDto
    {
        [Required]
        public string TicketType { get; set; } = "Regular"; // Regular or VIP
    }
}