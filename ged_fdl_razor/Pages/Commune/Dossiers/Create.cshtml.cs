using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ged_fdl_razor.Data;
using ged_fdl_razor.Models;

namespace ged_fdl_razor.Pages.Commune.Dossiers
{
    public class CreateModel : PageModel
    {
        private readonly DataContext _context;

        public CreateModel(DataContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public int CommuneId { get; set; }

        public ged_fdl_razor.Models.Commune Commune { get; set; }


        [BindProperty]
        public DossierFinancement Dossier { get; set; } = new DossierFinancement();

        public async Task<IActionResult> OnGetAsync(int communeId)
        {
            Commune = await _context.Communes.FindAsync(communeId);

            if (Commune == null)
                return NotFound();

            // Préremplir le lien avec la commune
            Dossier.CommuneId = Commune.CommuneID;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Commune = await _context.Communes.FindAsync(Dossier.CommuneId);
            if (Commune == null)
                return NotFound();

            if (!ModelState.IsValid)
                return Page();

            Dossier.DateCreation = DateTime.UtcNow;
            Dossier.Statut = Enums.StatutDossier.Brouillon;

            _context.DossiersFinancement.Add(Dossier);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Commune/Dossier", new { id = Commune.CommuneID });
        }
    }
}
