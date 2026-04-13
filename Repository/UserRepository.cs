using Eventify_High_Performance_Event_Management_API.Repository.Interfaces;
using Eventify_High_Performance_Event_Management_API.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Eventify_High_Performance_Event_Management_API.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly string? _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            var sql = "SELECT * FROM Event.Users WHERE UserId = @Id";
            return await connection.QueryFirstAsync<User>(sql, new { Id = id });
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<User>("SELECT * FROM Users");
        }
    }
}