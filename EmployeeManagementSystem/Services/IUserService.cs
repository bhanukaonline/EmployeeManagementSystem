using EmployeeManagementSystem.Class;
using EmployeeManagementSystem.Models;
using System.Threading.Tasks;

namespace EmployeeManagementSystem.Services
{
    public interface IUserService
    {
        Task<bool> RegisterUserAsync(User user, string password);
        Task<User> AuthenticateUserAsync(string username, string password);
    }
}
