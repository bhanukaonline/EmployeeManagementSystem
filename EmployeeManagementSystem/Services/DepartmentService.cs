using AttendanceAdmin.Context;
using Dapper;
using EmployeeManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace EmployeeManagementSystem.Services
{
    public interface IDepartmentService
    {
        Task<IEnumerable<Department>> GetAllDepartmentsAsync();
        Task<Department> GetDepartmentByIdAsync(int departmentId);
        Task<int> AddDepartmentAsync(Department department);
        Task<int> UpdateDepartmentAsync(Department department);
        Task<int> DeleteDepartmentAsync(int departmentId);
    }

    public class DepartmentService : IDepartmentService
    {
        private readonly DapperDbContext _dbContext;

        public DepartmentService(DapperDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Department>> GetAllDepartmentsAsync()
        {
            var sql = "SELECT * FROM Department";
            using var connection = _dbContext.CreateConnection();

            return await connection.QueryAsync<Department>(sql);
        }

        public async Task<Department> GetDepartmentByIdAsync(int departmentId)
        {
            var sql = "SELECT * FROM Department WHERE DepartmentId = @DepartmentId";
            using var connection = _dbContext.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<Department>(sql, new { DepartmentId = departmentId });
        }

        public async Task<int> AddDepartmentAsync(Department department)
        {
            var sql = "INSERT INTO Department (Name, ManagerId) VALUES (@Name, @ManagerId)";
            using var connection = _dbContext.CreateConnection();

            return await connection.ExecuteAsync(sql, department);
        }

        public async Task<int> UpdateDepartmentAsync(Department department)
        {
            var sql = "UPDATE Department SET Name = @Name, ManagerId = @ManagerId WHERE DepartmentId = @DepartmentId";
            using var connection = _dbContext.CreateConnection();

            return await connection.ExecuteAsync(sql, department);
        }

        public async Task<int> DeleteDepartmentAsync(int departmentId)
        {
            var sql = "DELETE FROM Department WHERE DepartmentId = @DepartmentId";
            using var connection = _dbContext.CreateConnection();

            return await connection.ExecuteAsync(sql, new { DepartmentId = departmentId });

        }
    }

}
