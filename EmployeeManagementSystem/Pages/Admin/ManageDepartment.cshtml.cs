using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Cryptography;
using System.Text;

namespace EmployeeManagementSystem.Pages.Admin
{
    [Authorize(Policy = "AdminOnly")]


    public class ManageDepartmentModel : PageModel
    {
        private readonly IDepartmentService _departmentService;
        private static readonly string SecretKey = Environment.GetEnvironmentVariable("HMAC_SECRET_KEY");

        public ManageDepartmentModel (IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        public IEnumerable<Department> Departments { get; set; }

        [BindProperty]
        public Department Department { get; set; }

        public bool IsEditMode { get; set; }

        public async Task OnGetAsync()
        {
            var sessionId = HttpContext.Request.Cookies["SessionId"];
            var sessionHmac = HttpContext.Request.Cookies["SessionHmac"];

            if (sessionId != null && sessionHmac != null && VerifyHmac(sessionId, sessionHmac))
            {
                Departments = await _departmentService.GetAllDepartmentsAsync();
            }
            else
            {
                RedirectToPage("/Account/Login");
            }
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            if (!ModelState.IsValid)
            {
                Departments = await _departmentService.GetAllDepartmentsAsync();
                return Page();
            }

            await _departmentService.AddDepartmentAsync(Department);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditAsync(int id)
        {
            Department = await _departmentService.GetDepartmentByIdAsync(id);
            IsEditMode = true;
            Departments = await _departmentService.GetAllDepartmentsAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostUpdateAsync()
        {
            if (!ModelState.IsValid)
            {
                Departments = await _departmentService.GetAllDepartmentsAsync();
                return Page();
            }

            await _departmentService.UpdateDepartmentAsync(Department);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            await _departmentService.DeleteDepartmentAsync(id);
            return RedirectToPage();
        }
        public async Task<IActionResult> OnPostCancelAsync(int id)
        {
            return RedirectToPage();
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
