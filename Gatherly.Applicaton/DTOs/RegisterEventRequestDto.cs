using System;
using System.ComponentModel.DataAnnotations;

namespace Gatherly.Application.DTOs.Registrations
{
    public class RegisterEventRequestDto
    {
        [Required]
        public string TicketType { get; set; } = "Regular"; // Regular or VIP

        public string GuestEmail { get; set; } = string.Empty;
        public string GuestName { get; set; } = string.Empty;
        public string PaymentToken { get; set; } = null!;
    }
}