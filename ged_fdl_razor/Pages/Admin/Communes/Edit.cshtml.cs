using ged_fdl_razor.Data;
using ged_fdl_razor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ged_fdl_razor.Pages.Admin.Communes
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly DataContext _context;

        public EditModel(DataContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ged_fdl_razor.Models.Commune Commune { get; set; } = default!;

        // GET: charger la commune
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
                return NotFound();

            var commune = await _context.Communes.FirstOrDefaultAsync(c => c.CommuneID == id);
            if (commune == null)
                return NotFound();

            Commune = commune;
            return Page();
        }

        // POST: sauvegarder la modification
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            // Charger l'entité depuis la DB pour éviter les erreurs EF
            var communeFromDb = await _context.Communes
                .FirstOrDefaultAsync(c => c.CommuneID == Commune.CommuneID);

            if (communeFromDb == null)
                return NotFound();

            // Mettre à jour les champs modifiables
            communeFromDb.Nom = Commune.Nom;
            communeFromDb.Email = Commune.Email;
            communeFromDb.District = Commune.District;
            communeFromDb.Region = Commune.Region;
            communeFromDb.IsActive = Commune.IsActive;

            // 🔹 Hachage du mot de passe si un nouveau mot de passe est fourni
            if (!string.IsNullOrWhiteSpace(Commune.PasswordHash))
            {
                var hasher = new PasswordHasher<ged_fdl_razor.Models.Commune>();
                communeFromDb.PasswordHash = hasher.HashPassword(communeFromDb, Commune.PasswordHash);
            }

            // 🔹 Mettre à jour le booléen MustChangePassword selon la sélection de l'admin
            communeFromDb.MustChangePassword = Commune.MustChangePassword;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Communes.Any(e => e.CommuneID == Commune.CommuneID))
                    return NotFound();
                else
                    throw;
            }

            // Rediriger vers la liste
            return RedirectToPage("./Index");
        }

        private bool CommuneExists(int id)
        {
            return _context.Communes.Any(e => e.CommuneID == id);
        }
    }
}
