using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using ged_fdl_razor.Data;
using Microsoft.EntityFrameworkCore;
using ged_fdl_razor.Models;

namespace ged_fdl_razor.Pages.Commune
{
    public class LoginModel : PageModel
    {
        private readonly DataContext _context;
        public LoginModel(DataContext context) => _context = context;

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public bool CompteInvalide { get; set; } = false;

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            var commune = await _context.Communes
                .FirstOrDefaultAsync(c => c.Email == Email);

            if (commune != null && commune.PasswordHash == Password) // TODO: hash réel
            {
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, commune.Nom),
            new Claim("CommuneID", commune.CommuneID.ToString())
        };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                // Vérifie MustChangePassword
                if (commune.MustChangePassword)
                {
                    return Redirect($"/Commune/ChangePassword/{commune.CommuneID}");
                }

                return Redirect($"/Commune/Dossier/{commune.CommuneID}");
            }

            CompteInvalide = true;
            return Page();
        }


        public async Task<IActionResult> OnGetLogout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/Commune/Login");
        }
    }
}
