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

        public IList<Document> Document { get; set; } = new List<Document>();

        //  Point clé : paramètre reçu depuis Dossiers/Index
        public async Task OnGetAsync(int? dossierId)
        {
            IQueryable<Document> query = _context.Documents
                .Include(d => d.DossierFinancement)
                    .ThenInclude(df => df.Commune);

            //  Filtrage par dossier
            if (dossierId.HasValue)
            {
                query = query.Where(d => d.DossierFinancementId == dossierId);
            }

            Document = await query.ToListAsync();
        }

        //  Télécharger
        public async Task<IActionResult> OnGetDownloadAsync(int id)
        {
            var document = await _context.Documents
                .FirstOrDefaultAsync(d => d.DocumentID == id);

            if (document == null)
                return NotFound();

            string fileName = GetFileNameWithExtension(document);

            return File(document.Contenu, document.ContentType, fileName);
        }

        //  Ouvrir dans le navigateur
        public async Task<IActionResult> OnGetOpenAsync(int id)
        {
            var document = await _context.Documents
                .FirstOrDefaultAsync(d => d.DocumentID == id);

            if (document == null)
                return NotFound();

            string fileName = GetFileNameWithExtension(document);

            Response.Headers.Append(
                "Content-Disposition",
                $"inline; filename=\"{fileName}\""
            );

            return File(document.Contenu, document.ContentType);
        }

        //  Utilitaire extension
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

            return document.Nom.EndsWith(extension, StringComparison.OrdinalIgnoreCase)
                ? document.Nom
                : document.Nom + extension;
        }
    }
}
