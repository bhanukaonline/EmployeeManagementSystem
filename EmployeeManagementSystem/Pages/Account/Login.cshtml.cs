using EmployeeManagementSystem.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagementSystem.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly IEmailService _emailService; // Add email service
        private static readonly string SecretKey = Environment.GetEnvironmentVariable("HMAC_SECRET_KEY");

        public LoginModel(IUserService userService, IEmailService emailService)
        {
            _userService = userService;
            _emailService = emailService;
        }

        [BindProperty]
        public LoginInputModel Input { get; set; }

        [BindProperty]
        public string OTP { get; set; } // Add OTP property

        public string ErrorMessage { get; set; }

        public class LoginInputModel
        {
            [Required]
            public string Username { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            public bool RememberMe { get; set; }
        }

        public void OnGet()
        {
            // Display the login form
        }

        public async Task<IActionResult> OnPostAsync()
        {
            //if (!ModelState.IsValid)
            //    return Page();

            var user = await _userService.AuthenticateUserAsync(Input.Username, Input.Password);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                return Page();
            }

            // Generate and send OTP
            var otp = GenerateOtp();
            await _emailService.SendEmailAsync(user.Email, "Your OTP Code", $"Your OTP code is {otp}");

            // Store OTP in TempData (or use a more secure method)
            TempData["OTP"] = otp;
            TempData["UserId"] = user.Id;

            // Redirect to OTP verification page
            return RedirectToPage("/Account/VerifyOtp");
        }

        private string GenerateOtp()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }
    }

}
