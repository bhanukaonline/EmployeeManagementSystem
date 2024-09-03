using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace EmployeeManagementSystem.Pages.Account
{
    [Authorize(Policy = "AdminOrManager")]

    public class EditUserModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly IDepartmentService _departmentService;
        private static readonly string SecretKey = Environment.GetEnvironmentVariable("HMAC_SECRET_KEY");


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
            public string Age { get; set; }
            public string Position { get; set; }
            public string Salary { get; set; }
            [Required]
            public int DepartmentId { get; set; }


        }
        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }
        public User User { get; set; }

        public string LoggedInUserRole { get; set; }

        public IEnumerable<Department> Departments { get; set; }
        public async Task  OnGetAsync()
        {
            var sessionId = HttpContext.Request.Cookies["SessionId"];
            var sessionHmac = HttpContext.Request.Cookies["SessionHmac"];

            if (sessionId != null && sessionHmac != null && VerifyHmac(sessionId, sessionHmac))
            {
                Departments = await _departmentService.GetAllDepartmentsAsync();

                User = await _userService.GetUserByIdAsync(Id);
                LoggedInUserRole = HttpContext.User.FindFirstValue(ClaimTypes.Role);
            }
            else
            {
                RedirectToPage("/Account/Login");
            }


        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var user = new User
            {
                Id=Id,
                Username = Input.Username,
                Role = Input.Role,
                Address = Input.Address,
                Phone = Input.Phone,
                Email = Input.Email,
                Age = Input.Age,
                Position = Input.Position,
                Salary = Input.Salary,
                DepartmentId = Input.DepartmentId

            };

            //bool isRegistered = await _userService.RegisterUserAsync(user, Input.Password);
            //if (!isRegistered)
            //{
            //    ModelState.AddModelError(string.Empty, "Username already exists.");
            //    return Page();
            //}
            await _userService.EditUser(user);



            if(LoggedInUserRole== "Admin")
            {
                return RedirectToPage("/Account/Users");
            }
            else if(LoggedInUserRole == "Manager")
            {
                return RedirectToPage("/Manager/ManageEmployee");
            }
            return RedirectToPage("/Account/Users");

            // Optionally, redirect to login page after successful registration


        }
        private bool VerifyHmac(string data, string hmac)
        {
            var computedHmac = GenerateHmac(data);
            return hmac == computedHmac;
        }
        private string GenerateHmac(string data)
        {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(SecretKey)))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
                return Convert.ToBase64String(hash);
            }
        }
    }
}
