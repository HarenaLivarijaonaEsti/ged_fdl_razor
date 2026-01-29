using ged_fdl_razor.Data;
using ged_fdl_razor.Enums;
using ged_fdl_razor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ged_fdl_razor.Pages.Admin.Documents
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
        public Document Document { get; set; } = new();

        // Nouveau : fichier uploadé
        [BindProperty]
        public IFormFile UploadedFile { get; set; } = default!;

        public IActionResult OnGet()
        {
            // Dropdown TypeDocument (enum)
            ViewData["TypeDocument"] =
                new SelectList(Enum.GetValues(typeof(TypeDocument)));

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ViewData["TypeDocument"] = new SelectList(Enum.GetValues(typeof(TypeDocument)));
                return Page();
            }

            // Gérer l'upload
            if (UploadedFile != null && UploadedFile.Length > 0)
            {
                using var ms = new MemoryStream();
                await UploadedFile.CopyToAsync(ms);
                Document.Contenu = ms.ToArray();
                Document.Taille = UploadedFile.Length;
                Document.ContentType = UploadedFile.ContentType;
            }
            else
            {
                ModelState.AddModelError("UploadedFile", "Le fichier est obligatoire.");
                ViewData["TypeDocument"] = new SelectList(Enum.GetValues(typeof(TypeDocument)));
                return Page();
            }

            _context.Documents.Add(Document);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        // Autocomplete DossierFinancement insensible à la casse
        public JsonResult OnGetSearchDossiers(string term)
        {
            if (string.IsNullOrWhiteSpace(term) || term.Length < 3)
                return new JsonResult(Array.Empty<object>());

            term = term.ToLower();

            var resultats = _context.DossiersFinancement
                .Include(d => d.Commune)
                .Where(d =>
                    d.Titre.ToLower().Contains(term) ||
                    d.Commune!.Nom.ToLower().Contains(term))
                .OrderBy(d => d.Titre)
                .Take(10)
                .Select(d => new
                {
                    d.Id,
                    Libelle = d.Titre + " - " + d.Commune!.Nom
                })
                .ToList();

            return new JsonResult(resultats);
        }
    }
}
