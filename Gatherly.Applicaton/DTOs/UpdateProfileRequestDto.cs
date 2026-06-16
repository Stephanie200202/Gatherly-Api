using System;
using System.Collections.Generic;
using System.Text;

namespace Gatherly.Applicaton.DTOs
{
    public class UpdateProfileRequestDto
    {
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string ProfilePhoto { get; set; }
    }
}
