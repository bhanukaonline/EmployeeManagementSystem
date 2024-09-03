using EmployeeManagementSystem.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace EmployeeManagementSystem.Pages.Account
{
    public class VerifyOtpModel : PageModel
    {
        private readonly IUserService _userService;
        private static readonly string SecretKey = Environment.GetEnvironmentVariable("HMAC_SECRET_KEY");


        public VerifyOtpModel(IUserService userService)
        {
            _userService = userService;
        }

        [BindProperty]
        public string OTP { get; set; }

        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var storedOtp = TempData["OTP"] as string;
            var userId = TempData["UserId"] as int?;

            if (storedOtp == null || userId == null || OTP != storedOtp)
            {
                ModelState.AddModelError(string.Empty, "Invalid OTP.");
                return Page();
            }

            var user = await _userService.GetUserByIdAsync(userId.Value);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "User not found.");
                return Page();
            }

            // Create the user claims
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
            };
            var sessionId = user.Id.ToString();
            var hmac = GenerateHmac(sessionId);

            HttpContext.Response.Cookies.Append("SessionId", sessionId, new CookieOptions { HttpOnly = true, Secure = true });
            HttpContext.Response.Cookies.Append("SessionHmac", hmac, new CookieOptions { HttpOnly = true, Secure = true });


            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            // Redirect to different pages based on the user's role
            if (user.Role == "Admin")
            {
                return RedirectToPage("/Admin/Dashboard");
            }
            else if (user.Role == "Manager")
            {
                return RedirectToPage("/Manager/ManageEmployee");
            }
            else if (user.Role == "Employee")
            {
                return RedirectToPage("/Employee/Employee");
            }
            return RedirectToPage("/Account/Login");
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
