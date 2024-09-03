using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace EmployeeManagementSystem.Pages.Manager
{
    public class ManageEmployeeModel : PageModel
    {
        private readonly IUserService _userService;

        public ManageEmployeeModel(IUserService userService)
        {
            _userService = userService;
        }
        public IEnumerable<User> Employees { get; set; }
        public int id { get; set; }
        public int LoggedInUserId { get; set; }
        public async Task OnGetAsync()
        {
            LoggedInUserId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            Employees = await _userService.GetEmployeesInDepartment(LoggedInUserId);
        }
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            await _userService.DeleteUserAsync(id);
            return RedirectToPage();
        }
    }
}
