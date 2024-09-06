using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace EmployeeManagementSystem.Pages.Manager
{
    [Authorize(Policy = "ManagerOnly")]

    public class ManageEmployeeModel : PageModel
    {
        private readonly IUserService _userService;
        private static readonly string SecretKey = Environment.GetEnvironmentVariable("HMAC_SECRET_KEY");


        public ManageEmployeeModel(IUserService userService)
        {
            _userService = userService;
        }
        public IEnumerable<User> Employees { get; set; }
        public int id { get; set; }
        public int LoggedInUserId { get; set; }
        public async Task OnGetAsync()
        {
            var sessionId = HttpContext.Request.Cookies["SessionId"];
            var sessionHmac = HttpContext.Request.Cookies["SessionHmac"];

            if (sessionId != null && sessionHmac != null && VerifyHmac(sessionId, sessionHmac))
            {
                LoggedInUserId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                Employees = await _userService.GetEmployeesInDepartment(LoggedInUserId);
            }
            else
            {
                RedirectToPage("/Account/Login");
            }
        }
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            await _userService.DeleteUserAsync(id);
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
