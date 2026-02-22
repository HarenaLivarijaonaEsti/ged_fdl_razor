using ged_fdl_razor.Data;
using ged_fdl_razor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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

        // ===============================
        // OnGet modifié pour recevoir communeId
        // ===============================
        public IActionResult OnGet(int? communeId)
        {
            ChargerCommunes();

            if (communeId.HasValue)
            {
                Remarque.CommuneId = communeId.Value;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ChargerCommunes();
                return Page();
            }

            // Date automatique
            Remarque.DateCreation = DateTime.Now;

            _context.Remarques.Add(Remarque);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        // ===============================
        // AJAX : Charger dossiers par commune
        // ===============================
        public async Task<JsonResult> OnGetDossiersByCommuneAsync(int communeId)
        {
            var dossiers = await _context.DossiersFinancement
                .Where(d => d.CommuneId == communeId)
                .Select(d => new
                {
                    id = d.Id,
                    titre = d.Titre
                })
                .OrderBy(d => d.titre)
                .ToListAsync();

            return new JsonResult(dossiers);
        }

        // ===============================
        // Charger uniquement les communes
        // ===============================
        private void ChargerCommunes()
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
        }
    }
}