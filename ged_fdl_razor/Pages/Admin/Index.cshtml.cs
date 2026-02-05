using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace ged_fdl_razor.Pages.Admin
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public bool CompteInvalide { get; set; }

        public IndexModel(IConfiguration configuration) => _configuration = configuration;

        public void OnGet()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                Response.Redirect("/Admin/Nav");
            }
        }

        public async Task<IActionResult> OnPostAsync(string username, string password, string? returnUrl)
        {
            string? adminLogin = _configuration["Auth:Admin:Login"];
            string? adminPassword = _configuration["Auth:Admin:Password"];

            if (username == adminLogin && password == adminPassword)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, "Admin")
                };

                var claimsIdentity = new ClaimsIdentity(claims, "AdminCookie");
                await HttpContext.SignInAsync("AdminCookie", new ClaimsPrincipal(claimsIdentity));

                return Redirect(returnUrl ?? "/Admin/Nav");
            }

            CompteInvalide = true;
            return Page();
        }

        public async Task<IActionResult> OnPostLogoutAsync()
        {
            await HttpContext.SignOutAsync("AdminCookie");
            return Redirect("/Admin/Index");
        }
    }
}
