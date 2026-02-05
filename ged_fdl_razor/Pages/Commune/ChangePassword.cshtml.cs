using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ged_fdl_razor.Data;
using CommuneModel = ged_fdl_razor.Models.Commune; // Alias pour éviter conflit

namespace ged_fdl_razor.Pages.Commune
{
    public class ChangePasswordModel : PageModel
    {
        private readonly DataContext _context;

        public ChangePasswordModel(DataContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string NewPassword { get; set; }

        [BindProperty]
        public string ConfirmPassword { get; set; }

        public string? ErrorMessage { get; set; }

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var commune = await _context.Communes.FindAsync(Id);
            if (commune == null) return NotFound();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (NewPassword != ConfirmPassword)
            {
                ErrorMessage = "Les mots de passe ne correspondent pas.";
                return Page();
            }

            var commune = await _context.Communes.FindAsync(Id);
            if (commune == null) return NotFound();

            // Hache uniquement si c'est le premier changement
            if (commune.MustChangePassword)
            {
                var hasher = new PasswordHasher<CommuneModel>();
                commune.PasswordHash = hasher.HashPassword(commune, NewPassword);
            }
            else
            {
                // Sinon on garde l'ancien mot de passe (pas de double hash)
                commune.PasswordHash = NewPassword;
            }

            // Toujours désactiver MustChangePassword après le changement
            commune.MustChangePassword = false;

            await _context.SaveChangesAsync();

            return Redirect($"/Commune/Dossier/{commune.CommuneID}");
        }
    }
}
