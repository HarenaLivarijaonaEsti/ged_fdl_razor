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
    public class EditModel : PageModel
    {
        private readonly DataContext _context;

        public EditModel(DataContext context)
        {
            _context = context;
        }

        [BindProperty]
        public DossierFinancement DossierFinancement { get; set; } = default!;

        // Dropdown lists
        public SelectList StatutList { get; set; } = default!;
        public SelectList CommuneList { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            var dossier = await _context.DossiersFinancement.FirstOrDefaultAsync(d => d.Id == id);
            if (dossier == null) return NotFound();

            DossierFinancement = dossier;

            // Initialiser DateSoumission si vide
            if (DossierFinancement.DateSoumission == null || DossierFinancement.DateSoumission == default)
            {
                DossierFinancement.DateSoumission = DateTime.UtcNow;
            }

            // Dropdown des statuts
            StatutList = new SelectList(Enum.GetValues(typeof(StatutDossier)), DossierFinancement.Statut);

            // Dropdown des communes (Nom + District)
            CommuneList = new SelectList(
                _context.Communes
                    .Select(c => new { c.CommuneID, Display = c.Nom + " - " + c.District }),
                "CommuneID",
                "Display",
                DossierFinancement.CommuneId
            );

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Recharger les dropdowns si erreur
                StatutList = new SelectList(Enum.GetValues(typeof(StatutDossier)), DossierFinancement.Statut);
                CommuneList = new SelectList(
                    _context.Communes
                        .Select(c => new { c.CommuneID, Display = c.Nom + " - " + c.District }),
                    "CommuneID",
                    "Display",
                    DossierFinancement.CommuneId
                );
                return Page();
            }

            // Vérifier et initialiser DateSoumission si jamais vide
            if (DossierFinancement.DateSoumission == null || DossierFinancement.DateSoumission == default)
            {
                DossierFinancement.DateSoumission = DateTime.UtcNow;
            }

            _context.Attach(DossierFinancement).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.DossiersFinancement.Any(d => d.Id == DossierFinancement.Id))
                    return NotFound();
                else
                    throw;
            }

            return RedirectToPage("./Index");
        }
    }
}
