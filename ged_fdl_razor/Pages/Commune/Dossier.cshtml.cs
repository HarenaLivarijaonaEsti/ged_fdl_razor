using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ged_fdl_razor.Data;
using ged_fdl_razor.Models;

namespace ged_fdl_razor.Pages.Commune
{
    public class DossierModel : PageModel
    {
        private readonly DataContext _context;

        public DossierModel(DataContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        public ged_fdl_razor.Models.Commune Commune { get; set; }


        public async Task<IActionResult> OnGetAsync()
        {
            Commune = await _context.Communes
                .Include(c => c.Dossiers)
                    .ThenInclude(d => d.Documents)
                .Include(c => c.Dossiers)
                    .ThenInclude(d => d.Remarques)
                .FirstOrDefaultAsync(c => c.CommuneID == Id);

            if (Commune == null)
                return NotFound();

            // Vérifie si le mot de passe doit être changé
            if (Commune.MustChangePassword)
            {
                return RedirectToPage("/Commune/ChangePassword", new { id = Commune.CommuneID });
            }

            return Page();
        }
    }
}
