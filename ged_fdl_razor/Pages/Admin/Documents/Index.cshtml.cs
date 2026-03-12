using ged_fdl_razor.Data;
using ged_fdl_razor.Enums;
using ged_fdl_razor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
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

        public async Task OnGetAsync(int? dossierId)
        {
            IQueryable<Document> query = _context.Documents
                .Include(d => d.DossierFinancement)
                .ThenInclude(df => df.Commune);

            if (dossierId.HasValue)
                query = query.Where(d => d.DossierFinancementId == dossierId);

            Document = await query.ToListAsync();
        }

        // Télécharger un document
        public async Task<IActionResult> OnGetDownloadAsync(int id)
        {
            var doc = await _context.Documents.FirstOrDefaultAsync(d => d.DocumentID == id);
            if (doc == null) return NotFound();

            return File(doc.Contenu, doc.ContentType, doc.Nom);
        }

        // Ouvrir un document dans le navigateur
        public async Task<IActionResult> OnGetOpenAsync(int id)
        {
            var doc = await _context.Documents.FirstOrDefaultAsync(d => d.DocumentID == id);
            if (doc == null) return NotFound();

            Response.Headers.Append("Content-Disposition", $"inline; filename=\"{doc.Nom}\"");
            return File(doc.Contenu, doc.ContentType);
        }

        // DTO pour binder la liste d'IDs et leur état
        public class DocumentUpdateDto
        {
            public int Id { get; set; }
            public bool IsChecked { get; set; } // true si coché, false sinon
        }

        // Wrapper pour binder la liste
        public class DocumentUpdateWrapper
        {
            public List<DocumentUpdateDto> Documents { get; set; } = new List<DocumentUpdateDto>();
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostValiderDocumentsAsync([FromBody] DocumentUpdateWrapper data)
        {
            if (data?.Documents == null || !data.Documents.Any())
                return new JsonResult(new { success = false });

            // Récupérer le dossierId pour mettre à jour tous les documents du même dossier
            var dossierId = await _context.Documents
                .Where(d => d.DocumentID == data.Documents.First().Id)
                .Select(d => d.DossierFinancementId)
                .FirstOrDefaultAsync();

            var allDocuments = await _context.Documents
                .Where(d => d.DossierFinancementId == dossierId)
                .ToListAsync();

            foreach (var doc in allDocuments)
            {
                var dto = data.Documents.FirstOrDefault(d => d.Id == doc.DocumentID);
                if (dto != null)
                    doc.EstVerifie = dto.IsChecked;
            }

            // Mettre à jour le statut du dossier
            var dossier = await _context.DossiersFinancement.FirstOrDefaultAsync(d => d.Id == dossierId);
            if (dossier != null)
            {
                int nombreVerifies = allDocuments.Count(d => d.EstVerifie);
                const int seuilValidation = 11;

                if (nombreVerifies >= seuilValidation && dossier.Statut == StatutDossier.Soumis)
                {
                    dossier.Statut = StatutDossier.Valide;
                }
                else if (nombreVerifies < seuilValidation && dossier.Statut == StatutDossier.Valide)
                {
                    dossier.Statut = StatutDossier.Soumis;
                }
            }

            await _context.SaveChangesAsync();

            return new JsonResult(new { success = true });
        }
    }
}