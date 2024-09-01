﻿using AttendanceAdmin.Context;
using BCrypt.Net;
using Dapper;
using EmployeeManagementSystem.Class;
//using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Models;
using System.Threading.Tasks;

namespace EmployeeManagementSystem.Services
{
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
            var sqlInsert = "INSERT INTO Users (Username, PasswordHash, Role) VALUES (@Username, @PasswordHash, @Role)";
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
    }
}
