using Dapper;
using FileStorage.Application.Interfaces;
using FileStorage.Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace FileStorage.Infrastructure.Repositories
{
    public class UserDapperRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserDapperRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<User>(
                "SP_GetUserWithQuota",
                new { UserId = id },
                commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<User>(
                "SELECT * FROM Users WHERE Username = @Username AND IsActive = 1",
                new { Username = username });
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<User>(
                "SELECT * FROM Users WHERE Email = @Email AND IsActive = 1",
                new { Email = email });
        }

        public async Task<bool> CreateUserAsync(User user)
        {
            using var connection = new SqlConnection(_connectionString);
            var sql = @"INSERT INTO Users (Username, Email, PasswordHash, Salt, Role, MaxStorageBytes) 
                       VALUES (@Username, @Email, @PasswordHash, @Salt, @Role, @MaxStorageBytes);
                       SELECT CAST(SCOPE_IDENTITY() as int);";

            var id = await connection.ExecuteScalarAsync<int>(sql, user);
            return id > 0;
        }

        public async Task<bool> UpdateUsedStorageAsync(int userId, long deltaBytes)
        {
            using var connection = new SqlConnection(_connectionString);
            var parameters = new { UserId = userId, DeltaBytes = deltaBytes, Success = 0 };

            await connection.ExecuteAsync("SP_CheckAndUpdateQuota", parameters,
                commandType: System.Data.CommandType.StoredProcedure);

            return parameters.Success == 1;
        }
    }
}