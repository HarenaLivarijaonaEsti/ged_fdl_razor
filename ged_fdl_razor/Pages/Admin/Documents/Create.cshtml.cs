using ged_fdl_razor.Data;
using ged_fdl_razor.Enums;
using ged_fdl_razor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ged_fdl_razor.Pages.Admin.Documents
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly DataContext _context;
        public CreateModel(DataContext context) => _context = context;

        [BindProperty]
        public Document Document { get; set; } = new();

        [BindProperty]
        public IFormFile UploadedFile { get; set; } = default!;

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            if (UploadedFile == null || UploadedFile.Length == 0)
            {
                ModelState.AddModelError("UploadedFile", "Le fichier est obligatoire.");
                return Page();
            }

            // Vérifier si le type existe déjà pour ce dossier
            var exists = await _context.Documents
                .AnyAsync(d => d.DossierFinancementId == Document.DossierFinancementId
                               && d.Type == Document.Type);

            if (exists)
            {
                ModelState.AddModelError("Document.Type", "Ce type de document existe déjà dans ce dossier.");
                return Page();
            }

            // Upload
            using var ms = new MemoryStream();
            await UploadedFile.CopyToAsync(ms);
            Document.Contenu = ms.ToArray();
            Document.Taille = UploadedFile.Length;
            Document.ContentType = UploadedFile.ContentType;
            Document.DateUpload = DateTime.UtcNow;

            _context.Documents.Add(Document);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        // Autocomplete dossiers
        public JsonResult OnGetSearchDossiers(string term)
        {
            if (string.IsNullOrWhiteSpace(term) || term.Length < 3)
                return new JsonResult(Array.Empty<object>());

            term = term.ToLower();
            var result = _context.DossiersFinancement
                .Include(d => d.Commune)
                .Where(d => d.Titre.ToLower().Contains(term) ||
                            d.Commune!.Nom.ToLower().Contains(term))
                .OrderBy(d => d.Titre)
                .Take(10)
                .Select(d => new { d.Id, libelle = d.Titre + " - " + d.Commune!.Nom })
                .ToList();

            return new JsonResult(result);
        }

        // Obtenir les types disponibles pour un dossier
        public async Task<JsonResult> OnGetGetAvailableTypes(int dossierId)
        {
            var usedTypes = await _context.Documents
                .Where(d => d.DossierFinancementId == dossierId)
                .Select(d => d.Type)
                .ToListAsync();

            var availableTypes = Enum.GetValues(typeof(TypeDocument))
                .Cast<TypeDocument>()
                .Except(usedTypes)
                .Select(t => t.ToString())
                .ToList();

            return new JsonResult(availableTypes);
        }
    }
}
