using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EmployeeManagementSystem.Pages.Account
{
    public class UsersModel : PageModel
    {
        private readonly IUserService _userService;

        public UsersModel(IUserService userService)
        {
            _userService = userService;
          
        }
        public IEnumerable<User> Users { get; set; }
        public async Task OnGetAsync()
        {
            Users = await _userService.GetAllUsersAsync();
        }
    }
}
