using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ged_fdl_razor.Data;
using ged_fdl_razor.Models;
using ged_fdl_razor.Enums;

namespace ged_fdl_razor.Pages.Admin.Dossiers
{
    public class CreateModel : PageModel
    {
        private readonly DataContext _context;

        public CreateModel(DataContext context)
        {
            _context = context;
        }

        [BindProperty]
        public DossierFinancement DossierFinancement { get; set; } = default!;

        // Pour initialiser les dropdowns
        private void PopulateDropdowns()
        {
            ViewData["CommuneId"] = new SelectList(
                _context.Communes
                        .Select(c => new { c.CommuneID, Display = c.Nom + " - " + c.District }),
                "CommuneID",
                "Display"
            );

            ViewData["StatutList"] = new SelectList(Enum.GetValues(typeof(StatutDossier)));
        }

        public IActionResult OnGet()
        {
            PopulateDropdowns();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                PopulateDropdowns();
                return Page();
            }

            // ⚠️ Remplir automatiquement la date de soumission
            DossierFinancement.DateSoumission = DateTime.UtcNow;

            int annee = DossierFinancement.DateSoumission.Value.Year;

            // Vérifier qu'un dossier pour la même commune et la même année n'existe pas
            bool existeDeja = await _context.DossiersFinancement
                .AnyAsync(d => d.CommuneId == DossierFinancement.CommuneId &&
                               d.DateSoumission.HasValue &&
                               d.DateSoumission.Value.Year == annee);

            if (existeDeja)
            {
                ModelState.AddModelError(string.Empty,
                    $"Un dossier pour cette commune existe déjà pour l'année {annee}.");
                PopulateDropdowns();
                return Page();
            }

            _context.DossiersFinancement.Add(DossierFinancement);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
