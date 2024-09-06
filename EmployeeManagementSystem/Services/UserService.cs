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
            var sql = "SELECT * FROM Users";
            using var connection = _dbContext.CreateConnection();
            var users = await connection.QueryAsync<User>(sql);
            foreach (var user1 in users)
            {
                // Decrypt sensitive fields
                user1.Username = EncryptionHelper.Decrypt(user1.Username);
                if(user1.Username == user.Username)
                {
                    return false;
                }
            }
           

            // Hash the password
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
            user.PasswordHash = passwordHash;

            // Encrypt sensitive fields
            user.Username = EncryptionHelper.Encrypt(user.Username);
            user.Address = EncryptionHelper.Encrypt(user.Address);
            user.Phone = EncryptionHelper.Encrypt(user.Phone);
            user.Email = EncryptionHelper.Encrypt(user.Email);
            user.Age = EncryptionHelper.Encrypt(user.Age);
            user.Position = EncryptionHelper.Encrypt(user.Position);
            user.Salary = EncryptionHelper.Encrypt(user.Salary);

            // Insert the user into the database
            var sqlInsert = "INSERT INTO Users (Username, PasswordHash, Role, Address, Phone, Email, Age, Position, Salary, DepartmentId) VALUES (@Username, @PasswordHash, @Role, @Address, @Phone, @Email, @Age, @Position, @Salary, @DepartmentId)";
            var result = await connection.ExecuteAsync(sqlInsert, user);
            return result > 0;
        }

        public async Task<User> AuthenticateUserAsync(string username, string password)
        {
            var sql = "SELECT * FROM Users";
            using var connection = _dbContext.CreateConnection();
            var users = await connection.QueryAsync<User>(sql);
            foreach (var user in users)
            {
                // Decrypt sensitive fields
                user.Username = EncryptionHelper.Decrypt(user.Username);
                user.Email = EncryptionHelper.Decrypt(user.Email);

                if (user.Username== username)
                {
                    bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
                    if (isPasswordValid)
                    {
                        return user;
                    }
                    //else
                    //{
                    //    return null;
                    //}
                }
                //else
                //{
                //    return null;
                //}

            }


            //if (user == null)
            //    return null;

            //bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            //if (!isPasswordValid)
            //    return null;

            // Decrypt sensitive fields
            

            return null;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            var sql = "SELECT * FROM Users";
            using var connection = _dbContext.CreateConnection();
            var users = await connection.QueryAsync<User>(sql);
            foreach (var user in users)
            {
                // Decrypt sensitive fields
                user.Username = EncryptionHelper.Decrypt(user.Username);
                user.Address = EncryptionHelper.Decrypt(user.Address);
                user.Phone = EncryptionHelper.Decrypt(user.Phone);
                user.Email = EncryptionHelper.Decrypt(user.Email);
                user.Age = EncryptionHelper.Decrypt(user.Age);
                user.Position = EncryptionHelper.Decrypt(user.Position);
                user.Salary = EncryptionHelper.Decrypt(user.Salary);
            }
            return users;
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            var sql = "SELECT * FROM Users WHERE Id = @UserId";
            using var connection = _dbContext.CreateConnection();
            var user = await connection.QueryFirstOrDefaultAsync<User>(sql, new { UserId = userId });
            if (user != null)
            {
                // Decrypt sensitive fields
                user.Username = EncryptionHelper.Decrypt(user.Username);
                user.Address = EncryptionHelper.Decrypt(user.Address);
                user.Phone = EncryptionHelper.Decrypt(user.Phone);
                user.Email = EncryptionHelper.Decrypt(user.Email);
                user.Age = EncryptionHelper.Decrypt(user.Age);
                user.Position = EncryptionHelper.Decrypt(user.Position);
                user.Salary = EncryptionHelper.Decrypt(user.Salary);
            }
            return user;
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
            var users = await connection.QueryAsync<User>(sql, new { Id = id });
            foreach (var user in users)
            {
                // Decrypt sensitive fields
                user.Username = EncryptionHelper.Decrypt(user.Username);
                user.Address = EncryptionHelper.Decrypt(user.Address);
                user.Phone = EncryptionHelper.Decrypt(user.Phone);
                user.Email = EncryptionHelper.Decrypt(user.Email);
                user.Age = EncryptionHelper.Decrypt(user.Age);
                user.Position = EncryptionHelper.Decrypt(user.Position);
                user.Salary = EncryptionHelper.Decrypt(user.Salary);
            }
            return users;
        }

        public async Task EditUser(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User parameter cannot be null");
            }

            if (_dbContext == null)
            {
                throw new InvalidOperationException("Database context is not initialized");
            }

            // Encrypt sensitive fields
            user.Username = EncryptionHelper.Encrypt(user.Username);
            user.Address = EncryptionHelper.Encrypt(user.Address);
            user.Phone = EncryptionHelper.Encrypt(user.Phone);
            user.Email = EncryptionHelper.Encrypt(user.Email);
            user.Age = EncryptionHelper.Encrypt(user.Age);
            user.Position = EncryptionHelper.Encrypt(user.Position);
            user.Salary = EncryptionHelper.Encrypt(user.Salary);

            var sql = "UPDATE Users SET Username = @Username, Role = @Role, Address = @Address, Phone = @Phone, Email = @Email, Age = @Age, Position = @Position, Salary = @Salary, DepartmentId = @DepartmentId WHERE Id = @Id";
            using var connection = _dbContext.CreateConnection();
            await connection.ExecuteAsync(sql, user);
        }
    }


}
