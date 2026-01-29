using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ged_fdl_razor.Data;
using ged_fdl_razor.Models;

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

            // Met à jour le mot de passe et désactive MustChangePassword
            commune.PasswordHash = NewPassword; // TODO : hash réel
            commune.MustChangePassword = false;

            await _context.SaveChangesAsync();

            return Redirect($"/Commune/Dossier/{commune.CommuneID}");
        }
    }
}
