using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Gatherly.Applicaton.DTOs
{
    public class ForgetPasswordDto
    {
        [Required]
       public string Email { get; set; }= string.Empty;
    }
}
