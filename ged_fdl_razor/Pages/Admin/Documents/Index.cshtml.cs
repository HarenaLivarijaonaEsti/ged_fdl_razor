using ged_fdl_razor.Data;
using ged_fdl_razor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ged_fdl_razor.Pages.Admin.Documents
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly DataContext _context;

        public IndexModel(DataContext context)
        {
            _context = context;
        }

        public IList<Document> Document { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Document = await _context.Documents
                .Include(d => d.DossierFinancement)
                    .ThenInclude(df => df.Commune)
                .ToListAsync();
        }

        // Handler pour Télécharger le fichier
        public async Task<IActionResult> OnGetDownloadAsync(int id)
        {
            var document = await _context.Documents
                .FirstOrDefaultAsync(d => d.DocumentID == id);

            if (document == null)
                return NotFound();

            string fileName = GetFileNameWithExtension(document);

            return File(document.Contenu, document.ContentType, fileName);
        }

        // Handler pour Ouvrir le fichier dans le navigateur
        public async Task<IActionResult> OnGetOpenAsync(int id)
        {
            var document = await _context.Documents
                .FirstOrDefaultAsync(d => d.DocumentID == id);

            if (document == null)
                return NotFound();

            string fileName = GetFileNameWithExtension(document);

            // ⚡ "inline" permet d’ouvrir directement dans le navigateur
            Response.Headers.Add("Content-Disposition", $"inline; filename=\"{fileName}\"");

            return File(document.Contenu, document.ContentType);
        }

        // Utilitaire pour ajouter l’extension correcte si manquante
        private string GetFileNameWithExtension(Document document)
        {
            string extension = document.ContentType switch
            {
                "application/pdf" => ".pdf",
                "application/msword" => ".doc",
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document" => ".docx",
                "application/vnd.ms-excel" => ".xls",
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" => ".xlsx",
                _ => ""
            };

            if (!document.Nom.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
                return document.Nom + extension;

            return document.Nom;
        }
    }
}
