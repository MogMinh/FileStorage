using FileStorage.Domain.Constants;

namespace FileStorage.Domain
{
    public class ApiResponse
    {
        public bool Success { get; set; }
        public string? ErrorCode { get; set; }
        public string? Message { get; set; }
        public object? Data { get; set; }

        public static ApiResponse Ok(object? data = null, string? message = null)
        {
            return new ApiResponse
            {
                Success = true,
                Data = data,
                Message = message
            };
        }

        public static ApiResponse Fail(string errorCode, string message)
        {
            return new ApiResponse
            {
                Success = false,
                ErrorCode = errorCode,
                Message = message
            };
        }
    }
}