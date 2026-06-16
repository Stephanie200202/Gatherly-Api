using System;
using System.Collections.Generic;
using System.Text;

namespace Gatherly.Applicaton.DTOs
{
    public class ApiResponseDto<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public object Errors { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
