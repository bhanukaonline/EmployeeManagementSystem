using AttendanceAdmin.Context;
using BCrypt.Net;
using Dapper;
using EmployeeManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EmployeeManagementSystem.Services
{
    public interface IUserService
    {
        Task<bool> RegisterUserAsync(User user, string password);
        Task<User> AuthenticateUserAsync(string username, string password);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> GetUserByIdAsync(int userId);
        Task<IActionResult> DeleteUserAsync(int userId);
        Task<IEnumerable<User>> GetEmployeesInDepartment(int id);
        Task EditUser(User User);
     }
    public class UserService : IUserService
    {
        private readonly DapperDbContext _dbContext;

        public UserService(DapperDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> RegisterUserAsync(User user, string password)
        {
            // Check if username already exists
            var sqlCheck = "SELECT COUNT(1) FROM Users WHERE Username = @Username";
            using var connection = _dbContext.CreateConnection();
            var existingUser = await connection.ExecuteScalarAsync<int>(sqlCheck, new { user.Username });
            if (existingUser > 0)
            {
                return false; // Username already exists
            }

            // Hash the password
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
            user.PasswordHash = passwordHash;

            // Insert the user into the database
            var sqlInsert = "INSERT INTO Users (Username, PasswordHash, Role , Address, Phone , Email, Age, Position, Salary, DepartmentId) VALUES (@Username, @PasswordHash, @Role ,@Address, @Phone , @Email, @Age, @Position, @Salary, @DepartmentId)";
            var result = await connection.ExecuteAsync(sqlInsert, user);
            return result > 0;
        }

        public async Task<User> AuthenticateUserAsync(string username, string password)
        {
            var sql = "SELECT * FROM Users WHERE Username = @Username";
            using var connection = _dbContext.CreateConnection();
            var user = await connection.QuerySingleOrDefaultAsync<User>(sql, new { Username = username });
            if (user == null)
                return null;

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            if (!isPasswordValid)
                return null;

            return user;
        }
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            var sql = "SELECT * FROM Users";
            using var connection = _dbContext.CreateConnection();
            return await connection.QueryAsync<User>(sql);
        }
        public async Task<User> GetUserByIdAsync(int userId)
        {
            var sql = "SELECT * FROM Users WHERE Id = @UserId";
            using var connection = _dbContext.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<User>(sql, new { UserId = userId });
        }
        public async Task<IActionResult> DeleteUserAsync(int userId)
        {
            var sql = "DELETE FROM Users WHERE Id = @UserId";
            using var connection = _dbContext.CreateConnection();
            await connection.ExecuteAsync(sql, new { UserId = userId });
            return new OkResult();
        }
        public async Task<IEnumerable<User>> GetEmployeesInDepartment(int id)
        {
            var sql = @"
                        SELECT u.* 
                        FROM Users u
                        INNER JOIN Department d ON u.DepartmentId = d.DepartmentId
                        WHERE d.ManagerId = @Id";
            using var connection = _dbContext.CreateConnection();
            return await connection.QueryAsync<User>(sql, new { Id = id });
        }
        public async Task EditUser(User User)
        {
            if (User == null)
            {
                throw new ArgumentNullException(nameof(User), "User parameter cannot be null");
            }

            if (_dbContext == null)
            {
                throw new InvalidOperationException("Database context is not initialized");
            }

            var sql = "UPDATE Users SET Username = @Username, Role = @Role, Address = @Address, Phone = @Phone, Email = @Email, Age = @Age, Position = @Position, Salary = @Salary, DepartmentId = @DepartmentId WHERE Id = @Id";
            using var connection = _dbContext.CreateConnection();
            await connection.ExecuteAsync(sql, User);
        }


    }
    
}
