using System;
using System.ComponentModel.DataAnnotations;

namespace Gatherly.Application.DTOs.Feedback
{
    public class SubmitFeedbackRequestDto
    {
        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5 stars.")]
        public int Rating { get; set; }

        [Required]
        [MaxLength(1000, ErrorMessage = "Comments cannot exceed 1000 characters.")]
        public string Comment { get; set; } = string.Empty;
    }
}