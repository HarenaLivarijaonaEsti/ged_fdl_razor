using ged_fdl_razor.Data;
using ged_fdl_razor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ged_fdl_razor.Pages.Admin.Remarques
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly DataContext _context;

        public CreateModel(DataContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Remarque Remarque { get; set; } = new();

        public IActionResult OnGet()
        {
            ChargerListes();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // 🔁 Important : recharger les listes en cas d’erreur
                ChargerListes();
                return Page();
            }

            // ✅ Date définie automatiquement au moment du Create
            Remarque.DateCreation = DateTime.Now;
            // 👉 remplace par DateTime.Now si tu veux l’heure locale

            _context.Remarques.Add(Remarque);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        // 🔧 Méthode utilitaire pour éviter la duplication
        private void ChargerListes()
        {
            ViewData["CommuneId"] = new SelectList(
                _context.Communes
                    .Select(c => new
                    {
                        c.CommuneID,
                        Libelle = c.Nom + " - " + c.District
                    })
                    .OrderBy(c => c.Libelle),
                "CommuneID",
                "Libelle"
            );

            ViewData["DossierFinancementId"] =
                new SelectList(_context.DossiersFinancement, "Id", "Titre");
        }
    }
}
