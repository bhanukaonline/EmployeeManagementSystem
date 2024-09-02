using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem.Pages.Account
{
    public class EditUserModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly IDepartmentService _departmentService;

        public EditUserModel(IUserService userService, IDepartmentService departmentService)
        {
            _userService = userService;
            _departmentService = departmentService;
        }
        [BindProperty]
        public RegisterInputModel Input { get; set; }

        public string ErrorMessage { get; set; }

        public class RegisterInputModel
        {
            [Required]
            [StringLength(50, MinimumLength = 3)]
            public string Username { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
            public string Password { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "Passwords do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            public string Role { get; set; }
            public string Address { get; set; }
            public string Phone { get; set; }
            [Required]
            public string Email { get; set; }
            public int Age { get; set; }
            public string Position { get; set; }
            public int Salary { get; set; }
            [Required]
            public int DepartmentId { get; set; }


        }
        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }
        public User User { get; set; }
        public IEnumerable<Department> Departments { get; set; }
        public async Task  OnGetAsync()
        {
            Departments = await _departmentService.GetAllDepartmentsAsync();

            User = await _userService.GetUserByIdAsync(Id);
        }
    }
}
