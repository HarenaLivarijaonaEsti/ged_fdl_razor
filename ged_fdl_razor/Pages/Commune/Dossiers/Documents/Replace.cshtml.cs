using ged_fdl_razor.Data;
using ged_fdl_razor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ged_fdl_razor.Pages.Commune.Dossiers.Documents
{
    public class ReplaceModel : PageModel
    {
        private readonly DataContext _context;

        public ReplaceModel(DataContext context)
        {
            _context = context;
        }

        // DOIT correspondre à @page "{documentId:int}"
        [BindProperty(SupportsGet = true)]
        public int DocumentId { get; set; }

        public Document Document { get; set; } = default!;

        [BindProperty]
        public IFormFile UploadFile { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            Document = await _context.Documents
                .Include(d => d.DossierFinancement)
                .FirstOrDefaultAsync(d => d.DocumentID == DocumentId);

            if (Document == null)
                return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var document = await _context.Documents
                .Include(d => d.DossierFinancement)
                .FirstOrDefaultAsync(d => d.DocumentID == DocumentId);

            if (document == null)
                return NotFound();

            if (UploadFile == null || UploadFile.Length == 0)
            {
                ModelState.AddModelError(nameof(UploadFile), "Veuillez sélectionner un fichier.");
                Document = document;
                return Page();
            }

            using var ms = new MemoryStream();
            await UploadFile.CopyToAsync(ms);

            document.Contenu = ms.ToArray();
            document.Taille = ms.Length;
            document.ContentType = UploadFile.ContentType;
            document.DateUpload = DateTime.UtcNow;

            // Mise à jour du flag
            var commune = await _context.Communes
                .FirstOrDefaultAsync(c => c.CommuneID == document.DossierFinancement!.CommuneId);
            if (commune != null)
                commune.HasNewDocuments = true;

            await _context.SaveChangesAsync();

            return RedirectToPage(
                "/Commune/Dossier",
                new { id = document.DossierFinancement!.CommuneId }
            );
        }
    }
}
