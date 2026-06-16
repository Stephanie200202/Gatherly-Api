using System;

namespace Gatherly.Application.DTOs.Common
{
    public class ApiResponseEnvelopeDto<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public object? Errors { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public static ApiResponseEnvelopeDto<T> CreateSuccess(T data, string message = "Request completed successfully")
        {
            return new ApiResponseEnvelopeDto<T>
            {
                Success = true,
                Message = message,
                Data = data,
                Errors = null
            };
        }

        public static ApiResponseEnvelopeDto<T> CreateError(string message, object? errors = null)
        {
            return new ApiResponseEnvelopeDto<T>
            {
                Success = false,
                Message = message,
                Data = default,
                Errors = errors
            };
        }
    }
}