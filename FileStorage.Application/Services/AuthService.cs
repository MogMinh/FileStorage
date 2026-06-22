using FileStorage.Application.DTOs.Auth;
using FileStorage.Application.Interfaces;
using FileStorage.Domain;
using FileStorage.Domain.Constants;
using FileStorage.Domain.Entities;
using System.Security.Cryptography;
using System.Text;

namespace FileStorage.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;  

        public AuthService(IUserRepository userRepository, IJwtService jwtService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            // Kiểm tra user tồn tại
            var existingUser = await _userRepository.GetByUsernameAsync(request.Username);
            if (existingUser != null)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Tên đăng nhập đã tồn tại",
                    ErrorCode = ErrorCodes.UserAlreadyExists
                };
            }

            // Hash password
            var salt = GenerateSalt();
            var passwordHash = HashPassword(request.Password, salt);

            var newUser = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = passwordHash,
                Salt = salt,
                Role = "User"
            };

            var success = await _userRepository.CreateUserAsync(newUser);

            if (!success)
            {
                return new AuthResponse { Success = false, Message = "Đăng ký thất bại", ErrorCode = ErrorCodes.InternalError };
            }

            var token = _jwtService.GenerateToken(newUser);

            return new AuthResponse
            {
                Success = true,
                Token = token,
                UserId = newUser.Id,
                Role = newUser.Role,
                Message = "Đăng ký thành công"
            };
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _userRepository.GetByUsernameAsync(request.Username);
            if (user == null)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Tài khoản hoặc mật khẩu không đúng",
                    ErrorCode = ErrorCodes.InvalidCredentials
                };
            }

            var hashedInput = HashPassword(request.Password, user.Salt);
            if (hashedInput != user.PasswordHash)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Tài khoản hoặc mật khẩu không đúng",
                    ErrorCode = ErrorCodes.InvalidCredentials
                };
            }

            var token = _jwtService.GenerateToken(user);

            return new AuthResponse
            {
                Success = true,
                Token = token,
                UserId = user.Id,
                Role = user.Role,
                Message = "Đăng nhập thành công"
            };
        }

        private string GenerateSalt()
        {
            byte[] saltBytes = new byte[16];
            RandomNumberGenerator.Fill(saltBytes);
            return Convert.ToBase64String(saltBytes);
        }

        private string HashPassword(string password, string salt)
        {
            using var sha256 = SHA256.Create();
            var saltedPassword = password + salt;
            var bytes = Encoding.UTF8.GetBytes(saltedPassword);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}