using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using ged_fdl_razor.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace ged_fdl_razor.Pages.Commune
{
    public class LoginModel : PageModel
    {
        private readonly DataContext _context;
        public LoginModel(DataContext context) => _context = context;

        [BindProperty] public string Email { get; set; }
        [BindProperty] public string Password { get; set; }
        public bool CompteInvalide { get; set; } = false;

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            var commune = await _context.Communes.FirstOrDefaultAsync(c => c.Email == Email);
            if (commune == null)
            {
                CompteInvalide = true;
                return Page();
            }

            var hasher = new PasswordHasher<Models.Commune>();
            bool passwordOk = false;

            if (!string.IsNullOrEmpty(commune.PasswordHash) &&
                commune.PasswordHash.StartsWith("AQAAAA"))
            {
                var result = hasher.VerifyHashedPassword(commune, commune.PasswordHash, Password);
                passwordOk = result == PasswordVerificationResult.Success;
            }
            else
            {
                if (commune.PasswordHash == Password)
                {
                    passwordOk = true;
                    commune.PasswordHash = hasher.HashPassword(commune, Password);
                    commune.MustChangePassword = true;
                    await _context.SaveChangesAsync();
                }
            }

            if (!passwordOk)
            {
                CompteInvalide = true;
                return Page();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, commune.Nom),
                new Claim("CommuneID", commune.CommuneID.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, "CommuneCookie");
            await HttpContext.SignInAsync("CommuneCookie", new ClaimsPrincipal(claimsIdentity));

            if (commune.MustChangePassword)
                return Redirect($"/Commune/ChangePassword/{commune.CommuneID}");

            return Redirect($"/Commune/Dossier/{commune.CommuneID}");
        }

        public async Task<IActionResult> OnPostLogoutAsync()
        {
            await HttpContext.SignOutAsync("CommuneCookie");
            return Redirect("/Commune/Login");
        }
    }
}
