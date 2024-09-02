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
        

        public async Task OnGetAsync()
        {
           
        }

        
    }
}
