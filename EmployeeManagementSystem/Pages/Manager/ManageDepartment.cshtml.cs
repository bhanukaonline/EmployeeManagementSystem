using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EmployeeManagementSystem.Pages.Manager
{
    [Authorize(Policy = "ManagerOnly")]

    public class ManageDepartmentModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
