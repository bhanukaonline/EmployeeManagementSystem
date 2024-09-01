using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EmployeeManagementSystem.Pages.Secure
{
    [Authorize(Policy = "AdminOnly")]
    public class AdminOnlyModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
