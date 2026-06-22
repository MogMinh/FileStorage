using FileStorage.Domain.Entities;

namespace FileStorage.Application.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user);
        int? ValidateToken(string token);
    }
}