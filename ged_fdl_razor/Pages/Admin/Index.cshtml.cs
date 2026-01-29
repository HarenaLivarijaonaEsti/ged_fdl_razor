using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Configuration;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;

namespace ged_fdl_razor.Pages.Admin
{
    public class IndexModel : PageModel
    {
        public bool compteValide;
        private readonly IConfiguration configuration;

        public IndexModel(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void OnGet()
        {
            if (HttpContext.User?.Identity != null && HttpContext.User.Identity.IsAuthenticated)
            {
                Response.Redirect("/Admin/Nav");
            }
        }
        public async Task<IActionResult> OnPostAsync(string username, string password, string ReturnUrl)
        {
            var authSection = configuration.GetSection("Auth");
            string? adminLogin = authSection["AdminLogin"];
            string? adminPassword = authSection["AdminPassword"];

            if ((username == adminLogin)&&(password == adminPassword))
            {
                
                var claims = new List<Claim>
                 {
                 new Claim(ClaimTypes.Name, username)
                 };
                var claimsIdentity = new ClaimsIdentity(claims, "Login");
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new
               ClaimsPrincipal(claimsIdentity));
                compteValide = false;
                return Redirect(ReturnUrl == null ? "/Admin/Nav" : ReturnUrl);
            }
            else
            {
                compteValide = true;
                return Page();
            }
                
        }

        public async Task<IActionResult> OnGetLogout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/Admin");
        }
    }
}
