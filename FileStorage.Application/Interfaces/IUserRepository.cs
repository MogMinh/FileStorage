using FileStorage.Domain.Entities;

namespace FileStorage.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByEmailAsync(string email);
        Task<bool> CreateUserAsync(User user);
        Task<bool> UpdateUsedStorageAsync(int userId, long deltaBytes);
    }
}