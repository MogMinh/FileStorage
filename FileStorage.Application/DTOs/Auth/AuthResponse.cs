namespace FileStorage.Application.DTOs.Auth
{
    public class AuthResponse
    {
        public bool Success { get; set; }
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public string? Message { get; set; }
        public string? ErrorCode { get; set; }
        public int UserId { get; set; }
        public string Role { get; set; } = string.Empty;
    }
}