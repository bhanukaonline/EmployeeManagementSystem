using EmployeeManagementSystem.Models;
using System.Threading.Tasks;

namespace EmployeeManagementSystem.Services
{
    public interface IUserService
    {
        Task<bool> RegisterUserAsync(User user, string password);
        Task<User> AuthenticateUserAsync(string username, string password);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> GetUserByIdAsync(int userId);
    }
}
