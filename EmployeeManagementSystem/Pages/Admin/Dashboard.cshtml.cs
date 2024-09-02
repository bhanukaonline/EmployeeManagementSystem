using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EmployeeManagementSystem.Pages.Admin
{
    [Authorize(Policy = "AdminOnly")]

    public class DashboardModel : PageModel
    {
        private readonly IDepartmentService _departmentService;

        public DashboardModel(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        public IEnumerable<Department> Departments { get; set; }

        [BindProperty]
        public Department Department { get; set; }

        public bool IsEditMode { get; set; }

        public async Task OnGetAsync()
        {
            Departments = await _departmentService.GetAllDepartmentsAsync();
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
    }
}
